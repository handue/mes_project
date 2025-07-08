using OracleMES.Core.Interfaces.Repositories;
using OracleMES.Core.Entities;
using OracleMES.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace OracleMES.Infrastructure.Repositories
{
    public class WorkcenterRepository : Repository<Workcenter>, IWorkcenterRepository
    {
        public WorkcenterRepository(MesDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Workcenter>> GetActiveWorkcentersAsync()
        {
            return await _context.Workcenters
                .Where(w => w.Isactive == 1)
                .ToListAsync();
        }

        public async Task<IEnumerable<Workcenter>> GetByLocationAsync(string location)
        {
            return await _context.Workcenters
                .Where(w => w.Location == location)
                .ToListAsync();
        }

        public async Task<IEnumerable<Workcenter>> GetByCapacityRangeAsync(decimal minCapacity, decimal maxCapacity)
        {
            return await _context.Workcenters
                .Where(w => w.Capacity >= minCapacity && w.Capacity <= maxCapacity)
                .ToListAsync();
        }

        public async Task<IEnumerable<Workcenter>> GetByCostRangeAsync(decimal minCost, decimal maxCost)
        {
            return await _context.Workcenters
                .Where(w => w.Costperhour >= minCost && w.Costperhour <= maxCost)
                .ToListAsync();
        }

        public async Task UpdateCapacityAsync(decimal workcenterId, decimal capacity)
        {
            var workcenter = await _context.Workcenters
                .FirstOrDefaultAsync(w => w.Workcenterid == workcenterId);
            
            if (workcenter != null)
            {
                workcenter.Capacity = capacity;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateCostAsync(decimal workcenterId, decimal costPerHour)
        {
            var workcenter = await _context.Workcenters
                .FirstOrDefaultAsync(w => w.Workcenterid == workcenterId);
            
            if (workcenter != null)
            {
                workcenter.Costperhour = costPerHour;
                await _context.SaveChangesAsync();
            }
        }
    }
} 