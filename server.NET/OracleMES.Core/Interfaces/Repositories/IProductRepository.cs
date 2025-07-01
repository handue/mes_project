using OracleMES.Core.Entities;

namespace OracleMES.Core.Interfaces.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetActiveProductsAsync();
    Task<IEnumerable<Product>> GetByCategoryAsync(string category);
    Task<IEnumerable<Product>> GetByCostRangeAsync(decimal minCost, decimal maxCost);

    Task UpdateProcessTimeAsync(decimal productId, decimal standardProcessTime);
    Task<IEnumerable<Product>> GetByProcessTimeRangeAsync(decimal minTime, decimal maxTime);

}