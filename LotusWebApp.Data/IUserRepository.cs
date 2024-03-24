using Microsoft.EntityFrameworkCore;

namespace LotusWebApp.Data;

public interface IUserRepository
{
    Task<int> CreateUser(string name, string role, CancellationToken cancellationToken);
    Task<int> DeleteUser(Guid userId, CancellationToken cancellationToken);
    Task<int> UpdateUser(Guid userId, string name, string role, CancellationToken cancellationToken);
    Task<User?> GetUser(Guid userId, CancellationToken cancellationToken);
    Task<List<User>> GetAllUsers(CancellationToken cancellationToken);
}

public sealed class UserRepository(MainDbContext context)
    : RepositoryBase<User>(context), IUserRepository, IRegisterRepository
{
    public async Task<int> CreateUser(string name, string role, CancellationToken cancellationToken)
    {
        await Context.Users.AddAsync(new User
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
        var user = await Context.Users.FindAsync(new object?[] { userId }, cancellationToken: cancellationToken);
        if (user == null)
        {
            return 0;
        }
        Context.Users.Remove(user);
        return await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> UpdateUser(Guid userId, string name, string role, CancellationToken cancellationToken)
    {
        var user = await Context.Users.FindAsync(new object?[] { userId }, cancellationToken: cancellationToken);
        if (user == null)
        {
            return 0;
        }
        Context.ChangeTracker.Clear();
        user.Name = name;
        user.Role = role;

        Context.Users.Update(user);
        return await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task<User?> GetUser(Guid userId, CancellationToken cancellationToken)
    {
        var user = await Context.Users.FindAsync(new object?[] { userId }, cancellationToken: cancellationToken);
        return user;
    }

    public async Task<List<User>> GetAllUsers(CancellationToken cancellationToken)
    {
        return await Context.Users.ToListAsync(cancellationToken);
    }
}