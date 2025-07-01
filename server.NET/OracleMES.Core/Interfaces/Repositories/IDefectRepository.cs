using OracleMES.Core.Entities;

namespace OracleMES.Core.Interfaces.Repositories;

public interface IDefectRepository : IRepository<Defect>
{
    Task<IEnumerable<Defect>> GetByQualityCheckAsync(decimal checkId);
    Task<IEnumerable<Defect>> GetByTypeAsync(string defectType);
    Task<IEnumerable<Defect>> GetBySeverityAsync(decimal severity);
    Task<IEnumerable<Defect>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

    Task<IEnumerable<Defect>> GetByLocationAsync(string location);
    Task<IEnumerable<Defect>> GetByRootCauseAsync(string rootCause);
    Task<IEnumerable<Defect>> GetUnresolvedDefectsAsync();

    Task UpdateDefectActionAsync(decimal defectId, string actionTaken);
}