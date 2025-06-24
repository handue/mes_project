using OracleMES.Core.Interfaces.Repositories;
using OracleMES.Core.Entities;

namespace OracleMES.Core.Interfaces.Repositories;

public interface IWorkOrderRepository : IRepository<Workorder>
{

    Task<IEnumerable<Workorder>> GetByStatusAsync(string status);
    Task<IEnumerable<Workorder>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Workorder>> GetByMachineAsync(decimal machineId);
    Task<IEnumerable<Workorder>> GetByEmployeeAsync(decimal employeeId);
    Task<IEnumerable<Workorder>> GetByProductAsync(decimal productId);
    Task<IEnumerable<Workorder>> GetByWorkcenterAsync(decimal workcenterId);

    Task<IEnumerable<Workorder>> GetByPriorityAsync(decimal priority);


    Task UpdateProductAsync(decimal orderId, decimal actualProduction, decimal scrap);
    Task UpdateStatusAsync(decimal orderId, string status);
    Task UpdateActualTimesAsync(decimal orderId, string actualStartTime, string actualEndTime);
    Task<IEnumerable<Workorder>> GetActiveWorkordersAsync();
    Task<IEnumerable<Workorder>> GetTodayWorkordersAsync();
    Task<IEnumerable<Workorder>> GetOverdueWorkordersAsync();
    Task UpdatePriorityAsync(decimal orderId, decimal priority);
    Task<IEnumerable<Workorder>> GetByLotNumberAsync(string lotNumber);

}
