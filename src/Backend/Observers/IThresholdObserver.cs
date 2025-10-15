namespace Backend.Observers;
using Backend.Services;

public interface IThresholdObserver : IDisposable
{
    void Subscribe(ISensorDataService observable);
    void Unsubscribe(ISensorDataService observable);
}