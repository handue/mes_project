using OracleMES.Core.Entities;

namespace OracleMES.Core.Interfaces.Repositories;

public interface ISupplierRepository : IRepository<Supplier>
{
    Task<IEnumerable<Supplier>> GetByReliabilityRangeAsync(decimal minReliability, decimal maxReliability);
    Task<IEnumerable<Supplier>> GetByLeadTimeRangeAsync(decimal minLeadTime, decimal maxLeadTime);

    Task UpdateReliabilityScoreAsync(decimal supplierId, decimal reliabilityScore);
    Task<IEnumerable<Supplier>> GetHighReliabilitySuppliersAsync(decimal threshold = 80);

}