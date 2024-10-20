using AI.DaDataProxy.DaData;
using AI.DaDataProxy.Http.Contracts;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.FeatureManagement.Mvc;

namespace AI.DaDataProxy.Host.Controllers;

/// <summary>
/// Controller responsible for handling DaData-related HTTP requests.
/// This controller is versioned and mapped to the route "api/v{version:apiVersion}/[controller]".
/// </summary>
[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/[controller]")]
[FeatureGate(FeatureToggles.ApiEnabled)]
public class DaDataController : ControllerBase
{
    private readonly DaDataHandler _aggregateHandler;

    /// <summary>
    /// Initializes a new instance of the DaDataController class.
    /// </summary>
    /// <param name="aggregateHandler">The handler for aggregate-related operations.</param>
    public DaDataController(DaDataHandler aggregateHandler)
    {
        _aggregateHandler = aggregateHandler;
    }

    /// <summary>
    /// Creates a new DaData.
    /// </summary>
    /// <param name="command">The command containing the details for creating the DaData.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>An ActionResult containing the ID of the newly created DaData.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateDaData([FromBody] CreateDaDataCommand command, CancellationToken cancellationToken)
    {
        var aggregateId = await _aggregateHandler.CreateDaData(command, cancellationToken);
        return CreatedAtAction(nameof(GetDaData), new { id = aggregateId }, aggregateId);
    }

    /// <summary>
    /// Retrieves an DaData by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the DaData to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>An ActionResult containing the DaDataDto of the requested DaData.</returns>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<DaDataDto>> GetDaData(long id, CancellationToken cancellationToken)
    {
        var aggregate = await _aggregateHandler.GetDaData(id, cancellationToken);
        return Ok(aggregate);
    }
}