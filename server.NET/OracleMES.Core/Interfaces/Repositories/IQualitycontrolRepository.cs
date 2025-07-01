using OracleMES.Core.Entities;

namespace OracleMES.Core.Interfaces.Repositories;

public interface IQualitycontrolRepository : IRepository<Qualitycontrol>
{

    Task<IEnumerable<Qualitycontrol>> GetByWorkorderAsync(decimal orderId);
    Task<IEnumerable<Qualitycontrol>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Qualitycontrol>> GetByResultAsync(string result);
    Task<IEnumerable<Qualitycontrol>> GetByInspectorAsync(decimal inspectorId);

    Task<IEnumerable<Qualitycontrol>> GetDefectsAsync();
    Task<IEnumerable<Qualitycontrol>> GetByDefectRateRangeAsync(decimal minRate, decimal maxRate);
    Task<IEnumerable<Qualitycontrol>> GetByYieldRateRangeAsync(decimal minYield, decimal maxYield);

    Task UpdateQualityResultAsync(decimal checkId, string result, decimal defectRate, decimal reworkRate, decimal yieldRate);

}