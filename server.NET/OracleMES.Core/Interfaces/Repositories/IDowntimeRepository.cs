using OracleMES.Core.Entities;

namespace OracleMES.Core.Interfaces.Repositories;

public interface IDowntimeRepository : IRepository<Downtime>
{
    Task<IEnumerable<Downtime>> GetByMachineAsync(decimal machineId); 
    Task<IEnumerable<Downtime>> GetByDateRangeAsync(DateTime startDate, DateTime endDate); 
    Task<IEnumerable<Downtime>> GetByCategoryAsync(string category); 
    Task<IEnumerable<Downtime>> GetByReasonAsync(string reason); 
    Task<IEnumerable<Downtime>> GetActiveDowntimesAsync(); 
    
    Task<decimal> GetTotalDowntimeAsync(decimal machineId, DateTime date); 
    Task<IEnumerable<Downtime>> GetLongDowntimesAsync(decimal minDuration); 
    Task<IEnumerable<Downtime>> GetByReportedByAsync(decimal reportedBy); 
}