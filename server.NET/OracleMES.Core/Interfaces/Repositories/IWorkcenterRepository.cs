using OracleMES.Core.Entities;

namespace OracleMES.Core.Interfaces.Repositories;

public interface IWorkcenterRepository : IRepository<Workcenter>
{
    Task<IEnumerable<Workcenter>> GetActiveWorkcentersAsync(); 
    Task<IEnumerable<Workcenter>> GetByLocationAsync(string location); 
    Task<IEnumerable<Workcenter>> GetByCapacityRangeAsync(decimal minCapacity, decimal maxCapacity); 

    Task<IEnumerable<Workcenter>> GetByCostRangeAsync(decimal minCost, decimal maxCost); 
    Task UpdateCapacityAsync(decimal workcenterId, decimal capacity); 
    Task UpdateCostAsync(decimal workcenterId, decimal costPerHour); 

}