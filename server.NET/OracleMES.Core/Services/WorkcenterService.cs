using OracleMES.Core.Entities;
using OracleMES.Core.Interfaces.Repositories;
using OracleMES.Core.Exceptions;

namespace OracleMES.Core.Services;

public class WorkcenterService
{
    private readonly IWorkcenterRepository _workcenterRepository;
    private readonly IMachineRepository _machineRepository;
    private readonly IWorkorderRepository _workorderRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public WorkcenterService(
        IWorkcenterRepository workcenterRepository,
        IMachineRepository machineRepository,
        IWorkorderRepository workorderRepository,
        IEmployeeRepository employeeRepository)
    {
        _workcenterRepository = workcenterRepository;
        _machineRepository = machineRepository;
        _workorderRepository = workorderRepository;
        _employeeRepository = employeeRepository;
    }

    // 작업장 생성
    public async Task<Workcenter> CreateWorkcenterAsync(Workcenter workcenter)
    {
        // 유효성 검증
        await ValidateWorkcenterAsync(workcenter);
        
        // 기본값 설정
        if (workcenter.Isactive == null)
            workcenter.Isactive = 1; // 활성 상태로 설정
        
        return await _workcenterRepository.AddAsync(workcenter);
    }

    // 작업장 정보 업데이트
    public async Task UpdateWorkcenterAsync(Workcenter workcenter)
    {
        var existingWorkcenter = await _workcenterRepository.GetByIdAsync(workcenter.Workcenterid);
        if (existingWorkcenter == null)
            throw new NotFoundException($"Workcenter with ID {workcenter.Workcenterid} not found");

        await ValidateWorkcenterAsync(workcenter);
        await _workcenterRepository.UpdateAsync(workcenter);
    }

    // 작업장 활성화/비활성화
    public async Task UpdateWorkcenterStatusAsync(decimal workcenterId, bool isActive)
    {
        var workcenter = await _workcenterRepository.GetByIdAsync(workcenterId);
        if (workcenter == null)
            throw new NotFoundException($"Workcenter with ID {workcenterId} not found");

        workcenter.Isactive = isActive ? 1 : 0;
        await _workcenterRepository.UpdateAsync(workcenter);
    }

    // 활성 작업장 조회
    public async Task<IEnumerable<Workcenter>> GetActiveWorkcentersAsync()
    {
        var allWorkcenters = await _workcenterRepository.GetAllAsync();
        return allWorkcenters.Where(w => w.Isactive == 1);
    }

    // 작업장별 설비 조회
    public async Task<IEnumerable<Machine>> GetMachinesByWorkcenterAsync(decimal workcenterId)
    {
        return await _machineRepository.GetByWorkcenterAsync(workcenterId);
    }

    // 작업장별 직원 조회
    public async Task<IEnumerable<Employee>> GetEmployeesByWorkcenterAsync(decimal workcenterId)
    {
        return await _employeeRepository.GetByWorkcenterAsync(workcenterId);
    }

    // 작업장별 작업지시 조회
    public async Task<IEnumerable<Workorder>> GetWorkordersByWorkcenterAsync(decimal workcenterId)
    {
        return await _workorderRepository.GetByWorkcenterAsync(workcenterId);
    }

    // 작업장 용량 분석
    public async Task<WorkcenterCapacityReport> AnalyzeWorkcenterCapacityAsync(decimal workcenterId, DateTime startDate, DateTime endDate)
    {
        var workcenter = await _workcenterRepository.GetByIdAsync(workcenterId);
        if (workcenter == null)
            throw new NotFoundException($"Workcenter with ID {workcenterId} not found");

        var machines = await _machineRepository.GetByWorkcenterAsync(workcenterId);
        var workorders = await _workorderRepository.GetByWorkcenterAsync(workcenterId);
        var periodWorkorders = workorders.Where(w => 
            DateTime.TryParse(w.Actualstarttime, out var start) && 
            start >= startDate && start <= endDate).ToList();

        var totalPlannedCapacity = workcenter.Capacity * (decimal)(endDate - startDate).TotalHours;
        var totalActualUsage = periodWorkorders.Sum(w => w.Setuptimeactual ?? 0) / 60; // 분을 시간으로 변환
        var capacityUtilization = totalPlannedCapacity > 0 ? (totalActualUsage / totalPlannedCapacity) * 100 : 0;

        var machineUtilization = new List<MachineUtilizationInfo>();
        foreach (var machine in machines)
        {
            var machineWorkorders = periodWorkorders.Where(w => w.Machineid == machine.Machineid).ToList();
            var machineUsage = machineWorkorders.Sum(w => w.Setuptimeactual ?? 0) / 60;
            var machineCapacity = machine.Nominalcapacity * (decimal)(endDate - startDate).TotalHours;
            var machineUtil = machineCapacity > 0 ? (machineUsage / machineCapacity) * 100 : 0;

            machineUtilization.Add(new MachineUtilizationInfo
            {
                MachineId = machine.Machineid,
                MachineName = machine.Name,
                PlannedCapacity = machineCapacity,
                ActualUsage = machineUsage,
                Utilization = Math.Round(machineUtil, 2)
            });
        }

        return new WorkcenterCapacityReport
        {
            WorkcenterId = workcenterId,
            WorkcenterName = workcenter.Name,
            StartDate = startDate,
            EndDate = endDate,
            TotalPlannedCapacity = totalPlannedCapacity,
            TotalActualUsage = totalActualUsage,
            CapacityUtilization = Math.Round(capacityUtilization, 2),
            MachineCount = machines.Count(),
            WorkorderCount = periodWorkorders.Count,
            MachineUtilization = machineUtilization
        };
    }

    // 작업장 생산성 분석
    public async Task<WorkcenterProductivityReport> AnalyzeWorkcenterProductivityAsync(decimal workcenterId, DateTime startDate, DateTime endDate)
    {
        var workcenter = await _workcenterRepository.GetByIdAsync(workcenterId);
        if (workcenter == null)
            throw new NotFoundException($"Workcenter with ID {workcenterId} not found");

        var workorders = await _workorderRepository.GetByWorkcenterAsync(workcenterId);
        var periodWorkorders = workorders.Where(w => 
            DateTime.TryParse(w.Actualstarttime, out var start) && 
            start >= startDate && start <= endDate).ToList();

        var totalWorkorders = periodWorkorders.Count;
        var completedWorkorders = periodWorkorders.Count(w => w.Status == "Completed");
        var totalPlannedQuantity = periodWorkorders.Sum(w => w.Quantity);
        var totalActualProduction = periodWorkorders.Sum(w => w.Actualproduction ?? 0);
        var totalScrap = periodWorkorders.Sum(w => w.Scrap ?? 0);
        var totalSetupTime = periodWorkorders.Sum(w => w.Setuptimeactual ?? 0);

        var completionRate = totalWorkorders > 0 ? (decimal)completedWorkorders / totalWorkorders * 100 : 0;
        var yieldRate = totalPlannedQuantity > 0 ? (totalActualProduction / totalPlannedQuantity) * 100 : 0;
        var scrapRate = totalActualProduction > 0 ? (totalScrap / totalActualProduction) * 100 : 0;
        var efficiency = totalSetupTime > 0 ? (totalActualProduction / totalSetupTime) * 60 : 0; // 분당 생산량

        return new WorkcenterProductivityReport
        {
            WorkcenterId = workcenterId,
            WorkcenterName = workcenter.Name,
            StartDate = startDate,
            EndDate = endDate,
            TotalWorkorders = totalWorkorders,
            CompletedWorkorders = completedWorkorders,
            TotalPlannedQuantity = totalPlannedQuantity,
            TotalActualProduction = totalActualProduction,
            TotalScrap = totalScrap,
            TotalSetupTime = totalSetupTime,
            CompletionRate = Math.Round(completionRate, 2),
            YieldRate = Math.Round(yieldRate, 2),
            ScrapRate = Math.Round(scrapRate, 2),
            Efficiency = Math.Round(efficiency, 2),
            CostPerHour = workcenter.Costperhour
        };
    }

    // 작업장별 작업 부하 분석
    public async Task<WorkcenterLoadReport> AnalyzeWorkcenterLoadAsync(DateTime startDate, DateTime endDate)
    {
        var allWorkcenters = await _workcenterRepository.GetAllAsync();
        var workcenterLoads = new List<WorkcenterLoadInfo>();

        foreach (var workcenter in allWorkcenters.Where(w => w.Isactive == 1))
        {
            var workorders = await _workorderRepository.GetByWorkcenterAsync(workcenter.Workcenterid);
            var periodWorkorders = workorders.Where(w => 
                DateTime.TryParse(w.Actualstarttime, out var start) && 
                start >= startDate && start <= endDate).ToList();

            var totalLoad = periodWorkorders.Sum(w => w.Setuptimeactual ?? 0) / 60; // 시간 단위
            var plannedCapacity = workcenter.Capacity * (decimal)(endDate - startDate).TotalHours;
            var loadPercentage = plannedCapacity > 0 ? (totalLoad / plannedCapacity) * 100 : 0;

            workcenterLoads.Add(new WorkcenterLoadInfo
            {
                WorkcenterId = workcenter.Workcenterid,
                WorkcenterName = workcenter.Name,
                PlannedCapacity = plannedCapacity,
                ActualLoad = totalLoad,
                LoadPercentage = Math.Round(loadPercentage, 2),
                WorkorderCount = periodWorkorders.Count,
                Location = workcenter.Location ?? ""
            });
        }

        return new WorkcenterLoadReport
        {
            StartDate = startDate,
            EndDate = endDate,
            WorkcenterLoads = workcenterLoads.OrderByDescending(w => w.LoadPercentage).ToList(),
            AverageLoadPercentage = workcenterLoads.Any() ? workcenterLoads.Average(w => w.LoadPercentage) : 0
        };
    }

    // 작업장 유효성 검증
    private async Task ValidateWorkcenterAsync(Workcenter workcenter)
    {
        // 필수 필드 검증
        if (string.IsNullOrEmpty(workcenter.Name))
            throw new ValidationException("Workcenter name is required");

        if (workcenter.Capacity <= 0)
            throw new ValidationException("Workcenter capacity must be greater than 0");

        if (string.IsNullOrEmpty(workcenter.Capacityuom))
            throw new ValidationException("Capacity unit of measure is required");

        if (workcenter.Costperhour < 0)
            throw new ValidationException("Cost per hour cannot be negative");

        // 용량 단위 유효성 검증
        var validUOMs = new[] { "PCS/HOUR", "KG/HOUR", "METER/HOUR", "LITER/HOUR" };
        if (!validUOMs.Contains(workcenter.Capacityuom.ToUpper()))
            throw new ValidationException($"Invalid capacity UOM: {workcenter.Capacityuom}. Valid UOMs are: {string.Join(", ", validUOMs)}");
    }
}

// 작업장 용량 분석 리포트 클래스
public class WorkcenterCapacityReport
{
    public decimal WorkcenterId { get; set; }
    public string WorkcenterName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPlannedCapacity { get; set; }
    public decimal TotalActualUsage { get; set; }
    public decimal CapacityUtilization { get; set; }
    public int MachineCount { get; set; }
    public int WorkorderCount { get; set; }
    public List<MachineUtilizationInfo> MachineUtilization { get; set; } = new();
}

public class MachineUtilizationInfo
{
    public decimal MachineId { get; set; }
    public string MachineName { get; set; } = string.Empty;
    public decimal PlannedCapacity { get; set; }
    public decimal ActualUsage { get; set; }
    public decimal Utilization { get; set; }
}

// 작업장 생산성 분석 리포트 클래스
public class WorkcenterProductivityReport
{
    public decimal WorkcenterId { get; set; }
    public string WorkcenterName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalWorkorders { get; set; }
    public int CompletedWorkorders { get; set; }
    public decimal TotalPlannedQuantity { get; set; }
    public decimal TotalActualProduction { get; set; }
    public decimal TotalScrap { get; set; }
    public decimal TotalSetupTime { get; set; }
    public decimal CompletionRate { get; set; }
    public decimal YieldRate { get; set; }
    public decimal ScrapRate { get; set; }
    public decimal Efficiency { get; set; }
    public decimal CostPerHour { get; set; }
}

// 작업장 부하 분석 리포트 클래스
public class WorkcenterLoadReport
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<WorkcenterLoadInfo> WorkcenterLoads { get; set; } = new();
    public decimal AverageLoadPercentage { get; set; }
}

public class WorkcenterLoadInfo
{
    public decimal WorkcenterId { get; set; }
    public string WorkcenterName { get; set; } = string.Empty;
    public decimal PlannedCapacity { get; set; }
    public decimal ActualLoad { get; set; }
    public decimal LoadPercentage { get; set; }
    public int WorkorderCount { get; set; }
    public string Location { get; set; } = string.Empty;
} 