using OracleMES.Core.Entities;

namespace OracleMES.Core.Interfaces.Repositories;


public interface IEmployeeRepository : IRepository<Employee>
{

    Task<IEnumerable<Employee>> GetByRoleAsync(string role);
    Task<IEnumerable<Employee>> GetByShiftAsync(decimal shiftId);
    Task<IEnumerable<Employee>> GetActiveEmployeesAsync();
    Task<IEnumerable<Employee>> GetAvailableEmployeesAsync();


    Task<IEnumerable<Employee>> GetBySkillAsync(string skill);
    Task<IEnumerable<Employee>> GetByHourlyRateRangeAsync(decimal minRate, decimal maxRate);

    Task<IEnumerable<Employee>> GetByWorkcenterAsync(decimal workcenterId);
    Task UpdateShiftAsync(decimal employeeId, decimal shiftId);

}