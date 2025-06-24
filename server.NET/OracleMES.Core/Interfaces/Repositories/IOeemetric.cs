using OracleMES.Core.Entities;

namespace OracleMES.Core.Interfaces.Repositories;

public interface IOeemetricRepository : IRepository<Oeemetric>
{
     
    Task<IEnumerable<Oeemetric>> GetByMachineAsync(decimal machineId);     Task<IEnumerable<Oeemetric>> GetByDateRangeAsync(DateTime startDate, DateTime endDate); 
    Task<IEnumerable<Oeemetric>> GetTodayMetricsAsync(); 
    
    
    Task<decimal> CalculateOEEAsync(decimal machineId, DateTime date); 
    Task<IEnumerable<Oeemetric>> GetHighOEEAsync(decimal threshold = 85); 
    Task<IEnumerable<Oeemetric>> GetLowOEEAsync(decimal threshold = 70); 
    
    Task<IEnumerable<Oeemetric>> GetByAvailabilityRangeAsync(decimal minAvailability, decimal maxAvailability);
    Task<IEnumerable<Oeemetric>> GetByPerformanceRangeAsync(decimal minPerformance, decimal maxPerformance);
    Task<IEnumerable<Oeemetric>> GetByQualityRangeAsync(decimal minQuality, decimal maxQuality);
}