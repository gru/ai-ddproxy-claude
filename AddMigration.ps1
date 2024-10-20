# Prompt for variables
$migrationName = Read-Host "Enter the migration name"
$projectPath = "Src/AI.DaDataProxy.Entities/AI.DaDataProxy.Entities.csproj"
$startupProjectPath = "Src/AI.DaDataProxy.Migrations/AI.DaDataProxy.Migrations.csproj"
$context = "AI.DaDataProxy.Entities.DaDataProxyDbContext"

# Create the migration
$command = "dotnet ef migrations add $migrationName --project `"$projectPath`" --startup-project `"$startupProjectPath`" --context `"$context`""

# Output the command (optional, for verification)
Write-Host "Executing command: $command"

# Execute the command
Invoke-Expression $command

# Check if the command was successful
if ($LASTEXITCODE -eq 0) {
    Write-Host "Migration '$migrationName' created successfully." -ForegroundColor Green
} else {
    Write-Host "Failed to create migration '$migrationName'. Please check the error messages above." -ForegroundColor Red
}