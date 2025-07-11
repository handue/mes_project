using OracleMES.Core.Entities;
using OracleMES.Core.Interfaces.Repositories;
using OracleMES.Core.Exceptions;

namespace OracleMES.Core.Services;

public class DowntimeService
{
    private readonly IDowntimeRepository _downtimeRepository;
    private readonly IMachineRepository _machineRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IWorkorderRepository _workorderRepository;

    public DowntimeService(
        IDowntimeRepository downtimeRepository,
        IMachineRepository machineRepository,
        IEmployeeRepository employeeRepository,
        IWorkorderRepository workorderRepository)
    {
        _downtimeRepository = downtimeRepository;
        _machineRepository = machineRepository;
        _employeeRepository = employeeRepository;
        _workorderRepository = workorderRepository;
    }

    // 다운타임 등록
    public async Task<Downtime> RegisterDowntimeAsync(Downtime downtime)
    {
        // 유효성 검증
        await ValidateDowntimeAsync(downtime);
        
        // 기본값 설정
        if (string.IsNullOrEmpty(downtime.Starttime))
            downtime.Starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        
        // 지속 시간 계산 (종료 시간이 제공된 경우)
        if (!string.IsNullOrEmpty(downtime.Endtime))
        {
            if (DateTime.TryParse(downtime.Starttime, out var startTime) && 
                DateTime.TryParse(downtime.Endtime, out var endTime))
            {
                downtime.Duration = (decimal)(endTime - startTime).TotalMinutes;
            }
        }
        
        return await _downtimeRepository.AddAsync(downtime);
    }

    // 다운타임 종료
    public async Task EndDowntimeAsync(decimal downtimeId, string endTime = null)
    {
        var downtime = await _downtimeRepository.GetByIdAsync(downtimeId);
        if (downtime == null)
            throw new NotFoundException($"Downtime with ID {downtimeId} not found");

        if (!string.IsNullOrEmpty(downtime.Endtime))
            throw new InvalidOperationException("Downtime is already ended");

        var actualEndTime = endTime ?? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        downtime.Endtime = actualEndTime;

        // 지속 시간 계산
        if (DateTime.TryParse(downtime.Starttime, out var startTime) && 
            DateTime.TryParse(actualEndTime, out var endTimeParsed))
        {
            downtime.Duration = (decimal)(endTimeParsed - startTime).TotalMinutes;
        }

        await _downtimeRepository.UpdateAsync(downtime);

        // 설비 상태를 Available로 변경
        await UpdateMachineStatusAfterDowntimeAsync(downtime.Machineid);
    }

    // 설비별 다운타임 조회
    public async Task<IEnumerable<Downtime>> GetDowntimesByMachineAsync(decimal machineId)
    {
        return await _downtimeRepository.GetByMachineAsync(machineId);
    }

    // 날짜 범위별 다운타임 조회
    public async Task<IEnumerable<Downtime>> GetDowntimesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _downtimeRepository.GetByDateRangeAsync(startDate, endDate);
    }

    // 카테고리별 다운타임 조회
    public async Task<IEnumerable<Downtime>> GetDowntimesByCategoryAsync(string category)
    {
        return await _downtimeRepository.GetByCategoryAsync(category);
    }

    // 사유별 다운타임 조회
    public async Task<IEnumerable<Downtime>> GetDowntimesByReasonAsync(string reason)
    {
        return await _downtimeRepository.GetByReasonAsync(reason);
    }

    // 활성 다운타임 조회
    public async Task<IEnumerable<Downtime>> GetActiveDowntimesAsync()
    {
        return await _downtimeRepository.GetActiveDowntimesAsync();
    }

    // 긴 다운타임 조회
    public async Task<IEnumerable<Downtime>> GetLongDowntimesAsync(decimal minDuration)
    {
        return await _downtimeRepository.GetLongDowntimesAsync(minDuration);
    }

    // 신고자별 다운타임 조회
    public async Task<IEnumerable<Downtime>> GetDowntimesByReportedByAsync(decimal reportedBy)
    {
        return await _downtimeRepository.GetByReportedByAsync(reportedBy);
    }

    // 설비별 총 다운타임 계산
    public async Task<decimal> GetTotalDowntimeByMachineAsync(decimal machineId, DateTime date)
    {
        return await _downtimeRepository.GetTotalDowntimeAsync(machineId, date);
    }

    // 다운타임 분석 리포트 생성
    public async Task<DowntimeAnalysisReport> AnalyzeDowntimeAsync(DateTime startDate, DateTime endDate)
    {
        var downtimes = await _downtimeRepository.GetByDateRangeAsync(startDate, endDate);
        
        if (!downtimes.Any())
            return new DowntimeAnalysisReport { StartDate = startDate, EndDate = endDate };

        var totalDowntimes = downtimes.Count();
        var totalDuration = downtimes.Sum(d => d.Duration ?? 0);
        var averageDuration = totalDowntimes > 0 ? totalDuration / totalDowntimes : 0;

        // 카테고리별 분석
        var categoryAnalysis = downtimes
            .GroupBy(d => d.Category)
            .Select(g => new CategoryAnalysis
            {
                Category = g.Key,
                Count = g.Count(),
                TotalDuration = g.Sum(d => d.Duration ?? 0),
                AverageDuration = g.Average(d => d.Duration ?? 0),
                Percentage = (decimal)g.Count() / totalDowntimes * 100
            })
            .OrderByDescending(c => c.TotalDuration)
            .ToList();

        // 사유별 분석
        var reasonAnalysis = downtimes
            .GroupBy(d => d.Reason)
            .Select(g => new ReasonAnalysis
            {
                Reason = g.Key,
                Count = g.Count(),
                TotalDuration = g.Sum(d => d.Duration ?? 0),
                AverageDuration = g.Average(d => d.Duration ?? 0),
                Percentage = (decimal)g.Count() / totalDowntimes * 100
            })
            .OrderByDescending(r => r.TotalDuration)
            .ToList();

        // 설비별 분석
        var machineAnalysis = new List<MachineDowntimeAnalysis>();
        var machineGroups = downtimes.GroupBy(d => d.Machineid);
        
        foreach (var group in machineGroups)
        {
            var machine = await _machineRepository.GetByIdAsync(group.Key);
            var machineDowntimes = group.ToList();
            
            machineAnalysis.Add(new MachineDowntimeAnalysis
            {
                MachineId = group.Key,
                MachineName = machine?.Name ?? "Unknown",
                DowntimeCount = machineDowntimes.Count,
                TotalDuration = machineDowntimes.Sum(d => d.Duration ?? 0),
                AverageDuration = machineDowntimes.Average(d => d.Duration ?? 0)
            });
        }

        return new DowntimeAnalysisReport
        {
            StartDate = startDate,
            EndDate = endDate,
            TotalDowntimes = totalDowntimes,
            TotalDuration = totalDuration,
            AverageDuration = Math.Round(averageDuration, 2),
            CategoryAnalysis = categoryAnalysis,
            ReasonAnalysis = reasonAnalysis,
            MachineAnalysis = machineAnalysis.OrderByDescending(m => m.TotalDuration).ToList()
        };
    }

    // 다운타임 트렌드 분석
    public async Task<DowntimeTrendReport> AnalyzeDowntimeTrendAsync(DateTime startDate, DateTime endDate)
    {
        var downtimes = await _downtimeRepository.GetByDateRangeAsync(startDate, endDate);
        var trendData = new List<DowntimeTrendPoint>();

        // 일별 트렌드 계산
        var dailyGroups = downtimes
            .GroupBy(d => DateTime.Parse(d.Starttime ?? DateTime.Now.ToString("yyyy-MM-dd")).Date)
            .OrderBy(g => g.Key);

        foreach (var group in dailyGroups)
        {
            var dailyDowntimes = group.ToList();
            var totalDuration = dailyDowntimes.Sum(d => d.Duration ?? 0);
            var count = dailyDowntimes.Count;

            trendData.Add(new DowntimeTrendPoint
            {
                Date = group.Key,
                DowntimeCount = count,
                TotalDuration = totalDuration,
                AverageDuration = count > 0 ? totalDuration / count : 0
            });
        }

        return new DowntimeTrendReport
        {
            StartDate = startDate,
            EndDate = endDate,
            TrendData = trendData,
            OverallAverageDuration = trendData.Any() ? trendData.Average(t => t.AverageDuration) : 0,
            TotalDowntimeCount = trendData.Sum(t => t.DowntimeCount)
        };
    }

    // 다운타임 예방 정비 알림 생성
    public async Task<IEnumerable<DowntimeAlert>> GenerateDowntimeAlertsAsync()
    {
        var alerts = new List<DowntimeAlert>();
        var recentDowntimes = await _downtimeRepository.GetByDateRangeAsync(
            DateTime.Now.AddDays(-30), DateTime.Now);

        // 빈번한 다운타임 설비 알림
        var frequentDowntimeMachines = recentDowntimes
            .GroupBy(d => d.Machineid)
            .Where(g => g.Count() >= 5) // 30일 내 5회 이상 다운타임
            .Select(g => new { MachineId = g.Key, Count = g.Count() });

        foreach (var machine in frequentDowntimeMachines)
        {
            var machineInfo = await _machineRepository.GetByIdAsync(machine.MachineId);
            alerts.Add(new DowntimeAlert
            {
                MachineId = machine.MachineId,
                MachineName = machineInfo?.Name ?? "Unknown",
                AlertType = "FrequentDowntime",
                Message = $"Frequent downtime detected: {machine.Count} times in last 30 days for machine {machineInfo?.Name}",
                Severity = "Warning",
                CreatedDate = DateTime.Now
            });
        }

        // 긴 다운타임 알림
        var longDowntimes = recentDowntimes.Where(d => d.Duration > 240); // 4시간 이상
        foreach (var downtime in longDowntimes)
        {
            var machine = await _machineRepository.GetByIdAsync(downtime.Machineid);
            alerts.Add(new DowntimeAlert
            {
                MachineId = downtime.Machineid,
                MachineName = machine?.Name ?? "Unknown",
                AlertType = "LongDowntime",
                Message = $"Long downtime detected: {downtime.Duration} minutes for machine {machine?.Name}",
                Severity = "Critical",
                CreatedDate = DateTime.Now
            });
        }

        return alerts;
    }

    // 설비 상태 업데이트 (다운타임 종료 후)
    private async Task UpdateMachineStatusAfterDowntimeAsync(decimal machineId)
    {
        var activeDowntimes = await _downtimeRepository.GetActiveDowntimesAsync();
        var machineActiveDowntimes = activeDowntimes.Where(d => d.Machineid == machineId);

        // 해당 설비에 활성 다운타임이 없으면 Available로 변경
        if (!machineActiveDowntimes.Any())
        {
            var machine = await _machineRepository.GetByIdAsync(machineId);
            if (machine != null && machine.Status != "Available")
            {
                await _machineRepository.UpdateStatusAsync(machineId, "Available");
            }
        }
    }

    // 다운타임 유효성 검증
    private async Task ValidateDowntimeAsync(Downtime downtime)
    {
        // 설비 존재 확인
        var machine = await _machineRepository.GetByIdAsync(downtime.Machineid);
        if (machine == null)
            throw new ValidationException($"Machine with ID {downtime.Machineid} not found");

        // 작업지시 존재 확인 (제공된 경우)
        if (downtime.Orderid.HasValue)
        {
            var workorder = await _workorderRepository.GetByIdAsync(downtime.Orderid.Value);
            if (workorder == null)
                throw new ValidationException($"Workorder with ID {downtime.Orderid} not found");
        }

        // 신고자 존재 확인 (제공된 경우)
        if (downtime.Reportedby.HasValue)
        {
            var employee = await _employeeRepository.GetByIdAsync(downtime.Reportedby.Value);
            if (employee == null)
                throw new ValidationException($"Employee with ID {downtime.Reportedby} not found");
        }

        // 필수 필드 검증
        if (string.IsNullOrEmpty(downtime.Reason))
            throw new ValidationException("Downtime reason is required");

        if (string.IsNullOrEmpty(downtime.Category))
            throw new ValidationException("Downtime category is required");

        // 시간 유효성 검증
        if (!string.IsNullOrEmpty(downtime.Starttime) && !string.IsNullOrEmpty(downtime.Endtime))
        {
            if (DateTime.TryParse(downtime.Starttime, out var startTime) && 
                DateTime.TryParse(downtime.Endtime, out var endTime))
            {
                if (endTime <= startTime)
                    throw new ValidationException("End time must be after start time");
            }
        }

        // 카테고리 유효성 검증
        var validCategories = new[] { "Planned", "Unplanned", "Breakdown", "Maintenance", "Setup", "Power", "Other" };
        if (!validCategories.Contains(downtime.Category))
            throw new ValidationException($"Invalid category: {downtime.Category}. Valid categories are: {string.Join(", ", validCategories)}");
    }
}

// 다운타임 분석 리포트 클래스
public class DowntimeAnalysisReport
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalDowntimes { get; set; }
    public decimal TotalDuration { get; set; }
    public decimal AverageDuration { get; set; }
    public List<CategoryAnalysis> CategoryAnalysis { get; set; } = new();
    public List<ReasonAnalysis> ReasonAnalysis { get; set; } = new();
    public List<MachineDowntimeAnalysis> MachineAnalysis { get; set; } = new();
}

public class CategoryAnalysis
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalDuration { get; set; }
    public decimal AverageDuration { get; set; }
    public decimal Percentage { get; set; }
}

public class ReasonAnalysis
{
    public string Reason { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalDuration { get; set; }
    public decimal AverageDuration { get; set; }
    public decimal Percentage { get; set; }
}

public class MachineDowntimeAnalysis
{
    public decimal MachineId { get; set; }
    public string MachineName { get; set; } = string.Empty;
    public int DowntimeCount { get; set; }
    public decimal TotalDuration { get; set; }
    public decimal AverageDuration { get; set; }
}

// 다운타임 트렌드 리포트 클래스
public class DowntimeTrendReport
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<DowntimeTrendPoint> TrendData { get; set; } = new();
    public decimal OverallAverageDuration { get; set; }
    public int TotalDowntimeCount { get; set; }
}

public class DowntimeTrendPoint
{
    public DateTime Date { get; set; }
    public int DowntimeCount { get; set; }
    public decimal TotalDuration { get; set; }
    public decimal AverageDuration { get; set; }
}

// 다운타임 알림 클래스
public class DowntimeAlert
{
    public decimal MachineId { get; set; }
    public string MachineName { get; set; } = string.Empty;
    public string AlertType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
} 