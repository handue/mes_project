using Microsoft.AspNetCore.Mvc;
using OracleMES.Core.DTOs;
using OracleMES.Core.Services;
using OracleMES.Core.Entities;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace OracleMES.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MachineController : ControllerBase
    {
        private readonly MachineService _machineService;
        private readonly ILogger<MachineController> _logger;
        private readonly IMapper _mapper;

        public MachineController(MachineService machineService, ILogger<MachineController> logger, IMapper mapper)
        {
            _machineService = machineService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MachineResponseDTO>>> GetAllMachines()
        {
            _logger.LogInformation("=== GetAllMachines Called === ");

            var machines = await _machineService.GetActiveMachinesAsync();
            _logger.LogInformation($"Machine Count: {machines.Count()}");
            var response = _mapper.Map<IEnumerable<MachineResponseDTO>>(machines);

            return Ok(response);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MachineResponseDTO>> GetMachine(string id)
        {

            if (!decimal.TryParse(id, out decimal machineId))
                return BadRequest(new { message = "올바른 설비 ID를 입력해주세요." });

            // 실제로는 GetByIdAsync 메서드가 필요하지만 현재 없으므로 임시로 처리
            var machines = await _machineService.GetActiveMachinesAsync();
            var machine = machines.FirstOrDefault(m => m.Machineid == machineId);

            if (machine == null)
                return NotFound(new { message = "설비를 찾을 수 없습니다." });

            var response = _mapper.Map<MachineResponseDTO>(machine);
            return Ok(response);

        }

        [HttpPost]
        public async Task<ActionResult<MachineResponseDTO>> CreateMachine(CreateMachineDTO createDto)
        {

            var machine = _mapper.Map<Machine>(createDto);
            var createdMachine = await _machineService.CreateMachineAsync(machine);
            var response = _mapper.Map<MachineResponseDTO>(createdMachine);
            return CreatedAtAction(nameof(GetMachine), new { id = createdMachine.Machineid }, response);


        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult> UpdateMachineStatus(string id, [FromBody] string newStatus)
        {

            if (!decimal.TryParse(id, out decimal machineId))
                return BadRequest(new { message = "올바른 설비 ID를 입력해주세요." });

            await _machineService.UpdateMachineStatusAsync(machineId, newStatus);
            return Ok(new { message = "설비 상태가 업데이트되었습니다." });

        }

        [HttpPost("{id}/maintenance/complete")]
        public async Task<ActionResult> CompleteMaintenance(string id)
        {

            if (!decimal.TryParse(id, out decimal machineId))
                return BadRequest(new { message = "올바른 설비 ID를 입력해주세요." });

            await _machineService.CompleteMaintenanceAsync(machineId);
            return Ok(new { message = "설비 유지보수가 완료되었습니다." });


        }

        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<MachineResponseDTO>>> GetAvailableMachines()
        {

            var machines = await _machineService.GetAvailableMachinesAsync();
            var response = _mapper.Map<IEnumerable<MachineResponseDTO>>(machines);
            return Ok(response);


        }

        [HttpGet("maintenance-due")]
        public async Task<ActionResult<IEnumerable<MachineResponseDTO>>> GetMaintenanceDueMachines()
        {

            var machines = await _machineService.GetMaintenanceDueMachinesAsync();
            var response = _mapper.Map<IEnumerable<MachineResponseDTO>>(machines);
            return Ok(response);

        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<MachineResponseDTO>>> GetMachinesByStatus(string status)
        {

            var machines = await _machineService.GetMachinesByStatusAsync(status);
            var response = _mapper.Map<IEnumerable<MachineResponseDTO>>(machines);
            return Ok(response);


        }

        [HttpGet("workcenter/{workcenterId}")]
        public async Task<ActionResult<IEnumerable<MachineResponseDTO>>> GetMachinesByWorkcenter(string workcenterId)
        {

            if (!decimal.TryParse(workcenterId, out decimal workcenterIdDecimal))
                return BadRequest(new { message = "올바른 작업장 ID를 입력해주세요." });

            var machines = await _machineService.GetMachinesByWorkcenterAsync(workcenterIdDecimal);
            var response = _mapper.Map<IEnumerable<MachineResponseDTO>>(machines);
            return Ok(response);


        }
    }
}