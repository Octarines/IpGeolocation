namespace Octarines.IpGeolocation.Repositories;

public interface IRepository<T>
{
    public Task<T> GetAsync(string id);
    public Task<IEnumerable<T>> GetAsync();
    public Task<T> InsertAsync(T instance);
    public Task<T> UpdateAsync(T instance);
}