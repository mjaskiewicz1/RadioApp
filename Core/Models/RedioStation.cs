namespace Core.Models;

public sealed record RadioStation(Guid Id, string Name, Uri StreamUrl, Uri? Favicon, int? Bitrate);