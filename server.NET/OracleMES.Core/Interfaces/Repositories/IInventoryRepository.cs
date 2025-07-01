using OracleMES.Core.Entities;

namespace OracleMES.Core.Interfaces.Repositories;

public interface IInventoryRepository : IRepository<Inventory>
{
    Task<IEnumerable<Inventory>> GetByCategoryAsync(string category); 
    Task<IEnumerable<Inventory>> GetLowStockItemsAsync(); 
    Task<IEnumerable<Inventory>> GetOutOfStockItemsAsync(); 
    Task<IEnumerable<Inventory>> GetBySupplierAsync(decimal supplierId); 

    Task UpdateStockAsync(decimal itemId, decimal quantity); 
    Task<IEnumerable<Inventory>> GetItemsNeedingReorderAsync(); 
    Task<IEnumerable<Inventory>> GetByLocationAsync(string location); 
    Task<IEnumerable<Inventory>> GetByCostRangeAsync(decimal minCost, decimal maxCost); 

    Task<IEnumerable<Inventory>> GetByLeadTimeRangeAsync(decimal minLeadTime, decimal maxLeadTime); 
}