using Microsoft.AspNetCore.Mvc;
using OracleMES.Core.DTOs;
using OracleMES.Core.Services;
using OracleMES.Core.Entities;

namespace OracleMES.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MachineController : ControllerBase
    {
        private readonly MachineService _machineService;

        public MachineController(MachineService machineService)
        {
            _machineService = machineService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MachineResponseDTO>>> GetAllMachines()
        {
            Console.WriteLine("GetAllMachines 호출");
            var machines = await _machineService.GetActiveMachinesAsync();
            var response = machines.Select(m => new MachineResponseDTO
            {
                MachineId = m.Machineid.ToString(),
                Name = m.Name,
                Type = m.Type,
                WorkcenterId = m.Workcenterid?.ToString(),
                Status = m.Status,
                NominalCapacity = m.Nominalcapacity,
                CapacityUOM = m.Capacityuom,
                SetupTime = m.Setuptime,
                EfficiencyFactor = m.Efficiencyfactor,
                MaintenanceFrequency = m.Maintenancefrequency,
                LastMaintenanceDate = m.Lastmaintenancedate,
                NextMaintenanceDate = m.Nextmaintenancedate,
                ProductChangeoverTime = m.Productchangeovertime,
                CostPerHour = m.Costperhour,
                InstallationDate = m.Installationdate,
                ModelNumber = m.Modelnumber
            });

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

            var response = new MachineResponseDTO
            {
                MachineId = machine.Machineid.ToString(),
                Name = machine.Name,
                Type = machine.Type,
                WorkcenterId = machine.Workcenterid?.ToString(),
                Status = machine.Status,
                NominalCapacity = machine.Nominalcapacity,
                CapacityUOM = machine.Capacityuom,
                SetupTime = machine.Setuptime,
                EfficiencyFactor = machine.Efficiencyfactor,
                MaintenanceFrequency = machine.Maintenancefrequency,
                LastMaintenanceDate = machine.Lastmaintenancedate,
                NextMaintenanceDate = machine.Nextmaintenancedate,
                ProductChangeoverTime = machine.Productchangeovertime,
                CostPerHour = machine.Costperhour,
                InstallationDate = machine.Installationdate,
                ModelNumber = machine.Modelnumber
            };
            return Ok(response);

        }

        [HttpPost]
        public async Task<ActionResult<MachineResponseDTO>> CreateMachine(CreateMachineDTO createDto)
        {

            var machine = new Machine
            {
                Machineid = decimal.Parse(createDto.MachineId),
                Name = createDto.Name,
                Type = createDto.Type,
                Workcenterid = !string.IsNullOrEmpty(createDto.WorkcenterId) ? decimal.Parse(createDto.WorkcenterId) : null,
                Status = createDto.Status ?? "Available",
                Nominalcapacity = createDto.NominalCapacity,
                Capacityuom = createDto.CapacityUOM,
                Setuptime = createDto.SetupTime,
                Efficiencyfactor = createDto.EfficiencyFactor,
                Maintenancefrequency = createDto.MaintenanceFrequency,
                Productchangeovertime = createDto.ProductChangeoverTime,
                Costperhour = createDto.CostPerHour,
                Installationdate = createDto.InstallationDate,
                Modelnumber = createDto.ModelNumber
            };

            var createdMachine = await _machineService.CreateMachineAsync(machine);

            var response = new MachineResponseDTO
            {
                MachineId = createdMachine.Machineid.ToString(),
                Name = createdMachine.Name,
                Type = createdMachine.Type,
                WorkcenterId = createdMachine.Workcenterid?.ToString(),
                Status = createdMachine.Status,
                NominalCapacity = createdMachine.Nominalcapacity,
                CapacityUOM = createdMachine.Capacityuom,
                SetupTime = createdMachine.Setuptime,
                EfficiencyFactor = createdMachine.Efficiencyfactor,
                MaintenanceFrequency = createdMachine.Maintenancefrequency,
                LastMaintenanceDate = createdMachine.Lastmaintenancedate,
                NextMaintenanceDate = createdMachine.Nextmaintenancedate,
                ProductChangeoverTime = createdMachine.Productchangeovertime,
                CostPerHour = createdMachine.Costperhour,
                InstallationDate = createdMachine.Installationdate,
                ModelNumber = createdMachine.Modelnumber
            };
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
            var response = machines.Select(m => new MachineResponseDTO
            {
                MachineId = m.Machineid.ToString(),
                Name = m.Name,
                Type = m.Type,
                WorkcenterId = m.Workcenterid?.ToString(),
                Status = m.Status,
                NominalCapacity = m.Nominalcapacity,
                CapacityUOM = m.Capacityuom,
                SetupTime = m.Setuptime,
                EfficiencyFactor = m.Efficiencyfactor,
                MaintenanceFrequency = m.Maintenancefrequency,
                LastMaintenanceDate = m.Lastmaintenancedate,
                NextMaintenanceDate = m.Nextmaintenancedate,
                ProductChangeoverTime = m.Productchangeovertime,
                CostPerHour = m.Costperhour,
                InstallationDate = m.Installationdate,
                ModelNumber = m.Modelnumber
            });
            return Ok(response);


        }

        [HttpGet("maintenance-due")]
        public async Task<ActionResult<IEnumerable<MachineResponseDTO>>> GetMaintenanceDueMachines()
        {

            var machines = await _machineService.GetMaintenanceDueMachinesAsync();
            var response = machines.Select(m => new MachineResponseDTO
            {
                MachineId = m.Machineid.ToString(),
                Name = m.Name,
                Type = m.Type,
                WorkcenterId = m.Workcenterid?.ToString(),
                Status = m.Status,
                NominalCapacity = m.Nominalcapacity,
                CapacityUOM = m.Capacityuom,
                SetupTime = m.Setuptime,
                EfficiencyFactor = m.Efficiencyfactor,
                MaintenanceFrequency = m.Maintenancefrequency,
                LastMaintenanceDate = m.Lastmaintenancedate,
                NextMaintenanceDate = m.Nextmaintenancedate,
                ProductChangeoverTime = m.Productchangeovertime,
                CostPerHour = m.Costperhour,
                InstallationDate = m.Installationdate,
                ModelNumber = m.Modelnumber
            });
            return Ok(response);

        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<MachineResponseDTO>>> GetMachinesByStatus(string status)
        {

            var machines = await _machineService.GetMachinesByStatusAsync(status);
            var response = machines.Select(m => new MachineResponseDTO
            {
                MachineId = m.Machineid.ToString(),
                Name = m.Name,
                Type = m.Type,
                WorkcenterId = m.Workcenterid?.ToString(),
                Status = m.Status,
                NominalCapacity = m.Nominalcapacity,
                CapacityUOM = m.Capacityuom,
                SetupTime = m.Setuptime,
                EfficiencyFactor = m.Efficiencyfactor,
                MaintenanceFrequency = m.Maintenancefrequency,
                LastMaintenanceDate = m.Lastmaintenancedate,
                NextMaintenanceDate = m.Nextmaintenancedate,
                ProductChangeoverTime = m.Productchangeovertime,
                CostPerHour = m.Costperhour,
                InstallationDate = m.Installationdate,
                ModelNumber = m.Modelnumber
            });
            return Ok(response);


        }

        [HttpGet("workcenter/{workcenterId}")]
        public async Task<ActionResult<IEnumerable<MachineResponseDTO>>> GetMachinesByWorkcenter(string workcenterId)
        {

            if (!decimal.TryParse(workcenterId, out decimal workcenterIdDecimal))
                return BadRequest(new { message = "올바른 작업장 ID를 입력해주세요." });

            var machines = await _machineService.GetMachinesByWorkcenterAsync(workcenterIdDecimal);
            var response = machines.Select(m => new MachineResponseDTO
            {
                MachineId = m.Machineid.ToString(),
                Name = m.Name,
                Type = m.Type,
                WorkcenterId = m.Workcenterid?.ToString(),
                Status = m.Status,
                NominalCapacity = m.Nominalcapacity,
                CapacityUOM = m.Capacityuom,
                SetupTime = m.Setuptime,
                EfficiencyFactor = m.Efficiencyfactor,
                MaintenanceFrequency = m.Maintenancefrequency,
                LastMaintenanceDate = m.Lastmaintenancedate,
                NextMaintenanceDate = m.Nextmaintenancedate,
                ProductChangeoverTime = m.Productchangeovertime,
                CostPerHour = m.Costperhour,
                InstallationDate = m.Installationdate,
                ModelNumber = m.Modelnumber
            });
            return Ok(response);


        }
    }
}