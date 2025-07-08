using OracleMES.Core.Interfaces.Repositories;
using OracleMES.Core.Entities;
using OracleMES.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace OracleMES.Infrastructure.Repositories
{
    public class InventoryRepository : Repository<Inventory>, IInventoryRepository
    {
        public InventoryRepository(MesDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Inventory>> GetByCategoryAsync(string category)
        {
            return await _context.Inventories
                .Where(i => i.Category == category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inventory>> GetLowStockItemsAsync()
        {
            return await _context.Inventories
                .Where(i => i.Quantity <= i.Reorderlevel)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inventory>> GetOutOfStockItemsAsync()
        {
            return await _context.Inventories
                .Where(i => i.Quantity == 0)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inventory>> GetBySupplierAsync(decimal supplierId)
        {
            return await _context.Inventories
                .Where(i => i.Supplierid == supplierId)
                .ToListAsync();
        }

        public async Task UpdateStockAsync(decimal itemId, decimal quantity)
        {
            var item = await _context.Inventories
                .FirstOrDefaultAsync(i => i.Itemid == itemId);
            
            if (item != null)
            {
                item.Quantity = quantity;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Inventory>> GetItemsNeedingReorderAsync()
        {
            return await _context.Inventories
                .Where(i => i.Quantity <= i.Reorderlevel)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inventory>> GetByLocationAsync(string location)
        {
            return await _context.Inventories
                .Where(i => i.Location == location)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inventory>> GetByCostRangeAsync(decimal minCost, decimal maxCost)
        {
            return await _context.Inventories
                .Where(i => i.Cost >= minCost && i.Cost <= maxCost)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inventory>> GetByLeadTimeRangeAsync(decimal minLeadTime, decimal maxLeadTime)
        {
            return await _context.Inventories
                .Where(i => i.Leadtime >= minLeadTime && i.Leadtime <= maxLeadTime)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalInventoryValueAsync()
        {
            return await _context.Inventories
                .SumAsync(i => i.Cost * i.Quantity);
        }

        public async Task UpdateStockLevelAsync(decimal itemId, decimal newQuantity)
        {
            await UpdateStockAsync(itemId, newQuantity);
        }
    }
} 