using OracleMES.Core.Interfaces.Repositories;
using OracleMES.Core.Entities;
using OracleMES.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace OracleMES.Infrastructure.Repositories
{
    public class SupplierRepository : Repository<Supplier>, ISupplierRepository
    {
        public SupplierRepository(MesDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Supplier>> GetByReliabilityRangeAsync(decimal minReliability, decimal maxReliability)
        {
            return await _context.Suppliers
                .Where(s => s.Reliabilityscore >= minReliability && s.Reliabilityscore <= maxReliability)
                .ToListAsync();
        }

        public async Task<IEnumerable<Supplier>> GetByLeadTimeRangeAsync(decimal minLeadTime, decimal maxLeadTime)
        {
            return await _context.Suppliers
                .Where(s => s.Leadtime >= minLeadTime && s.Leadtime <= maxLeadTime)
                .ToListAsync();
        }

        public async Task UpdateReliabilityScoreAsync(decimal supplierId, decimal reliabilityScore)
        {
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.Supplierid == supplierId);
            
            if (supplier != null)
            {
                supplier.Reliabilityscore = reliabilityScore;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Supplier>> GetHighReliabilitySuppliersAsync(decimal threshold = 80)
        {
            return await _context.Suppliers
                .Where(s => s.Reliabilityscore >= threshold)
                .ToListAsync();
        }
    }
} 