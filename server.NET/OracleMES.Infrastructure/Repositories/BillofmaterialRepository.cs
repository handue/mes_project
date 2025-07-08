using OracleMES.Core.Interfaces.Repositories;
using OracleMES.Core.Entities;
using OracleMES.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace OracleMES.Infrastructure.Repositories
{
    public class BillofmaterialRepository : Repository<Billofmaterial>, IBillofmaterialRepository
    {
        public BillofmaterialRepository(MesDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Billofmaterial>> GetByProductAsync(decimal productId)
        {
            return await _context.Billofmaterials
                .Where(b => b.Productid == productId)
                .ToListAsync();
        }

        public async Task<Billofmaterial> GetByBomIdAsync(decimal bomId)
        {
            return await _context.Billofmaterials
                .FirstOrDefaultAsync(b => b.Bomid == bomId) ?? throw new InvalidOperationException("Can't find Bill of Material with this ID");
        }

        public async Task<IEnumerable<Billofmaterial>> GetByComponentAsync(decimal componentId)
        {
            return await _context.Billofmaterials
                .Where(b => b.Componentid == componentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Billofmaterial>> GetByQuantityRangeAsync(decimal minQuantity, decimal maxQuantity)
        {
            return await _context.Billofmaterials
                .Where(b => b.Quantity >= minQuantity && b.Quantity <= maxQuantity)
                .ToListAsync();
        }

        public async Task<IEnumerable<Billofmaterial>> GetByScrapFactorRangeAsync(decimal minScrap, decimal maxScrap)
        {
            return await _context.Billofmaterials
                .Where(b => b.Scrapfactor >= minScrap && b.Scrapfactor <= maxScrap)
                .ToListAsync();
        }
    }
}