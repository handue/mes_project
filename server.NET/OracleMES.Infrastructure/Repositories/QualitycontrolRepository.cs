using OracleMES.Core.Interfaces.Repositories;
using OracleMES.Core.Entities;
using OracleMES.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace OracleMES.Infrastructure.Repositories
{
    public class QualitycontrolRepository : Repository<Qualitycontrol>, IQualitycontrolRepository
    {
        public QualitycontrolRepository(MesDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Qualitycontrol>> GetByWorkorderAsync(decimal orderId)
        {
            return await _context.Qualitycontrols
                .Where(q => q.Orderid == orderId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Qualitycontrol>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var allQualityControls = await _context.Qualitycontrols.ToListAsync();
            return allQualityControls.Where(q => q.Date != null && 
                           DateTime.TryParse(q.Date, out var checkDate) &&
                           checkDate >= startDate && checkDate <= endDate).ToList();
        }

        public async Task<IEnumerable<Qualitycontrol>> GetByResultAsync(string result)
        {
            return await _context.Qualitycontrols
                .Where(q => q.Result == result)
                .ToListAsync();
        }

        public async Task<IEnumerable<Qualitycontrol>> GetByInspectorAsync(decimal inspectorId)
        {
            return await _context.Qualitycontrols
                .Where(q => q.Inspectorid == inspectorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Qualitycontrol>> GetDefectsAsync()
        {
            return await _context.Qualitycontrols
                .Where(q => q.Result == "failed" || q.Defectrate > 0)
                .ToListAsync();
        }

        public async Task<IEnumerable<Qualitycontrol>> GetByDefectRateRangeAsync(decimal minRate, decimal maxRate)
        {
            return await _context.Qualitycontrols
                .Where(q => q.Defectrate >= minRate && q.Defectrate <= maxRate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Qualitycontrol>> GetByYieldRateRangeAsync(decimal minYield, decimal maxYield)
        {
            return await _context.Qualitycontrols
                .Where(q => q.Yieldrate >= minYield && q.Yieldrate <= maxYield)
                .ToListAsync();
        }

        public async Task UpdateQualityResultAsync(decimal checkId, string result, decimal defectRate, decimal reworkRate, decimal yieldRate)
        {
            var qualityCheck = await _context.Qualitycontrols
                .FirstOrDefaultAsync(q => q.Checkid == checkId);
            
            if (qualityCheck != null)
            {
                qualityCheck.Result = result;
                qualityCheck.Defectrate = defectRate;
                qualityCheck.Reworkrate = reworkRate;
                qualityCheck.Yieldrate = yieldRate;
                await _context.SaveChangesAsync();
            }
        }
    }
} 