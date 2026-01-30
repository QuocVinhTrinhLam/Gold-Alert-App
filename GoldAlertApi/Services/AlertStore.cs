using System.Collections.Concurrent;
using GoldAlertApi.Models;

namespace GoldAlertApi.Services;

public interface IAlertStore
{
    IEnumerable<Alert> GetAll();
    Alert Add(CreateAlertRequest request);
    void Delete(int id);
}

public class InMemoryAlertStore : IAlertStore
{
    private readonly ConcurrentDictionary<int, Alert> _alerts = new();
    private int _nextId = 1;

    public IEnumerable<Alert> GetAll()
    {
        return _alerts.Values;
    }

    public Alert Add(CreateAlertRequest request)
    {
        var alert = new Alert
        {
            Id = _nextId++,
            TargetPrice = request.TargetPrice,
            Condition = request.Condition,
            CreatedAt = DateTime.UtcNow
        };
        _alerts.TryAdd(alert.Id, alert);
        return alert;
    }

    public void Delete(int id)
    {
        _alerts.TryRemove(id, out _);
    }
}
