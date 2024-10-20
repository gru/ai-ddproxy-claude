namespace AI.DaDataProxy.Http.Contracts;

/// <summary>
/// DaData.
/// </summary>
public class DaDataDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the DaData.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the DaData.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}