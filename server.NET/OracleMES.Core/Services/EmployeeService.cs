// using OracleMES.Core.Entities;
// using OracleMES.Core.Interfaces.Repositories;
// using OracleMES.Core.Exceptions;

// namespace OracleMES.Core.Services;

// public class EmployeeService
// {
//     private readonly IEmployeeRepository _employeeRepository;
//     private readonly IShiftRepository _shiftRepository;
//     private readonly IWorkorderRepository _workorderRepository;

//     public EmployeeService(
//         IEmployeeRepository employeeRepository,
//         IShiftRepository shiftRepository,
//         IWorkorderRepository workorderRepository)
//     {
//         _employeeRepository = employeeRepository;
//         _shiftRepository = shiftRepository;
//         _workorderRepository = workorderRepository;
//     }

//     // 직원 생성
//     public async Task<Employee> CreateEmployeeAsync(Employee employee)
//     {
//         // 유효성 검증
//         await ValidateEmployeeAsync(employee);
        
//         // 기본값 설정
//         if (string.IsNullOrEmpty(employee.Hiredate))
//             employee.Hiredate = DateTime.Now.ToString("yyyy-MM-dd");
        
//         return await _employeeRepository.AddAsync(employee);
//     }

//     // 직원 정보 업데이트
//     public async Task UpdateEmployeeAsync(Employee employee)
//     {
//         var existingEmployee = await _employeeRepository.GetByIdAsync(employee.Employeeid);
//         if (existingEmployee == null)
//             throw new NotFoundException($"Employee with ID {employee.Employeeid} not found");

//         await ValidateEmployeeAsync(employee);
//         await _employeeRepository.UpdateAsync(employee);
//     }

//     // 직원 시프트 변경
//     public async Task UpdateEmployeeShiftAsync(decimal employeeId, decimal shiftId)
//     {
//         var employee = await _employeeRepository.GetByIdAsync(employeeId);
//         if (employee == null)
//             throw new NotFoundException($"Employee with ID {employeeId} not found");

//         var shift = await _shiftRepository.GetByIdAsync(shiftId);
//         if (shift == null)
//             throw new ValidationException($"Shift with ID {shiftId} not found");

//         await _employeeRepository.UpdateShiftAsync(employeeId, shiftId);
//     }

//     // 직원 스킬 추가
//     public async Task AddEmployeeSkillAsync(decimal employeeId, string skill)
//     {
//         var employee = await _employeeRepository.GetByIdAsync(employeeId);
//         if (employee == null)
//             throw new NotFoundException($"Employee with ID {employeeId} not found");

//         if (string.IsNullOrEmpty(skill))
//             throw new ValidationException("Skill cannot be empty");

//         var currentSkills = employee.Skills ?? "";
//         var skillsList = currentSkills.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();
        
//         if (!skillsList.Contains(skill))
//         {
//             skillsList.Add(skill);
//             employee.Skills = string.Join(",", skillsList);
//             await _employeeRepository.UpdateAsync(employee);
//         }
//     }

//     // 직원 스킬 제거
//     public async Task RemoveEmployeeSkillAsync(decimal employeeId, string skill)
//     {
//         var employee = await _employeeRepository.GetByIdAsync(employeeId);
//         if (employee == null)
//             throw new NotFoundException($"Employee with ID {employeeId} not found");

//         var currentSkills = employee.Skills ?? "";
//         var skillsList = currentSkills.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();
        
//         skillsList.Remove(skill);
//         employee.Skills = string.Join(",", skillsList);
//         await _employeeRepository.UpdateAsync(employee);
//     }

//     // 활성 직원 조회
//     public async Task<IEnumerable<Employee>> GetActiveEmployeesAsync()
//     {
//         return await _employeeRepository.GetActiveEmployeesAsync();
//     }

//     // 가용 직원 조회
//     public async Task<IEnumerable<Employee>> GetAvailableEmployeesAsync()
//     {
//         return await _employeeRepository.GetAvailableEmployeesAsync();
//     }

//     // 역할별 직원 조회
//     public async Task<IEnumerable<Employee>> GetEmployeesByRoleAsync(string role)
//     {
//         return await _employeeRepository.GetByRoleAsync(role);
//     }

//     // 시프트별 직원 조회
//     public async Task<IEnumerable<Employee>> GetEmployeesByShiftAsync(decimal shiftId)
//     {
//         return await _employeeRepository.GetByShiftAsync(shiftId);
//     }

//     // 스킬별 직원 조회
//     public async Task<IEnumerable<Employee>> GetEmployeesBySkillAsync(string skill)
//     {
//         return await _employeeRepository.GetBySkillAsync(skill);
//     }

//     // 시급 범위별 직원 조회
//     public async Task<IEnumerable<Employee>> GetEmployeesByHourlyRateRangeAsync(decimal minRate, decimal maxRate)
//     {
//         return await _employeeRepository.GetByHourlyRateRangeAsync(minRate, maxRate);
//     }

//     // 작업장별 직원 조회
//     public async Task<IEnumerable<Employee>> GetEmployeesByWorkcenterAsync(decimal workcenterId)
//     {
//         return await _employeeRepository.GetByWorkcenterAsync(workcenterId);
//     }

//     // 직원 작업 이력 조회
//     public async Task<IEnumerable<Workorder>> GetEmployeeWorkHistoryAsync(decimal employeeId, DateTime startDate, DateTime endDate)
//     {
//         var workorders = await _workorderRepository.GetByEmployeeAsync(employeeId);
//         return workorders.Where(w => 
//             DateTime.TryParse(w.Actualstarttime, out var start) && 
//             start >= startDate && start <= endDate);
//     }

//     // 직원 생산성 분석
//     public async Task<EmployeeProductivityReport> AnalyzeEmployeeProductivityAsync(decimal employeeId, DateTime startDate, DateTime endDate)
//     {
//         var employee = await _employeeRepository.GetByIdAsync(employeeId);
//         if (employee == null)
//             throw new NotFoundException($"Employee with ID {employeeId} not found");

//         var workorders = await _workorderRepository.GetByEmployeeAsync(employeeId);
//         var periodWorkorders = workorders.Where(w => 
//             DateTime.TryParse(w.Actualstarttime, out var start) && 
//             start >= startDate && start <= endDate).ToList();

//         var totalWorkorders = periodWorkorders.Count;
//         var completedWorkorders = periodWorkorders.Count(w => w.Status == "Completed");
//         var totalProduction = periodWorkorders.Sum(w => w.Actualproduction ?? 0);
//         var totalScrap = periodWorkorders.Sum(w => w.Scrap ?? 0);
//         var totalHours = periodWorkorders.Sum(w => w.Setuptimeactual ?? 0) / 60; // 분을 시간으로 변환

//         var productivity = totalHours > 0 ? totalProduction / totalHours : 0;
//         var completionRate = totalWorkorders > 0 ? (decimal)completedWorkorders / totalWorkorders * 100 : 0;
//         var scrapRate = totalProduction > 0 ? totalScrap / totalProduction * 100 : 0;

//         return new EmployeeProductivityReport
//         {
//             EmployeeId = employeeId,
//             EmployeeName = employee.Name,
//             StartDate = startDate,
//             EndDate = endDate,
//             TotalWorkorders = totalWorkorders,
//             CompletedWorkorders = completedWorkorders,
//             TotalProduction = totalProduction,
//             TotalScrap = totalScrap,
//             TotalHours = totalHours,
//             Productivity = Math.Round(productivity, 2),
//             CompletionRate = Math.Round(completionRate, 2),
//             ScrapRate = Math.Round(scrapRate, 2),
//             HourlyRate = employee.Hourlyrate
//         };
//     }

//     // 직원 스킬 매트릭스 생성
//     public async Task<EmployeeSkillMatrix> GenerateSkillMatrixAsync()
//     {
//         var allEmployees = await _employeeRepository.GetAllAsync();
//         var skillMatrix = new EmployeeSkillMatrix();

//         foreach (var employee in allEmployees)
//         {
//             var skills = employee.Skills?.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList() ?? new List<string>();
            
//             skillMatrix.EmployeeSkills.Add(new EmployeeSkillInfo
//             {
//                 EmployeeId = employee.Employeeid,
//                 EmployeeName = employee.Name,
//                 Role = employee.Role ?? "",
//                 Skills = skills
//             });

//             // 전체 스킬 목록 업데이트
//             foreach (var skill in skills)
//             {
//                 if (!skillMatrix.AllSkills.Contains(skill))
//                     skillMatrix.AllSkills.Add(skill);
//             }
//         }

//         return skillMatrix;
//     }

//     // 직원 유효성 검증
//     private async Task ValidateEmployeeAsync(Employee employee)
//     {
//         // 시프트 존재 확인 (제공된 경우)
//         if (employee.Shiftid.HasValue)
//         {
//             var shift = await _shiftRepository.GetByIdAsync(employee.Shiftid.Value);
//             if (shift == null)
//                 throw new ValidationException($"Shift with ID {employee.Shiftid} not found");
//         }

//         // 필수 필드 검증
//         if (string.IsNullOrEmpty(employee.Name))
//             throw new ValidationException("Employee name is required");

//         if (employee.Hourlyrate < 0)
//             throw new ValidationException("Hourly rate cannot be negative");

//         // 역할 유효성 검증
//         var validRoles = new[] { "Operator", "Supervisor", "Technician", "Manager", "Inspector" };
//         if (!string.IsNullOrEmpty(employee.Role) && !validRoles.Contains(employee.Role))
//             throw new ValidationException($"Invalid role: {employee.Role}. Valid roles are: {string.Join(", ", validRoles)}");
//     }
// }

// // 직원 생산성 리포트 클래스
// public class EmployeeProductivityReport
// {
//     public decimal EmployeeId { get; set; }
//     public string EmployeeName { get; set; } = string.Empty;
//     public DateTime StartDate { get; set; }
//     public DateTime EndDate { get; set; }
//     public int TotalWorkorders { get; set; }
//     public int CompletedWorkorders { get; set; }
//     public decimal TotalProduction { get; set; }
//     public decimal TotalScrap { get; set; }
//     public decimal TotalHours { get; set; }
//     public decimal Productivity { get; set; }
//     public decimal CompletionRate { get; set; }
//     public decimal ScrapRate { get; set; }
//     public decimal HourlyRate { get; set; }
// }

// // 직원 스킬 매트릭스 클래스
// public class EmployeeSkillMatrix
// {
//     public List<string> AllSkills { get; set; } = new();
//     public List<EmployeeSkillInfo> EmployeeSkills { get; set; } = new();
// }

// public class EmployeeSkillInfo
// {
//     public decimal EmployeeId { get; set; }
//     public string EmployeeName { get; set; } = string.Empty;
//     public string Role { get; set; } = string.Empty;
//     public List<string> Skills { get; set; } = new();
// } 