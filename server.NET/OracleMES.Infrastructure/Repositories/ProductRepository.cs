using OracleMES.Core.Interfaces.Repositories;
using OracleMES.Core.Entities;
using OracleMES.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace OracleMES.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(MesDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            return await _context.Products
                .Where(p => p.Isactive == 1)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
        {
            return await _context.Products
                .Where(p => p.Category == category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByCostRangeAsync(decimal minCost, decimal maxCost)
        {
            return await _context.Products
                .Where(p => p.Cost >= minCost && p.Cost <= maxCost)
                .ToListAsync();
        }

        public async Task UpdateProcessTimeAsync(decimal productId, decimal standardProcessTime)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Productid == productId);
            
            if (product != null)
            {
                product.Standardprocesstime = standardProcessTime;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Product>> GetByProcessTimeRangeAsync(decimal minTime, decimal maxTime)
        {
            return await _context.Products
                .Where(p => p.Standardprocesstime >= minTime && p.Standardprocesstime <= maxTime)
                .ToListAsync();
        }
    }
} 