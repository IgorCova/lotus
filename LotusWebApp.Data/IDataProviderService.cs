namespace LotusWebApp.Data;

public interface IDataProviderService
{
    Task<int> CreateUser(string name, string role, CancellationToken cancellationToken);
    Task<int> DeleteUser(Guid userId, CancellationToken cancellationToken);
    Task<int> UpdateUser(Guid userId, string name, string role, CancellationToken cancellationToken);
    Task<Customer?> GetUser(Guid userId, CancellationToken cancellationToken);
    Task<List<Customer>> GetAllUsers(CancellationToken cancellationToken);
}

public class DataProviderService(IUserRepository userRepository) : IDataProviderService
{
    public async Task<int> CreateUser(string name, string role, CancellationToken cancellationToken)
    {
        return await userRepository.CreateUser(name, role, cancellationToken);
    }

    public async Task<int> DeleteUser(Guid userId, CancellationToken cancellationToken)
    {
        return await userRepository.DeleteUser(userId, cancellationToken);
    }

    public async Task<int> UpdateUser(Guid userId, string name, string role, CancellationToken cancellationToken)
    {
        return await userRepository.UpdateUser(userId, name, role, cancellationToken);
    }

    public async Task<Customer?> GetUser(Guid userId, CancellationToken cancellationToken)
    {
        return await userRepository.GetUser(userId, cancellationToken);
    }

    public async Task<List<Customer>> GetAllUsers(CancellationToken cancellationToken)
    {
        return await userRepository.GetAllUsers(cancellationToken);
    }
}