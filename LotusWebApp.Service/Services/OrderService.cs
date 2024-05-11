namespace LotusWebApp.Services;

public interface IOrderService
{
    Task<bool> MakeAnOrder(string userId, decimal amount, CancellationToken cancellationToken);
}

public class OrderService(IBillingService billingService): IOrderService
{
    public async Task<bool> MakeAnOrder(string userId, decimal amount, CancellationToken cancellationToken)
    {
        return await billingService.UserPayOrder(userId, amount, cancellationToken);
    }
}