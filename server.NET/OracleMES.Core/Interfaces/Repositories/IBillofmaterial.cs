using OracleMES.Core.Entities;

namespace OracleMES.Core.Interfaces.Repositories;

public interface IBillofmaterialRepository : IRepository<Billofmaterial>
{
    Task<IEnumerable<Billofmaterial>> GetByProductAsync(decimal productId); 
    Task<IEnumerable<Billofmaterial>> GetByComponentAsync(decimal componentId); 
    Task<IEnumerable<Billofmaterial>> GetByQuantityRangeAsync(decimal minQuantity, decimal maxQuantity); 
    Task<IEnumerable<Billofmaterial>> GetByScrapFactorRangeAsync(decimal minScrap, decimal maxScrap); 
}