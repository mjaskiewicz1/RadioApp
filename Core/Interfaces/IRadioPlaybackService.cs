using Core.Models;

namespace Core.Interfaces;

public interface IRadioPlaybackService
{
    RadioStation? CurrentStation { get; }
    bool IsPlaying { get; }

    event EventHandler? StateChanged;
    event EventHandler? InitializationFailed;
    event EventHandler? PlaybackFailed;

    void Play(RadioStation station);
    void Resume();
    void Pause();
    void Stop();
}