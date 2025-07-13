using Microsoft.AspNetCore.Mvc;
using OracleMES.Core.DTOs;
using OracleMES.Core.Services;  
using OracleMES.Core.Entities;

namespace OracleMES.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeService _employeeService;

        public EmployeeController(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeResponseDTO>>> GetAllEmployees()
        {
            try
            {
                var employees = await _employeeService.GetAllEmployeesAsync();
                var response = employees.Select(e => new EmployeeResponseDTO
                {
                    EmployeeId = e.Employeeid.ToString(),
                    Name = e.Name,
                    Role = e.Role,
                    ShiftId = e.Shiftid?.ToString(),
                    HourlyRate = e.Hourlyrate,
                    Skills = e.Skills,
                    HireDate = e.Hiredate
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "직원 목록을 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeResponseDTO>> GetEmployee(string id)
        {
            try
            {
                if (!decimal.TryParse(id, out decimal employeeId))
                    return BadRequest(new { message = "올바른 직원 ID를 입력해주세요." });

                // 실제로는 GetByIdAsync 메서드가 필요하지만 현재 없으므로 임시로 처리
                var employees = await _employeeService.GetAllEmployeesAsync();
                var employee = employees.FirstOrDefault(e => e.Employeeid == employeeId);
                
                if (employee == null)
                    return NotFound(new { message = "직원을 찾을 수 없습니다." });

                var response = new EmployeeResponseDTO
                {
                    EmployeeId = employee.Employeeid.ToString(),
                    Name = employee.Name,
                    Role = employee.Role,
                    ShiftId = employee.Shiftid?.ToString(),
                    HourlyRate = employee.Hourlyrate,
                    Skills = employee.Skills,
                    HireDate = employee.Hiredate
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "직원을 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeResponseDTO>> CreateEmployee(CreateEmployeeDTO createDto)
        {
            try
            {
                var employee = new Employee
                {
                    Employeeid = decimal.Parse(createDto.EmployeeId),
                    Name = createDto.Name,
                    Role = createDto.Role,
                    Shiftid = !string.IsNullOrEmpty(createDto.ShiftId) ? decimal.Parse(createDto.ShiftId) : null,
                    Hourlyrate = createDto.HourlyRate,
                    Skills = createDto.Skills,
                    Hiredate = createDto.HireDate
                };

                var createdEmployee = await _employeeService.CreateEmployeeAsync(employee);

                var response = new EmployeeResponseDTO
                {
                    EmployeeId = createdEmployee.Employeeid.ToString(),
                    Name = createdEmployee.Name,
                    Role = createdEmployee.Role,
                    ShiftId = createdEmployee.Shiftid?.ToString(),
                    HourlyRate = createdEmployee.Hourlyrate,
                    Skills = createdEmployee.Skills,
                    HireDate = createdEmployee.Hiredate
                };
                return CreatedAtAction(nameof(GetEmployee), new { id = createdEmployee.Employeeid }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "직원을 생성하는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EmployeeResponseDTO>> UpdateEmployee(string id, UpdateEmployeeDTO updateDto)
        {
            try
            {
                if (!decimal.TryParse(id, out decimal employeeId))
                    return BadRequest(new { message = "올바른 직원 ID를 입력해주세요." });

                // 실제로는 GetByIdAsync 메서드가 필요하지만 현재 없으므로 임시로 처리
                var employees = await _employeeService.GetAllEmployeesAsync();
                var existingEmployee = employees.FirstOrDefault(e => e.Employeeid == employeeId);
                
                if (existingEmployee == null)
                    return NotFound(new { message = "직원을 찾을 수 없습니다." });

                // 업데이트할 속성들만 변경
                if (!string.IsNullOrEmpty(updateDto.Name))
                    existingEmployee.Name = updateDto.Name;
                if (!string.IsNullOrEmpty(updateDto.Role))
                    existingEmployee.Role = updateDto.Role;
                if (!string.IsNullOrEmpty(updateDto.ShiftId))
                    existingEmployee.Shiftid = decimal.Parse(updateDto.ShiftId);
                if (updateDto.HourlyRate.HasValue)
                    existingEmployee.Hourlyrate = updateDto.HourlyRate.Value;
                if (!string.IsNullOrEmpty(updateDto.Skills))
                    existingEmployee.Skills = updateDto.Skills;
                if (!string.IsNullOrEmpty(updateDto.HireDate))
                    existingEmployee.Hiredate = updateDto.HireDate;

                await _employeeService.UpdateEmployeeAsync(existingEmployee);

                var response = new EmployeeResponseDTO
                {
                    EmployeeId = existingEmployee.Employeeid.ToString(),
                    Name = existingEmployee.Name,
                    Role = existingEmployee.Role,
                    ShiftId = existingEmployee.Shiftid?.ToString(),
                    HourlyRate = existingEmployee.Hourlyrate,
                    Skills = existingEmployee.Skills,
                    HireDate = existingEmployee.Hiredate
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "직원을 업데이트하는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<EmployeeResponseDTO>>> GetActiveEmployees()
        {
            try
            {
                var employees = await _employeeService.GetActiveEmployeesAsync();
                var response = employees.Select(e => new EmployeeResponseDTO
                {
                    EmployeeId = e.Employeeid.ToString(),
                    Name = e.Name,
                    Role = e.Role,
                    ShiftId = e.Shiftid?.ToString(),
                    HourlyRate = e.Hourlyrate,
                    Skills = e.Skills,
                    HireDate = e.Hiredate
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "활성 직원을 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<EmployeeResponseDTO>>> GetAvailableEmployees()
        {
            try
            {
                var employees = await _employeeService.GetAvailableEmployeesAsync();
                var response = employees.Select(e => new EmployeeResponseDTO
                {
                    EmployeeId = e.Employeeid.ToString(),
                    Name = e.Name,
                    Role = e.Role,
                    ShiftId = e.Shiftid?.ToString(),
                    HourlyRate = e.Hourlyrate,
                    Skills = e.Skills,
                    HireDate = e.Hiredate
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "가용 직원을 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpGet("role/{role}")]
        public async Task<ActionResult<IEnumerable<EmployeeResponseDTO>>> GetEmployeesByRole(string role)
        {
            try
            {
                var employees = await _employeeService.GetEmployeesByRoleAsync(role);
                var response = employees.Select(e => new EmployeeResponseDTO
                {
                    EmployeeId = e.Employeeid.ToString(),
                    Name = e.Name,
                    Role = e.Role,
                    ShiftId = e.Shiftid?.ToString(),
                    HourlyRate = e.Hourlyrate,
                    Skills = e.Skills,
                    HireDate = e.Hiredate
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "역할별 직원을 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpGet("shift/{shiftId}")]
        public async Task<ActionResult<IEnumerable<EmployeeResponseDTO>>> GetEmployeesByShift(string shiftId)
        {
            try
            {
                if (!decimal.TryParse(shiftId, out decimal shiftIdDecimal))
                    return BadRequest(new { message = "올바른 시프트 ID를 입력해주세요." });

                var employees = await _employeeService.GetEmployeesByShiftAsync(shiftIdDecimal);
                var response = employees.Select(e => new EmployeeResponseDTO
                {
                    EmployeeId = e.Employeeid.ToString(),
                    Name = e.Name,
                    Role = e.Role,
                    ShiftId = e.Shiftid?.ToString(),
                    HourlyRate = e.Hourlyrate,
                    Skills = e.Skills,
                    HireDate = e.Hiredate
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "시프트별 직원을 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpGet("skill/{skill}")]
        public async Task<ActionResult<IEnumerable<EmployeeResponseDTO>>> GetEmployeesBySkill(string skill)
        {
            try
            {
                var employees = await _employeeService.GetEmployeesBySkillAsync(skill);
                var response = employees.Select(e => new EmployeeResponseDTO
                {
                    EmployeeId = e.Employeeid.ToString(),
                    Name = e.Name,
                    Role = e.Role,
                    ShiftId = e.Shiftid?.ToString(),
                    HourlyRate = e.Hourlyrate,
                    Skills = e.Skills,
                    HireDate = e.Hiredate
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "스킬별 직원을 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }
    }
} 