using OracleMES.Core.Entities;

namespace OracleMES.Core.Interfaces.Repositories;

public interface IMachineRepository : IRepository<Machine>
{

    Task<IEnumerable<Machine>> GetByWorkcenterAsync(decimal workcenterId);
    Task<IEnumerable<Machine>> GetByStatusAsync(string status);
    Task<IEnumerable<Machine>> GetActiveMachinesAsync(); 
    Task<IEnumerable<Machine>> GetAvailableMachinesAsync(); 
    Task<IEnumerable<Machine>> GetMaintenanceDueAsync(); 
    Task UpdateStatusAsync(decimal machineId, string status); 
    Task UpdateMaintenanceDateAsync(decimal machineId, string nextMaintenanceDate); 
    Task<IEnumerable<Machine>> GetByTypeAsync(string type); 
    Task<IEnumerable<Machine>> GetByEfficiencyRangeAsync(decimal minEfficiency, decimal maxEfficiency); 

}
