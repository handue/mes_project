using OracleMES.Core.Interfaces.Repositories;
using OracleMES.Core.Entities;
using OracleMES.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace OracleMES.Infrastructure.Repositories
{
    public class DowntimeRepository : Repository<Downtime>, IDowntimeRepository
    {
        public DowntimeRepository(MesDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Downtime>> GetByMachineAsync(decimal machineId)
        {
            return await _context.Downtimes
                .Where(d => d.Machineid == machineId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Downtime>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var allDowntimes = await _context.Downtimes.ToListAsync();
            return allDowntimes.Where(d => d.Starttime != null && 
                           DateTime.TryParse(d.Starttime, out var startTime) &&
                           startTime >= startDate && startTime <= endDate).ToList();
        }

        public async Task<IEnumerable<Downtime>> GetByCategoryAsync(string category)
        {
            return await _context.Downtimes
                .Where(d => d.Category == category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Downtime>> GetByReasonAsync(string reason)
        {
            return await _context.Downtimes
                .Where(d => d.Reason == reason)
                .ToListAsync();
        }

        public async Task<IEnumerable<Downtime>> GetActiveDowntimesAsync()
        {
            return await _context.Downtimes
                .Where(d => d.Endtime == null || d.Endtime == "")
                .ToListAsync();
        }

        public async Task<decimal> GetTotalDowntimeAsync(decimal machineId, DateTime date)
        {
            var allDowntimes = await _context.Downtimes.ToListAsync();
            var downtimes = allDowntimes.Where(d => d.Machineid == machineId && 
                                                   d.Starttime != null && 
                                                   DateTime.TryParse(d.Starttime, out var startTime) &&
                                                   startTime.Date == date.Date);
            
            return downtimes.Sum(d => d.Duration ?? 0);
        }

        public async Task<IEnumerable<Downtime>> GetLongDowntimesAsync(decimal minDuration)
        {
            return await _context.Downtimes
                .Where(d => d.Duration > minDuration)
                .ToListAsync();
        }

        public async Task<IEnumerable<Downtime>> GetByReportedByAsync(decimal reportedBy)
        {
            return await _context.Downtimes
                .Where(d => d.Reportedby == reportedBy)
                .ToListAsync();
        }
    }
} 