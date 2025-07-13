using Microsoft.AspNetCore.Mvc;
using OracleMES.Core.DTOs;
using OracleMES.Core.Services;
using OracleMES.Core.Entities;

namespace OracleMES.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkorderController : ControllerBase
    {
        private readonly WorkorderService _workorderService;

        public WorkorderController(WorkorderService workorderService)
        {
            _workorderService = workorderService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkorderResponseDTO>>> GetAllWorkorders()
        {
            try
            {
                var workorders = await _workorderService.GetActiveWorkordersAsync();
                var response = workorders.Select(w => new WorkorderResponseDTO
                {
                    OrderId = w.Orderid.ToString(),
                    ProductId = w.Productid.ToString(),
                    WorkcenterId = w.Workcenterid.ToString(),
                    MachineId = w.Machineid.ToString(),
                    EmployeeId = w.Employeeid.ToString(),
                    Quantity = (int)w.Quantity,
                    PlannedStartTime = w.Plannedstarttime,
                    PlannedEndTime = w.Plannedendtime,
                    ActualStartTime = w.Actualstarttime,
                    ActualEndTime = w.Actualendtime,
                    Status = w.Status,
                    Priority = (int)w.Priority,
                    LeadTime = (int)w.Leadtime,
                    LotNumber = w.Lotnumber,
                    ActualProduction = w.Actualproduction,
                    Scrap = w.Scrap,
                    SetupTimeActual = w.Setuptimeactual
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "작업지시 목록을 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WorkorderResponseDTO>> GetWorkorder(string id)
        {
            try
            {
                if (!decimal.TryParse(id, out decimal orderId))
                    return BadRequest(new { message = "올바른 작업지시 ID를 입력해주세요." });

                // 실제로는 GetByIdAsync 메서드가 필요하지만 현재 없으므로 임시로 처리
                var workorders = await _workorderService.GetActiveWorkordersAsync();
                var workorder = workorders.FirstOrDefault(w => w.Orderid == orderId);
                
                if (workorder == null)
                    return NotFound(new { message = "작업지시를 찾을 수 없습니다." });

                var response = new WorkorderResponseDTO
                {
                    OrderId = workorder.Orderid.ToString(),
                    ProductId = workorder.Productid.ToString(),
                    WorkcenterId = workorder.Workcenterid.ToString(),
                    MachineId = workorder.Machineid.ToString(),
                    EmployeeId = workorder.Employeeid.ToString(),
                    Quantity = (int)workorder.Quantity,
                    PlannedStartTime = workorder.Plannedstarttime,
                    PlannedEndTime = workorder.Plannedendtime,
                    ActualStartTime = workorder.Actualstarttime,
                    ActualEndTime = workorder.Actualendtime,
                    Status = workorder.Status,
                    Priority = (int)workorder.Priority,
                    LeadTime = (int)workorder.Leadtime,
                    LotNumber = workorder.Lotnumber,
                    ActualProduction = workorder.Actualproduction,
                    Scrap = workorder.Scrap,
                    SetupTimeActual = workorder.Setuptimeactual
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "작업지시를 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<WorkorderResponseDTO>> CreateWorkorder(CreateWorkorderDTO createDto)
        {
            try
            {
                var workorder = new Workorder
                {
                    Orderid = decimal.Parse(createDto.OrderId),
                    Productid = decimal.Parse(createDto.ProductId),
                    Workcenterid = decimal.Parse(createDto.WorkcenterId),
                    Machineid = decimal.Parse(createDto.MachineId),
                    Employeeid = decimal.Parse(createDto.EmployeeId),
                    Quantity = createDto.Quantity,
                    Plannedstarttime = createDto.PlannedStartTime,
                    Plannedendtime = createDto.PlannedEndTime,
                    Status = createDto.Status ?? "Planned",
                    Priority = createDto.Priority,
                    Leadtime = createDto.LeadTime,
                    Lotnumber = createDto.LotNumber
                };

                var createdWorkorder = await _workorderService.CreateWorkorderAsync(workorder);

                var response = new WorkorderResponseDTO
                {
                    OrderId = createdWorkorder.Orderid.ToString(),
                    ProductId = createdWorkorder.Productid.ToString(),
                    WorkcenterId = createdWorkorder.Workcenterid.ToString(),
                    MachineId = createdWorkorder.Machineid.ToString(),
                    EmployeeId = createdWorkorder.Employeeid.ToString(),
                    Quantity = (int)createdWorkorder.Quantity,
                    PlannedStartTime = createdWorkorder.Plannedstarttime,
                    PlannedEndTime = createdWorkorder.Plannedendtime,
                    ActualStartTime = createdWorkorder.Actualstarttime,
                    ActualEndTime = createdWorkorder.Actualendtime,
                    Status = createdWorkorder.Status,
                    Priority = (int)createdWorkorder.Priority,
                    LeadTime = (int)createdWorkorder.Leadtime,
                    LotNumber = createdWorkorder.Lotnumber,
                    ActualProduction = createdWorkorder.Actualproduction,
                    Scrap = createdWorkorder.Scrap,
                    SetupTimeActual = createdWorkorder.Setuptimeactual
                };
                return CreatedAtAction(nameof(GetWorkorder), new { id = createdWorkorder.Orderid }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "작업지시를 생성하는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult> UpdateWorkorderStatus(string id, [FromBody] string newStatus)
        {
            try
            {
                if (!decimal.TryParse(id, out decimal orderId))
                    return BadRequest(new { message = "올바른 작업지시 ID를 입력해주세요." });

                await _workorderService.UpdateWorkorderStatusAsync(orderId, newStatus);
                return Ok(new { message = "작업지시 상태가 업데이트되었습니다." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "작업지시 상태를 업데이트하는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpPost("{id}/start")]
        public async Task<ActionResult> StartWorkorder(string id)
        {
            try
            {
                if (!decimal.TryParse(id, out decimal orderId))
                    return BadRequest(new { message = "올바른 작업지시 ID를 입력해주세요." });

                await _workorderService.StartWorkorderAsync(orderId);
                return Ok(new { message = "작업지시가 시작되었습니다." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "작업지시를 시작하는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpPost("{id}/complete")]
        public async Task<ActionResult> CompleteWorkorder(string id, [FromBody] CompleteWorkorderRequest request)
        {
            try
            {
                if (!decimal.TryParse(id, out decimal orderId))
                    return BadRequest(new { message = "올바른 작업지시 ID를 입력해주세요." });

                await _workorderService.CompleteWorkorderAsync(orderId, request.ActualProduction, request.Scrap);
                return Ok(new { message = "작업지시가 완료되었습니다." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "작업지시를 완료하는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<WorkorderResponseDTO>>> GetWorkordersByStatus(string status)
        {
            try
            {
                var workorders = await _workorderService.GetWorkordersByStatusAsync(status);
                var response = workorders.Select(w => new WorkorderResponseDTO
                {
                    OrderId = w.Orderid.ToString(),
                    ProductId = w.Productid.ToString(),
                    WorkcenterId = w.Workcenterid.ToString(),
                    MachineId = w.Machineid.ToString(),
                    EmployeeId = w.Employeeid.ToString(),
                    Quantity = (int)w.Quantity,
                    PlannedStartTime = w.Plannedstarttime,
                    PlannedEndTime = w.Plannedendtime,
                    ActualStartTime = w.Actualstarttime,
                    ActualEndTime = w.Actualendtime,
                    Status = w.Status,
                    Priority = (int)w.Priority,
                    LeadTime = (int)w.Leadtime,
                    LotNumber = w.Lotnumber,
                    ActualProduction = w.Actualproduction,
                    Scrap = w.Scrap,
                    SetupTimeActual = w.Setuptimeactual
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "상태별 작업지시를 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpGet("today")]
        public async Task<ActionResult<IEnumerable<WorkorderResponseDTO>>> GetTodayWorkorders()
        {
            try
            {
                var workorders = await _workorderService.GetTodayWorkordersAsync();
                var response = workorders.Select(w => new WorkorderResponseDTO
                {
                    OrderId = w.Orderid.ToString(),
                    ProductId = w.Productid.ToString(),
                    WorkcenterId = w.Workcenterid.ToString(),
                    MachineId = w.Machineid.ToString(),
                    EmployeeId = w.Employeeid.ToString(),
                    Quantity = (int)w.Quantity,
                    PlannedStartTime = w.Plannedstarttime,
                    PlannedEndTime = w.Plannedendtime,
                    ActualStartTime = w.Actualstarttime,
                    ActualEndTime = w.Actualendtime,
                    Status = w.Status,
                    Priority = (int)w.Priority,
                    LeadTime = (int)w.Leadtime,
                    LotNumber = w.Lotnumber,
                    ActualProduction = w.Actualproduction,
                    Scrap = w.Scrap,
                    SetupTimeActual = w.Setuptimeactual
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "오늘의 작업지시를 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }
    }

    public class CompleteWorkorderRequest
    {
        public decimal ActualProduction { get; set; }
        public decimal Scrap { get; set; }
    }
} 