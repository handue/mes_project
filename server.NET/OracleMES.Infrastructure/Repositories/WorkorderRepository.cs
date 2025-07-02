using OracleMES.Core.Entities;
using OracleMES.Core.Interfaces.Repositories;
using OracleMES.Infrastructure.Data;
using OracleMES.Core.Constants;
using Microsoft.EntityFrameworkCore;

namespace OracleMES.Infrastructure.Repositories;

public class WorkorderRepository : Repository<Workorder>, IWorkorderRepository
{
    public WorkorderRepository(MesDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Workorder>> GetActiveWorkordersAsync()
    {
        return await _dbSet
            .Where(w => w.Status == WorkorderStatus.InProgress)
            .ToListAsync();
    }

    public async Task<IEnumerable<Workorder>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(w => w.Plannedstarttime != null &&
                        DateTime.Parse(w.Plannedstarttime) >= startDate &&
                        DateTime.Parse(w.Plannedstarttime) <= endDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Workorder>> GetByEmployeeAsync(decimal employeeId)
    {
        return await _dbSet
            .Where(w => w.Employeeid == employeeId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Workorder>> GetByLotNumberAsync(string lotNumber)
    {
        return await _dbSet
            .Where(w => w.Lotnumber == lotNumber)
            .ToListAsync();
    }

    public async Task<IEnumerable<Workorder>> GetByMachineAsync(decimal machineId)
    {
        return await _dbSet
            .Where(w => w.Machineid == machineId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Workorder>> GetByPriorityAsync(decimal priority)
    {
        return await _dbSet
            .Where(w => w.Priority == priority)
            .ToListAsync();
    }

    public async Task<IEnumerable<Workorder>> GetByProductAsync(decimal productId)
    {
        return await _dbSet
            .Where(w => w.Productid == productId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Workorder>> GetByStatusAsync(string status)
    {
        return await _dbSet
            .Where(w => w.Status == status)
            .ToListAsync();
    }

    public async Task<IEnumerable<Workorder>> GetByWorkcenterAsync(decimal workcenterId)
    {
        return await _dbSet
            .Where(w => w.Workcenterid == workcenterId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Workorder>> GetOverdueWorkordersAsync()
    {
        var now = DateTime.Now;
        return await _dbSet
            .Where(w => w.Status != WorkorderStatus.Completed &&
                       w.Status != WorkorderStatus.Cancelled && w.Plannedendtime != null &&
                       DateTime.Parse(w.Plannedendtime) < now)
            .ToListAsync();
    }

    public async Task<IEnumerable<Workorder>> GetTodayWorkordersAsync()
    {
        var today = DateTime.Today;

        return await _dbSet
            .Where(w => w.Plannedstarttime != null &&  DateTime.Parse(w.Plannedstarttime).Date == today)
            .ToListAsync();
    }

    public async Task UpdateActualTimesAsync(decimal orderId, string actualStartTime, string actualEndTime)
    {
        var workorder = await _dbSet
            .Where(w => w.Orderid == orderId)
            .FirstOrDefaultAsync();

        if (workorder != null)
        {
            workorder.Actualstarttime = actualStartTime;
            workorder.Actualendtime = actualEndTime;
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdatePriorityAsync(decimal orderId, decimal priority)
    {
        var workorder = await _dbSet
            .Where(w => w.Orderid == orderId)
            .FirstOrDefaultAsync();

        if (workorder != null)
        {
            workorder.Priority = priority;
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateProductAsync(decimal orderId, decimal actualProduction, decimal scrap)
    {
        var workorder = await _dbSet
            .Where(w => w.Orderid == orderId)
            .FirstOrDefaultAsync();

        if (workorder != null)
        {
            workorder.Actualproduction = actualProduction;
            workorder.Scrap = scrap;
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateStatusAsync(decimal orderId, string status)
    {
        var workorder = await _dbSet
            .Where(w => w.Orderid == orderId)
            .FirstOrDefaultAsync();

        if (workorder != null)
        {
            workorder.Status = status;
            await _context.SaveChangesAsync();
        }
    }
}