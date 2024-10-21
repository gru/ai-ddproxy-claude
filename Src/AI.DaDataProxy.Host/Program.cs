using System.Diagnostics;
using System.Reflection;
using AI.DaDataProxy;
using AI.DaDataProxy.DaData;
using AI.DaDataProxy.Entities;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;
using Serilog;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using RestEase.HttpClientFactory;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .AddUserSecrets<Program>();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Destructure.ByTransforming<Exception>(ex => ex.Demystify())
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddFeatureManagement();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions.Add("correlationId",
            Activity.Current?.Id ?? context.HttpContext.TraceIdentifier);

        var exceptionHandlerFeature = context.HttpContext.Features.Get<IExceptionHandlerFeature>();
        if (exceptionHandlerFeature != null)
        {
            if (exceptionHandlerFeature.Error is ValidationException validationException)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest; 
                context.ProblemDetails.Status = StatusCodes.Status400BadRequest;
                context.ProblemDetails.Title = "Validation error";
                context.ProblemDetails.Detail = "One or more validation errors occurred.";
                context.ProblemDetails.Extensions.Add("errors", validationException.Errors.Select(e => new
                {
                    e.PropertyName,
                    e.ErrorMessage
                }));
            }

            if (exceptionHandlerFeature.Error is DaDataIntegrationException)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                context.ProblemDetails.Status = StatusCodes.Status503ServiceUnavailable;
                context.ProblemDetails.Title = "DaData Integration Error";
                context.ProblemDetails.Detail = "An error occurred while communicating with the DaData service.";
            }

            if (exceptionHandlerFeature.Error is DaDataTooManyRequests)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.ProblemDetails.Status = StatusCodes.Status429TooManyRequests;
                context.ProblemDetails.Title = "Too Many Requests";
                context.ProblemDetails.Detail = "The daily request limit for DaData service has been exceeded.";
            }

            if (builder.Environment.IsDevelopment())
            {
                context.ProblemDetails.Detail = exceptionHandlerFeature.Error.Demystify().ToString();
            }
        }
    };
});

builder.Services.AddSingleton<IConnectionMultiplexer>(_ => 
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));

builder.Services.AddOptions<DaDataOptions>()
    .Bind(builder.Configuration.GetSection("DaData"))
    .ValidateDataAnnotations();

builder.Services.AddOptions<DaDataCachingOptions>()
    .Bind(builder.Configuration.GetSection("DaDataCaching"))
    .ValidateDataAnnotations();

builder.Services
    .AddRestEaseClient<IDaDataApi>()
    .ConfigureHttpClient((provider, client) =>
    {
        var options = provider.GetRequiredService<IOptions<DaDataOptions>>().Value;
        client.BaseAddress = new Uri(options.BaseUrl);
        client.DefaultRequestHeaders.Add("Authorization", $"Token {options.ApiKey}");
        client.DefaultRequestHeaders.Add("X-Secret", options.Secret);
    });

builder.Services.AddDbContextPool<DaDataProxyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), 
        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure()));

var aiProjectAssembly = Assembly.Load("AI.DaDataProxy");
builder.Services.AddValidatorsFromAssembly(aiProjectAssembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDaDataProxyServices();

var app = builder.Build();

var versionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
var swaggerGenOptions = app.Services.GetRequiredService<IOptions<SwaggerGenOptions>>();

foreach (var description in versionProvider.ApiVersionDescriptions)
{
    swaggerGenOptions.Value.SwaggerGeneratorOptions.SwaggerDocs.Add(
        description.GroupName,
        new OpenApiInfo
        {
            Title = $"AI.DaDataProxy API {description.ApiVersion}",
            Version = description.ApiVersion.ToString()
        });
}

var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
app.Services.GetRequiredService<IOptions<SwaggerGenOptions>>().Value.IncludeXmlComments(xmlPath);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in versionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName);
        }
    });
    
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
