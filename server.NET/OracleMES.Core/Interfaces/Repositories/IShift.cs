using OracleMES.Core.Entities;

namespace OracleMES.Core.Interfaces.Repositories;

public interface IShiftRepository : IRepository<Shift>
{
    Task<IEnumerable<Shift>> GetActiveShiftsAsync();
    Task<IEnumerable<Shift>> GetWeekendShiftsAsync();
    Task<IEnumerable<Shift>> GetByCapacityRangeAsync(decimal minCapacity, decimal maxCapacity);
    Task UpdateCapacityAsync(decimal shiftId, decimal capacity);

}