using OracleMES.Core.Entities;
using OracleMES.Core.Interfaces.Repositories;
using OracleMES.Core.Exceptions;

namespace OracleMES.Core.Services;

public class MachineService
{
    private readonly IMachineRepository _machineRepository;
    private readonly IWorkcenterRepository _workcenterRepository;
    private readonly IDowntimeRepository _downtimeRepository;

    public MachineService(
        IMachineRepository machineRepository,
        IWorkcenterRepository workcenterRepository,
        IDowntimeRepository downtimeRepository)
    {
        _machineRepository = machineRepository;
        _workcenterRepository = workcenterRepository;
        _downtimeRepository = downtimeRepository;
    }

    // 설비 생성
    public async Task<Machine> CreateMachineAsync(Machine machine)
    {
        // 유효성 검증
        await ValidateMachineAsync(machine);

        // 기본값 설정
        if (string.IsNullOrEmpty(machine.Status))
            machine.Status = "Available";

        if (string.IsNullOrEmpty(machine.Installationdate))
            machine.Installationdate = DateTime.Now.ToString("yyyy-MM-dd");

        // 다음 유지보수 날짜 계산
        machine.Nextmaintenancedate = CalculateNextMaintenanceDate(machine.Installationdate, machine.Maintenancefrequency);

        return await _machineRepository.AddAsync(machine);
    }

    // 설비 상태 변경
    public async Task UpdateMachineStatusAsync(decimal machineId, string newStatus)
    {
        var machine = await _machineRepository.GetByIdAsync(machineId);
        if (machine == null)
            throw new AppException($"Machine with ID {machineId} not found", ErrorCodes.NotFound);

        if (machine.Status == null)
        {
            throw new AppException("Machine status is required", ErrorCodes.ValidationError);
        }
        // 상태 변경 유효성 검증
        ValidateStatusTransition(machine.Status, newStatus);

        await _machineRepository.UpdateStatusAsync(machineId, newStatus);
    }

    // 설비 유지보수 완료
    public async Task CompleteMaintenanceAsync(decimal machineId)
    {
        var machine = await _machineRepository.GetByIdAsync(machineId);
        if (machine == null)
            throw new AppException($"Machine with ID {machineId} not found", ErrorCodes.NotFound);

        // 마지막 유지보수 날짜 업데이트
        var lastMaintenanceDate = DateTime.Now.ToString("yyyy-MM-dd");

        // 다음 유지보수 날짜 계산
        var nextMaintenanceDate = CalculateNextMaintenanceDate(lastMaintenanceDate, machine.Maintenancefrequency);

        await _machineRepository.UpdateMaintenanceDateAsync(machineId, nextMaintenanceDate);
        await UpdateMachineStatusAsync(machineId, "Available");
    }

    // 설비 유지보수 예정 조회
    public async Task<IEnumerable<Machine>> GetMaintenanceDueMachinesAsync()
    {
        return await _machineRepository.GetMaintenanceDueAsync();
    }

    // 가용 설비 조회
    public async Task<IEnumerable<Machine>> GetAvailableMachinesAsync()
    {
        return await _machineRepository.GetAvailableMachinesAsync();
    }

    // 활성 설비 조회
    public async Task<IEnumerable<Machine>> GetActiveMachinesAsync()
    {
        return await _machineRepository.GetActiveMachinesAsync();
    }

    // 작업장별 설비 조회
    public async Task<IEnumerable<Machine>> GetMachinesByWorkcenterAsync(decimal workcenterId)
    {
        return await _machineRepository.GetByWorkcenterAsync(workcenterId);
    }

    // 상태별 설비 조회
    public async Task<IEnumerable<Machine>> GetMachinesByStatusAsync(string status)
    {
        return await _machineRepository.GetByStatusAsync(status);
    }

    // 타입별 설비 조회
    public async Task<IEnumerable<Machine>> GetMachinesByTypeAsync(string type)
    {
        return await _machineRepository.GetByTypeAsync(type);
    }

    // 효율성 범위별 설비 조회
    public async Task<IEnumerable<Machine>> GetMachinesByEfficiencyRangeAsync(decimal minEfficiency, decimal maxEfficiency)
    {
        return await _machineRepository.GetByEfficiencyRangeAsync(minEfficiency, maxEfficiency);
    }

    // 설비 다운타임 조회
    public async Task<IEnumerable<Downtime>> GetMachineDowntimesAsync(decimal machineId, DateTime startDate, DateTime endDate)
    {
        return await _downtimeRepository.GetByMachineAsync(machineId);
    }

    // 설비 총 다운타임 계산
    public async Task<decimal> GetTotalMachineDowntimeAsync(decimal machineId, DateTime date)
    {
        return await _downtimeRepository.GetTotalDowntimeAsync(machineId, date);
    }

    // 설비 가용성 계산
    public async Task<decimal> CalculateMachineAvailabilityAsync(decimal machineId, DateTime date)
    {
        var totalDowntime = await GetTotalMachineDowntimeAsync(machineId, date);
        var totalMinutes = 24 * 60; // 하루 24시간

        return Math.Max(0, (totalMinutes - totalDowntime) / totalMinutes * 100);
    }

    // 설비 효율성 분석
    public async Task<MachineEfficiencyReport> GetMachineEfficiencyReportAsync(decimal machineId, DateTime startDate, DateTime endDate)
    {
        var machine = await _machineRepository.GetByIdAsync(machineId);
        if (machine == null)
            throw new AppException($"Machine with ID {machineId} not found", ErrorCodes.NotFound);

        var downtimes = await _downtimeRepository.GetByDateRangeAsync(startDate, endDate);
        var machineDowntimes = downtimes.Where(d => d.Machineid == machineId);

        var totalDowntime = machineDowntimes.Sum(d => d.Duration ?? 0);
        var totalMinutes = (endDate - startDate).TotalMinutes;
        var availability = Math.Max(0, (totalMinutes - (double)totalDowntime) / totalMinutes * 100);

        return new MachineEfficiencyReport
        {
            MachineId = machineId,
            MachineName = machine.Name,
            StartDate = startDate,
            EndDate = endDate,
            Availability = (decimal)availability,
            TotalDowntime = totalDowntime,
            DowntimeCount = machineDowntimes.Count(),
            EfficiencyFactor = machine.Efficiencyfactor,
            CostPerHour = machine.Costperhour
        };
    }

    // 설비 유효성 검증
    private async Task ValidateMachineAsync(Machine machine)
    {
        // 작업장 존재 확인
        if (machine.Workcenterid.HasValue)
        {
            var workcenter = await _workcenterRepository.GetByIdAsync(machine.Workcenterid.Value);
            if (workcenter == null)
                throw new AppException($"Workcenter with ID {machine.Workcenterid} not found", ErrorCodes.NotFound);
        }

        // 필수 필드 검증
        if (string.IsNullOrEmpty(machine.Name))
            throw new AppException("Machine name is required", ErrorCodes.ValidationError);

        if (machine.Nominalcapacity <= 0)
            throw new AppException("Nominal capacity must be greater than 0", ErrorCodes.ValidationError);

        if (machine.Setuptime < 0)
            throw new AppException("Setup time cannot be negative", ErrorCodes.ValidationError);

        if (machine.Efficiencyfactor < 0 || machine.Efficiencyfactor > 1)
            throw new AppException("Efficiency factor must be between 0 and 1", ErrorCodes.ValidationError);

        if (machine.Maintenancefrequency <= 0)
            throw new AppException("Maintenance frequency must be greater than 0", ErrorCodes.ValidationError);

        if (machine.Costperhour < 0)
            throw new AppException("Cost per hour cannot be negative", ErrorCodes.ValidationError);
    }

    // 상태 전환 유효성 검증
    private void ValidateStatusTransition(string currentStatus, string newStatus)
    {
        var validTransitions = new Dictionary<string, string[]>
        {
            ["Available"] = new[] { "In Use", "Maintenance", "Breakdown", "Offline" },
            ["In Use"] = new[] { "Available", "Maintenance", "Breakdown" },
            ["Maintenance"] = new[] { "Available", "Breakdown" },
            ["Breakdown"] = new[] { "Maintenance", "Available" },
            ["Offline"] = new[] { "Available" }
        };

        if (!validTransitions.ContainsKey(currentStatus) ||
            !validTransitions[currentStatus].Contains(newStatus))
        {
            throw new InvalidOperationException($"Invalid status transition from {currentStatus} to {newStatus}");
        }
    }

    // 다음 유지보수 날짜 계산
    private string CalculateNextMaintenanceDate(string lastMaintenanceDate, decimal frequencyDays)
    {
        if (DateTime.TryParse(lastMaintenanceDate, out DateTime lastDate))
        {
            var nextDate = lastDate.AddDays((double)frequencyDays);
            return nextDate.ToString("yyyy-MM-dd");
        }

        return DateTime.Now.AddDays((double)frequencyDays).ToString("yyyy-MM-dd");
    }
}

// 설비 효율성 리포트 클래스
public class MachineEfficiencyReport
{
    public decimal MachineId { get; set; }
    public string MachineName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Availability { get; set; }
    public decimal TotalDowntime { get; set; }
    public int DowntimeCount { get; set; }
    public decimal EfficiencyFactor { get; set; }
    public decimal CostPerHour { get; set; }
}