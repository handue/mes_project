using OracleMES.Core.Entities;
using OracleMES.Core.Interfaces.Repositories;
using OracleMES.Core.Exceptions;

namespace OracleMES.Core.Services;

public class OEEService
{
    private readonly IOeemetricRepository _oeeRepository;
    private readonly IMachineRepository _machineRepository;
    private readonly IDowntimeRepository _downtimeRepository;
    private readonly IWorkorderRepository _workorderRepository;

    public OEEService(
        IOeemetricRepository oeeRepository,
        IMachineRepository machineRepository,
        IDowntimeRepository downtimeRepository,
        IWorkorderRepository workorderRepository)
    {
        _oeeRepository = oeeRepository;
        _machineRepository = machineRepository;
        _downtimeRepository = downtimeRepository;
        _workorderRepository = workorderRepository;
    }

    // OEE 메트릭 생성
    public async Task<Oeemetric> CreateOEEMetricAsync(Oeemetric oeeMetric)
    {
        // 유효성 검증
        await ValidateOEEMetricAsync(oeeMetric);
        
        // 기본값 설정
        if (string.IsNullOrEmpty(oeeMetric.Date))
            oeeMetric.Date = DateTime.Now.ToString("yyyy-MM-dd");
        
        // OEE 계산
        oeeMetric.Oee = CalculateOEE(oeeMetric.Availability ?? 0, oeeMetric.Performance ?? 0, oeeMetric.Quality ?? 0);
        
        return await _oeeRepository.AddAsync(oeeMetric);
    }

    // OEE 계산 및 업데이트
    public async Task<decimal> CalculateAndUpdateOEEAsync(decimal machineId, DateTime date)
    {
        var machine = await _machineRepository.GetByIdAsync(machineId);
        if (machine == null)
            throw new AppException($"Machine with ID {machineId} not found", ErrorCodes.NotFound);

        // 가용성 계산
        var availability = await CalculateAvailabilityAsync(machineId, date);
        
        // 성능 계산
        var performance = await CalculatePerformanceAsync(machineId, date);
        
        // 품질 계산
        var quality = await CalculateQualityAsync(machineId, date);
        
        // OEE 계산
        var oee = CalculateOEE(availability, performance, quality);

        // 기존 메트릭 업데이트 또는 새로 생성
        var existingMetric = await GetOEEMetricByMachineAndDateAsync(machineId, date);
        if (existingMetric != null)
        {
            existingMetric.Availability = availability;
            existingMetric.Performance = performance;
            existingMetric.Quality = quality;
            existingMetric.Oee = oee;
            await _oeeRepository.UpdateAsync(existingMetric);
        }
        else
        {
            var newMetric = new Oeemetric
            {
                Machineid = machineId,
                Date = date.ToString("yyyy-MM-dd"),
                Availability = availability,
                Performance = performance,
                Quality = quality,
                Oee = oee
            };
            await _oeeRepository.AddAsync(newMetric);
        }

        return oee;
    }

    // 가용성 계산
    public async Task<decimal> CalculateAvailabilityAsync(decimal machineId, DateTime date)
    {
        var totalDowntime = await _downtimeRepository.GetTotalDowntimeAsync(machineId, date);
        var totalMinutes = 24 * 60; // 하루 24시간
        
        var availability = Math.Max(0, (totalMinutes - totalDowntime) / totalMinutes * 100);
        return Math.Round(availability, 2);
    }

    // 성능 계산
    public async Task<decimal> CalculatePerformanceAsync(decimal machineId, DateTime date)
    {
        var machine = await _machineRepository.GetByIdAsync(machineId);
        if (machine == null)
            return 0;

        var workorders = await _workorderRepository.GetByMachineAsync(machineId);
        var dateWorkorders = workorders.Where(w => 
            DateTime.TryParse(w.Actualstarttime, out var startDate) && 
            startDate.Date == date.Date);

        if (!dateWorkorders.Any())
            return 0;

        var totalActualTime = dateWorkorders.Sum(w => 
        {
            if (DateTime.TryParse(w.Actualstarttime, out var start) && 
                DateTime.TryParse(w.Actualendtime, out var end))
            {
                return (decimal)(end - start).TotalMinutes;
            }
            return 0;
        });

        var totalPlannedTime = dateWorkorders.Sum(w => w.Leadtime);
        
        if (totalPlannedTime <= 0)
            return 0;

        var performance = Math.Min(100, (totalActualTime / totalPlannedTime) * 100);
        return Math.Round(performance, 2);
    }

    // 품질 계산
    public Task<decimal> CalculateQualityAsync(decimal machineId, DateTime date)
    {
        // 이 메서드는 품질 검사 데이터를 기반으로 계산
        // 실제 구현에서는 품질 검사 결과를 조회하여 계산
        // 여기서는 기본값 95% 반환
        return Task.FromResult(95.0m);
    }

    // OEE 계산 (가용성 × 성능 × 품질)
    private decimal CalculateOEE(decimal availability, decimal performance, decimal quality)
    {
        return Math.Round((availability * performance * quality) / 10000, 2);
    }

    // 설비별 OEE 메트릭 조회
    public async Task<IEnumerable<Oeemetric>> GetOEEMetricsByMachineAsync(decimal machineId)
    {
        return await _oeeRepository.GetByMachineAsync(machineId);
    }

    // 날짜 범위별 OEE 메트릭 조회
    public async Task<IEnumerable<Oeemetric>> GetOEEMetricsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _oeeRepository.GetByDateRangeAsync(startDate, endDate);
    }

    // 오늘 OEE 메트릭 조회
    public async Task<IEnumerable<Oeemetric>> GetTodayOEEMetricsAsync()
    {
        return await _oeeRepository.GetTodayMetricsAsync();
    }

    // 높은 OEE 설비 조회
    public async Task<IEnumerable<Oeemetric>> GetHighOEEMachinesAsync(decimal threshold = 85)
    {
        return await _oeeRepository.GetHighOEEAsync(threshold);
    }

    // 낮은 OEE 설비 조회
    public async Task<IEnumerable<Oeemetric>> GetLowOEEMachinesAsync(decimal threshold = 70)
    {
        return await _oeeRepository.GetLowOEEAsync(threshold);
    }

    // 가용성 범위별 OEE 조회
    public async Task<IEnumerable<Oeemetric>> GetOEEMetricsByAvailabilityRangeAsync(decimal minAvailability, decimal maxAvailability)
    {
        return await _oeeRepository.GetByAvailabilityRangeAsync(minAvailability, maxAvailability);
    }

    // 성능 범위별 OEE 조회
    public async Task<IEnumerable<Oeemetric>> GetOEEMetricsByPerformanceRangeAsync(decimal minPerformance, decimal maxPerformance)
    {
        return await _oeeRepository.GetByPerformanceRangeAsync(minPerformance, maxPerformance);
    }

    // 품질 범위별 OEE 조회
    public async Task<IEnumerable<Oeemetric>> GetOEEMetricsByQualityRangeAsync(decimal minQuality, decimal maxQuality)
    {
        return await _oeeRepository.GetByQualityRangeAsync(minQuality, maxQuality);
    }

    // 설비별 OEE 트렌드 분석
    public async Task<OEETrendReport> AnalyzeOEETrendAsync(decimal machineId, DateTime startDate, DateTime endDate)
    {
        var machine = await _machineRepository.GetByIdAsync(machineId);
        if (machine == null)
            throw new AppException($"Machine with ID {machineId} not found", ErrorCodes.NotFound);

        var oeeMetrics = await _oeeRepository.GetByDateRangeAsync(startDate, endDate);
        var machineMetrics = oeeMetrics.Where(o => o.Machineid == machineId).OrderBy(o => o.Date);

        var trendData = new List<OEETrendPoint>();
        foreach (var metric in machineMetrics)
        {
            trendData.Add(new OEETrendPoint
            {
                Date = DateTime.Parse(metric.Date ?? DateTime.Now.ToString("yyyy-MM-dd")),
                OEE = metric.Oee ?? 0,
                Availability = metric.Availability ?? 0,
                Performance = metric.Performance ?? 0,
                Quality = metric.Quality ?? 0
            });
        }

        return new OEETrendReport
        {
            MachineId = machineId,
            MachineName = machine.Name,
            StartDate = startDate,
            EndDate = endDate,
            TrendData = trendData,
            AverageOEE = trendData.Any() ? trendData.Average(t => t.OEE) : 0,
            AverageAvailability = trendData.Any() ? trendData.Average(t => t.Availability) : 0,
            AveragePerformance = trendData.Any() ? trendData.Average(t => t.Performance) : 0,
            AverageQuality = trendData.Any() ? trendData.Average(t => t.Quality) : 0
        };
    }

    // 전체 설비 OEE 분석 리포트
    public async Task<OverallOEEReport> GenerateOverallOEEReportAsync(DateTime startDate, DateTime endDate)
    {
        var allMachines = await _machineRepository.GetAllAsync();
        var oeeMetrics = await _oeeRepository.GetByDateRangeAsync(startDate, endDate);
        
        var machineReports = new List<MachineOEEReport>();

        foreach (var machine in allMachines)
        {
            var machineMetrics = oeeMetrics.Where(o => o.Machineid == machine.Machineid).ToList();
            
            if (machineMetrics.Any())
            {
                var averageOEE = machineMetrics.Average(m => m.Oee ?? 0);
                var averageAvailability = machineMetrics.Average(m => m.Availability ?? 0);
                var averagePerformance = machineMetrics.Average(m => m.Performance ?? 0);
                var averageQuality = machineMetrics.Average(m => m.Quality ?? 0);

                machineReports.Add(new MachineOEEReport
                {
                    MachineId = machine.Machineid,
                    MachineName = machine.Name,
                    AverageOEE = Math.Round(averageOEE, 2),
                    AverageAvailability = Math.Round(averageAvailability, 2),
                    AveragePerformance = Math.Round(averagePerformance, 2),
                    AverageQuality = Math.Round(averageQuality, 2),
                    MetricCount = machineMetrics.Count
                });
            }
        }

        return new OverallOEEReport
        {
            StartDate = startDate,
            EndDate = endDate,
            MachineReports = machineReports,
            OverallAverageOEE = machineReports.Any() ? machineReports.Average(r => r.AverageOEE) : 0,
            OverallAverageAvailability = machineReports.Any() ? machineReports.Average(r => r.AverageAvailability) : 0,
            OverallAveragePerformance = machineReports.Any() ? machineReports.Average(r => r.AveragePerformance) : 0,
            OverallAverageQuality = machineReports.Any() ? machineReports.Average(r => r.AverageQuality) : 0
        };
    }

    // OEE 알림 생성
    public async Task<IEnumerable<OEEAlert>> GenerateOEEAlertsAsync()
    {
        var alerts = new List<OEEAlert>();
        var todayMetrics = await _oeeRepository.GetTodayMetricsAsync();

        foreach (var metric in todayMetrics)
        {
            if (metric.Oee < 70)
            {
                var machine = await _machineRepository.GetByIdAsync(metric.Machineid);
                alerts.Add(new OEEAlert
                {
                    MachineId = metric.Machineid,
                    MachineName = machine?.Name ?? "Unknown",
                    AlertType = "LowOEE",
                    Message = $"Low OEE detected: {metric.Oee}% for machine {machine?.Name}",
                    Severity = "Warning",
                    OEEValue = metric.Oee ?? 0,
                    CreatedDate = DateTime.Now
                });
            }
        }

        return alerts;
    }

    // 특정 날짜의 설비 OEE 메트릭 조회
    private async Task<Oeemetric?> GetOEEMetricByMachineAndDateAsync(decimal machineId, DateTime date)
    {
        var metrics = await _oeeRepository.GetByMachineAsync(machineId);
        return metrics.FirstOrDefault(m => 
            DateTime.TryParse(m.Date, out var metricDate) && 
            metricDate.Date == date.Date);
    }

    // OEE 메트릭 유효성 검증
    private async Task ValidateOEEMetricAsync(Oeemetric oeeMetric)
    {
        // 설비 존재 확인
        var machine = await _machineRepository.GetByIdAsync(oeeMetric.Machineid);
        if (machine == null)
            throw new AppException($"Machine with ID {oeeMetric.Machineid} not found", ErrorCodes.NotFound);

        // 비율 값 검증
        if (oeeMetric.Availability.HasValue && (oeeMetric.Availability < 0 || oeeMetric.Availability > 100))
            throw new AppException("Availability must be between 0 and 100", ErrorCodes.ValidationError);

        if (oeeMetric.Performance.HasValue && (oeeMetric.Performance < 0 || oeeMetric.Performance > 100))
            throw new AppException("Performance must be between 0 and 100", ErrorCodes.ValidationError);

        if (oeeMetric.Quality.HasValue && (oeeMetric.Quality < 0 || oeeMetric.Quality > 100))
            throw new AppException("Quality must be between 0 and 100", ErrorCodes.ValidationError);

        if (oeeMetric.Oee.HasValue && (oeeMetric.Oee < 0 || oeeMetric.Oee > 100))
            throw new AppException("OEE must be between 0 and 100", ErrorCodes.ValidationError);
    }
}

// OEE 트렌드 리포트 클래스
public class OEETrendReport
{
    public decimal MachineId { get; set; }
    public string MachineName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<OEETrendPoint> TrendData { get; set; } = new();
    public decimal AverageOEE { get; set; }
    public decimal AverageAvailability { get; set; }
    public decimal AveragePerformance { get; set; }
    public decimal AverageQuality { get; set; }
}

public class OEETrendPoint
{
    public DateTime Date { get; set; }
    public decimal OEE { get; set; }
    public decimal Availability { get; set; }
    public decimal Performance { get; set; }
    public decimal Quality { get; set; }
}

// 전체 OEE 리포트 클래스
public class OverallOEEReport
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<MachineOEEReport> MachineReports { get; set; } = new();
    public decimal OverallAverageOEE { get; set; }
    public decimal OverallAverageAvailability { get; set; }
    public decimal OverallAveragePerformance { get; set; }
    public decimal OverallAverageQuality { get; set; }
}

public class MachineOEEReport
{
    public decimal MachineId { get; set; }
    public string MachineName { get; set; } = string.Empty;
    public decimal AverageOEE { get; set; }
    public decimal AverageAvailability { get; set; }
    public decimal AveragePerformance { get; set; }
    public decimal AverageQuality { get; set; }
    public int MetricCount { get; set; }
}

// OEE 알림 클래스
public class OEEAlert
{
    public decimal MachineId { get; set; }
    public string MachineName { get; set; } = string.Empty;
    public string AlertType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public decimal OEEValue { get; set; }
    public DateTime CreatedDate { get; set; }
} 