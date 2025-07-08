using OracleMES.Core.Interfaces.Repositories;
using OracleMES.Core.Entities;
using OracleMES.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace OracleMES.Infrastructure.Repositories
{
    public class MaterialconsumptionRepository : Repository<Materialconsumption>, IMaterialconsumptionRepository
    {
        public MaterialconsumptionRepository(MesDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Materialconsumption>> GetByWorkorderAsync(decimal orderId)
        {
            return await _context.Materialconsumptions
                .Where(m => m.Orderid == orderId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Materialconsumption>> GetByItemAsync(decimal itemId)
        {
            return await _context.Materialconsumptions
                .Where(m => m.Itemid == itemId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Materialconsumption>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var allConsumptions = await _context.Materialconsumptions.ToListAsync();
            return allConsumptions.Where(m => m.Consumptiondate != null && 
                           DateTime.TryParse(m.Consumptiondate, out var consumptionDate) &&
                           consumptionDate >= startDate && consumptionDate <= endDate).ToList();
        }

        public async Task<IEnumerable<Materialconsumption>> GetByVarianceRangeAsync(decimal minVariance, decimal maxVariance)
        {
            return await _context.Materialconsumptions
                .Where(m => m.Variancepercent >= minVariance && m.Variancepercent <= maxVariance)
                .ToListAsync();
        }

        public async Task<IEnumerable<Materialconsumption>> GetHighVarianceAsync(decimal threshold = 10)
        {
            return await _context.Materialconsumptions
                .Where(m => m.Variancepercent > threshold)
                .ToListAsync();
        }

        public async Task UpdateActualQuantityAsync(decimal consumptionId, decimal actualQuantity)
        {
            var consumption = await _context.Materialconsumptions
                .FirstOrDefaultAsync(m => m.Consumptionid == consumptionId);
            
            if (consumption != null)
            {
                consumption.Actualquantity = actualQuantity;
                
                // Variance 계산
                if (consumption.Plannedquantity > 0)
                {
                    consumption.Variancepercent = ((actualQuantity - consumption.Plannedquantity) / consumption.Plannedquantity) * 100;
                }
                
                await _context.SaveChangesAsync();
            }
        }
    }
} 