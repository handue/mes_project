using OracleMES.Core.Entities;

namespace OracleMES.Core.Interfaces.Repositories;

public interface IMaterialconsumptionRepository : IRepository<Materialconsumption>
{
    Task<IEnumerable<Materialconsumption>> GetByWorkorderAsync(decimal orderId); 
    Task<IEnumerable<Materialconsumption>> GetByItemAsync(decimal itemId); 
    Task<IEnumerable<Materialconsumption>> GetByDateRangeAsync(DateTime startDate, DateTime endDate); 

    Task<IEnumerable<Materialconsumption>> GetByVarianceRangeAsync(decimal minVariance, decimal maxVariance); 
    Task<IEnumerable<Materialconsumption>> GetHighVarianceAsync(decimal threshold = 10); 

    Task UpdateActualQuantityAsync(decimal consumptionId, decimal actualQuantity); 
}