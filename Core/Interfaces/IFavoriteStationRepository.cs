using System.Collections.Immutable;

using Core.Data.Entities;

namespace Core.Interfaces;

public interface IFavoriteStationRepository
{
    Task<ImmutableList<FavoriteStation>> GetAllAsync();
    Task<FavoriteStation> AddAsync(FavoriteStation station);
    Task<bool> RemoveAsync(int id);
    Task<bool> ExistsAsync(Guid externalStationId);
}