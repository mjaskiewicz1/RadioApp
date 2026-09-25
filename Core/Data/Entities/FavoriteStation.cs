namespace Core.Data.Entities;

public sealed class FavoriteStation
{
    public long Id { get; set; }
    public Guid ExternalStationId { get; set; }
    public required string Name { get; set; }
    public required Uri StreamUrl { get; set; }
    public Uri? Favicon { get; set; }
    public int?  Bitrate { get; set; }
}