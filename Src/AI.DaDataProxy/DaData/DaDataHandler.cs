using AI.DaDataProxy.Entities;
using AI.DaDataProxy.Http.Contracts;
using FluentValidation;

namespace AI.DaDataProxy.DaData;

/// <summary>
/// Handles business logic operations related to DaDatas.
/// This class is responsible for creating and retrieving DaData entities.
/// </summary>
public class DaDataHandler
{
    private readonly DaDataProxyDbContext _dbContext;
    private readonly IValidator<CreateDaDataCommand> _validator;

    /// <summary>
    /// Initializes a new instance of the DaDataHandler class.
    /// </summary>
    /// <param name="dbContext">The database context for accessing DaData entities.</param>
    /// <param name="validator">The validator for CreateDaDataCommand.</param>
    public DaDataHandler(DaDataProxyDbContext dbContext, IValidator<CreateDaDataCommand> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }

    /// <summary>
    /// Creates a new DaData entity based on the provided command.
    /// </summary>
    /// <param name="command">The command containing details for creating the DaData.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>The ID of the newly created DaData.</returns>
    /// <exception cref="ValidationException">Thrown when the command fails validation.</exception>
    public async Task<long> CreateDaData(CreateDaDataCommand command, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var aggregate = new DaDataEntity
        {
            Name = command.Name
        };

        _dbContext.DaDatas.Add(aggregate);
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        return aggregate.Id;
    }

    /// <summary>
    /// Retrieves an DaData entity by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the DaData to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>The retrieved DaDataEntity.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an DaData with the specified ID is not found.</exception>
    public async Task<DaDataEntity> GetDaData(long id, CancellationToken cancellationToken)
    {
        var aggregate = await _dbContext.DaDatas.FindAsync([id], cancellationToken);
            
        if (aggregate == null)
        {
            throw new InvalidOperationException($"DaData with id {id} not found");
        }

        return aggregate;
    }
}