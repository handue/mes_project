using OracleMES.Core.Interfaces.Repositories;
using OracleMES.Core.Entities;
using OracleMES.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace OracleMES.Infrastructure.Repositories
{
    public class ShiftRepository : Repository<Shift>, IShiftRepository
    {
        public ShiftRepository(MesDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Shift>> GetActiveShiftsAsync()
        {
            // Shift 모델에 Isactive 필드가 없으므로 모든 시프트를 반환
            return await _context.Shifts.ToListAsync();
        }

        public async Task<IEnumerable<Shift>> GetWeekendShiftsAsync()
        {
            return await _context.Shifts
                .Where(s => s.Isweekend == 1)
                .ToListAsync();
        }

        public async Task<IEnumerable<Shift>> GetByCapacityRangeAsync(decimal minCapacity, decimal maxCapacity)
        {
            return await _context.Shifts
                .Where(s => s.Capacity >= minCapacity && s.Capacity <= maxCapacity)
                .ToListAsync();
        }

        public async Task UpdateCapacityAsync(decimal shiftId, decimal capacity)
        {
            var shift = await _context.Shifts
                .FirstOrDefaultAsync(s => s.Shiftid == shiftId);
            
            if (shift != null)
            {
                shift.Capacity = capacity;
                await _context.SaveChangesAsync();
            }
        }
    }
} 