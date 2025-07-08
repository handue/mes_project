using OracleMES.Core.Interfaces.Repositories;
using OracleMES.Core.Entities;
using OracleMES.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace OracleMES.Infrastructure.Repositories
{
    public class OeemetricRepository : Repository<Oeemetric>, IOeemetricRepository
    {
        public OeemetricRepository(MesDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Oeemetric>> GetByMachineAsync(decimal machineId)
        {
            return await _context.Oeemetrics
                .Where(o => o.Machineid == machineId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Oeemetric>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var allOeeMetrics = await _context.Oeemetrics.ToListAsync();
            return allOeeMetrics.Where(o => o.Date != null && 
                           DateTime.TryParse(o.Date, out var metricDate) &&
                           metricDate >= startDate && metricDate <= endDate).ToList();
        }

        public async Task<IEnumerable<Oeemetric>> GetTodayMetricsAsync()
        {
            var today = DateTime.Today;
            var allOeeMetrics = await _context.Oeemetrics.ToListAsync();
            return allOeeMetrics.Where(o => o.Date != null && 
                           DateTime.TryParse(o.Date, out var metricDate) &&
                           metricDate.Date == today).ToList();
        }

        public async Task<decimal> CalculateOEEAsync(decimal machineId, DateTime date)
        {
            var allOeeMetrics = await _context.Oeemetrics.ToListAsync();
            var metric = allOeeMetrics.FirstOrDefault(o => o.Machineid == machineId && 
                                                          o.Date != null && 
                                                          DateTime.TryParse(o.Date, out var metricDate) &&
                                                          metricDate.Date == date.Date);
            
            if (metric != null && metric.Oee.HasValue)
            {
                return metric.Oee.Value;
            }
            
            // OEE가 없으면 계산 (Availability * Performance * Quality)
            if (metric != null && metric.Availability.HasValue && metric.Performance.HasValue && metric.Quality.HasValue)
            {
                return metric.Availability.Value * metric.Performance.Value * metric.Quality.Value;
            }
            
            return 0;
        }

        public async Task<IEnumerable<Oeemetric>> GetHighOEEAsync(decimal threshold = 85)
        {
            return await _context.Oeemetrics
                .Where(o => o.Oee >= threshold)
                .ToListAsync();
        }

        public async Task<IEnumerable<Oeemetric>> GetLowOEEAsync(decimal threshold = 70)
        {
            return await _context.Oeemetrics
                .Where(o => o.Oee < threshold)
                .ToListAsync();
        }

        public async Task<IEnumerable<Oeemetric>> GetByAvailabilityRangeAsync(decimal minAvailability, decimal maxAvailability)
        {
            return await _context.Oeemetrics
                .Where(o => o.Availability >= minAvailability && o.Availability <= maxAvailability)
                .ToListAsync();
        }

        public async Task<IEnumerable<Oeemetric>> GetByPerformanceRangeAsync(decimal minPerformance, decimal maxPerformance)
        {
            return await _context.Oeemetrics
                .Where(o => o.Performance >= minPerformance && o.Performance <= maxPerformance)
                .ToListAsync();
        }

        public async Task<IEnumerable<Oeemetric>> GetByQualityRangeAsync(decimal minQuality, decimal maxQuality)
        {
            return await _context.Oeemetrics
                .Where(o => o.Quality >= minQuality && o.Quality <= maxQuality)
                .ToListAsync();
        }
    }
} 