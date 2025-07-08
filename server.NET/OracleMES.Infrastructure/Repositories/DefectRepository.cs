using OracleMES.Core.Interfaces.Repositories;
using OracleMES.Core.Entities;
using OracleMES.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace OracleMES.Infrastructure.Repositories
{
    public class DefectRepository : Repository<Defect>, IDefectRepository
    {
        public DefectRepository(MesDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Defect>> GetByQualityCheckAsync(decimal checkId)
        {
            return await _context.Defects
                .Where(d => d.Checkid == checkId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Defect>> GetByTypeAsync(string defectType)
        {
            return await _context.Defects
                .Where(d => d.Defecttype == defectType)
                .ToListAsync();
        }

        public async Task<IEnumerable<Defect>> GetBySeverityAsync(decimal severity)
        {
            return await _context.Defects
                .Where(d => d.Severity == severity)
                .ToListAsync();
        }

        public async Task<IEnumerable<Defect>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            // Defect 모델에 날짜 필드가 없으므로 Qualitycontrol과 조인하여 날짜 필터링
            // 현재는 모든 defect를 반환
            return await _context.Defects.ToListAsync();
        }

        public async Task<IEnumerable<Defect>> GetByLocationAsync(string location)
        {
            return await _context.Defects
                .Where(d => d.Location == location)
                .ToListAsync();
        }

        public async Task<IEnumerable<Defect>> GetByRootCauseAsync(string rootCause)
        {
            return await _context.Defects
                .Where(d => d.Rootcause == rootCause)
                .ToListAsync();
        }

        public async Task<IEnumerable<Defect>> GetUnresolvedDefectsAsync()
        {
            return await _context.Defects
                .Where(d => d.Actiontaken == null || d.Actiontaken == "")
                .ToListAsync();
        }

        public async Task UpdateDefectActionAsync(decimal defectId, string actionTaken)
        {
            var defect = await _context.Defects
                .FirstOrDefaultAsync(d => d.Defectid == defectId);
            
            if (defect != null)
            {
                defect.Actiontaken = actionTaken;
                await _context.SaveChangesAsync();
            }
        }
    }
} 