using RestEase;

namespace AI.DaDataProxy.Http.Contracts;

/// <summary>
/// Represents the contract for the DaData API (version 1).
/// This interface defines the operations available for interacting with DaData resources.
/// It is designed to be used with RestEase for generating API clients.
/// </summary>
[Header("Accept", "application/json")]
public interface IDaDataV1Controller
{
    /// <summary>
    /// Creates a new DaData.
    /// </summary>
    /// <param name="command">The command containing the details for creating the DaData.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>The ID of the newly created DaData.</returns>
    [Post("api/v1/DaData")]
    Task<long> CreateDaData([Body] CreateDaDataCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an DaData by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the DaData to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>The DaData DTO containing the details of the requested DaData.</returns>
    [Get("api/v1/DaData/{id}")]
    Task<DaDataDto> GetDaData([Path] long id, CancellationToken cancellationToken = default);
}