using System.Collections.ObjectModel;

using Core.Data.Entities;
using Core.Extensions;
using Core.Interfaces;
using Core.Models;

namespace RadioApp.ViewModels;

public sealed class FavoriteStationsViewModel(IFavoriteStationRepository favoriteStationRepository)
{
    private readonly SemaphoreSlim _operationLock = new(1, 1);

    public ObservableCollection<FavoriteStation> FavoriteStations { get; } = [];

    public Task LoadAsync()
        => ExecuteAsync(async () =>
        {
            var stations = await favoriteStationRepository.GetAllAsync();

            FavoriteStations.Clear();

            foreach (var station in stations)
                FavoriteStations.Add(station);

            return true;
        });

    public bool Contains(Guid externalStationId)
        => FavoriteStations.Any(station => station.ExternalStationId == externalStationId);

    public Task<bool> AddAsync(RadioStation station)
        => ExecuteAsync(() => AddStationAsync(station));

    public Task<bool> ToggleAsync(RadioStation station)
        => ExecuteAsync(async () =>
        {
            var favoriteStation = FavoriteStations.FirstOrDefault(
                favorite => favorite.ExternalStationId == station.Id);

            if (favoriteStation is not null)
            {
                await RemoveStationAsync(favoriteStation);
                return false;
            }

            await AddStationAsync(station);
            return true;
        });

    public Task<bool> RemoveAsync(FavoriteStation station)
        => ExecuteAsync(() => RemoveStationAsync(station));

    private async Task<bool> AddStationAsync(RadioStation station)
    {
        if (Contains(station.Id))
            return false;

        var favoriteStation = await favoriteStationRepository.AddAsync(station.ToFavoriteStation());
        FavoriteStations.Insert(0, favoriteStation);
        return true;
    }

    private async Task<bool> RemoveStationAsync(FavoriteStation station)
    {
        var removed = await favoriteStationRepository.RemoveAsync(station.Id);
        var currentItem = FavoriteStations.FirstOrDefault(favorite => favorite.Id == station.Id);

        // A missing database row is already deleted; also remove any stale item from the view.
        return currentItem is not null ? FavoriteStations.Remove(currentItem) : removed;
    }

    private async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        await _operationLock.WaitAsync();

        try
        {
            return await action();
        }
        finally
        {
            _operationLock.Release();
        }
    }
}