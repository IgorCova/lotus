using System.Diagnostics.Metrics;

namespace LotusWebApp;

public class ContosoMetrics
{
    private readonly Counter<int> _userCreatedCounter;

    public ContosoMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("Contoso.Web");
        _userCreatedCounter = meter.CreateCounter<int>("contoso.user.created");
    }

    public void UserCreated(string userName, int quantity = 1)
    {
        _userCreatedCounter.Add(quantity,
            new KeyValuePair<string, object?>("contoso.user.name", userName));
    }
}