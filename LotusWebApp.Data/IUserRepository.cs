using Microsoft.EntityFrameworkCore;

namespace LotusWebApp.Data;

public interface IUserRepository
{
    Task<int> CreateUser(string name, string role, CancellationToken cancellationToken);
    Task<int> DeleteUser(Guid userId, CancellationToken cancellationToken);
    Task<int> UpdateUser(Guid userId, string name, string role, CancellationToken cancellationToken);
    Task<Customer?> GetUser(Guid userId, CancellationToken cancellationToken);
    Task<List<Customer>> GetAllUsers(CancellationToken cancellationToken);
}

public sealed class UserRepository(ApplicationDbContext context)
    : RepositoryBase<Customer>(context), IUserRepository, IRegisterRepository
{
    public async Task<int> CreateUser(string name, string role, CancellationToken cancellationToken)
    {
        await Context.Customers.AddAsync(new Customer
        {
            Id = Guid.NewGuid(),
            Name = name,
            Role = role,
            CreatedDate = DateTime.Now,
        }, cancellationToken);
        return await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> DeleteUser(Guid userId, CancellationToken cancellationToken)
    {
        var user = await Context.Customers.FindAsync(new object?[] { userId }, cancellationToken: cancellationToken);
        if (user == null)
        {
            return 0;
        }
        Context.Customers.Remove(user);
        return await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> UpdateUser(Guid userId, string name, string role, CancellationToken cancellationToken)
    {
        var user = await Context.Customers.FindAsync(new object?[] { userId }, cancellationToken: cancellationToken);
        if (user == null)
        {
            return 0;
        }
        Context.ChangeTracker.Clear();
        user.Name = name;
        user.Role = role;

        Context.Customers.Update(user);
        return await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Customer?> GetUser(Guid userId, CancellationToken cancellationToken)
    {
        var user = await Context.Customers.FindAsync(new object?[] { userId }, cancellationToken: cancellationToken);
        return user;
    }

    public async Task<List<Customer>> GetAllUsers(CancellationToken cancellationToken)
    {
        return await Context.Customers.ToListAsync(cancellationToken);
    }
}