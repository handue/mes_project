using OracleMES.Core.Entities;
using OracleMES.Core.Interfaces.Repositories;
using OracleMES.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace OracleMES.Infrastructure.Repositories;

public class MachineRepository : Repository<Machine>, IMachineRepository
{
    public MachineRepository(MesDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Machine>> GetByWorkcenterAsync(decimal workcenterId)
    {
        return await _dbSet
            .Where(m => m.Workcenterid == workcenterId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Machine>> GetByStatusAsync(string status)
    {
        return await _dbSet
            .Where(m => m.Status == status)
            .ToListAsync();
    }

    public async Task<IEnumerable<Machine>> GetActiveMachinesAsync()
    {
        return await _dbSet
            .Where(m => m.Status == "RUNNING" || m.Status == "IDLE")
            .ToListAsync();
    }

    public async Task<IEnumerable<Machine>> GetAvailableMachinesAsync()
    {
        return await _dbSet
            .Where(m => m.Status == "IDLE")
            .ToListAsync();
    }

    public async Task<IEnumerable<Machine>> GetMaintenanceDueAsync()
    {
        var today = DateTime.Today;
        return await _dbSet
            .Where(m => !string.IsNullOrEmpty(m.Nextmaintenancedate) &&
                       DateTime.Parse(m.Nextmaintenancedate) <= today)
            .ToListAsync();
    }

    public async Task UpdateStatusAsync(decimal machineId, string status)
    {
        var machine = await _dbSet
            .Where(m => m.Machineid == machineId)
            .FirstOrDefaultAsync();
        
        if (machine != null)
        {
            machine.Status = status;
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateMaintenanceDateAsync(decimal machineId, string nextMaintenanceDate)
    {
        var machine = await _dbSet
            .Where(m => m.Machineid == machineId)
            .FirstOrDefaultAsync();
        
        if (machine != null)
        {
            machine.Nextmaintenancedate = nextMaintenanceDate;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Machine>> GetByTypeAsync(string type)
    {
        return await _dbSet
            .Where(m => m.Type == type)
            .ToListAsync();
    }

    public async Task<IEnumerable<Machine>> GetByEfficiencyRangeAsync(decimal minEfficiency, decimal maxEfficiency)
    {
        return await _dbSet
            .Where(m => m.Efficiencyfactor >= minEfficiency && m.Efficiencyfactor <= maxEfficiency)
            .ToListAsync();
    }
}