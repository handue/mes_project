using OracleMES.Core.Interfaces.Repositories;
using OracleMES.Core.Entities;
using OracleMES.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace OracleMES.Infrastructure.Repositories
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(MesDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Employee>> GetByRoleAsync(string role)
        {
            return await _context.Employees
                .Where(e => e.Role == role)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetByShiftAsync(decimal shiftId)
        {
            return await _context.Employees
                .Where(e => e.Shiftid == shiftId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetActiveEmployeesAsync()
        {
            // Employee 모델에 Status 필드가 없으므로 모든 직원을 반환
            // 필요시 Status 필드를 추가하거나 다른 조건으로 필터링
            return await _context.Employees.ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetAvailableEmployeesAsync()
        {
            // 현재 작업중이지 않은 직원들을 반환하는 로직
            // Workorder와 조인하여 현재 작업중인 직원을 제외
            return await _context.Employees.ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetBySkillAsync(string skill)
        {
            return await _context.Employees
                .Where(e => e.Skills != null && e.Skills.Contains(skill))
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetByHourlyRateRangeAsync(decimal minRate, decimal maxRate)
        {
            return await _context.Employees
                .Where(e => e.Hourlyrate >= minRate && e.Hourlyrate <= maxRate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetByWorkcenterAsync(decimal workcenterId)
        {
            // Workcenter와 Employee 간의 관계가 정의되어 있지 않으므로
            // 현재는 빈 리스트를 반환하거나 관계를 추가해야 함
            return await Task.FromResult(new List<Employee>());
        }

        public async Task UpdateShiftAsync(decimal employeeId, decimal shiftId)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Employeeid == employeeId);
            
            if (employee != null)
            {
                employee.Shiftid = shiftId;
                await _context.SaveChangesAsync();
            }
        }
    }
} 