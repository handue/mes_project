using OracleMES.Core.Entities;
using OracleMES.Core.Interfaces.Repositories;
using OracleMES.Core.Exceptions;

namespace OracleMES.Core.Services;

public class QualityControlService
{
    private readonly IQualitycontrolRepository _qualityControlRepository;
    private readonly IDefectRepository _defectRepository;
    private readonly IWorkorderRepository _workorderRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public QualityControlService(
        IQualitycontrolRepository qualityControlRepository,
        IDefectRepository defectRepository,
        IWorkorderRepository workorderRepository,
        IEmployeeRepository employeeRepository)
    {
        _qualityControlRepository = qualityControlRepository;
        _defectRepository = defectRepository;
        _workorderRepository = workorderRepository;
        _employeeRepository = employeeRepository;
    }

    // 품질 검사 생성
    public async Task<Qualitycontrol> CreateQualityCheckAsync(Qualitycontrol qualityCheck)
    {
        // 유효성 검증
        await ValidateQualityCheckAsync(qualityCheck);
        
        // 기본값 설정
        if (string.IsNullOrEmpty(qualityCheck.Date))
            qualityCheck.Date = DateTime.Now.ToString("yyyy-MM-dd");
        
        if (string.IsNullOrEmpty(qualityCheck.Result))
            qualityCheck.Result = "Pending";
        
        return await _qualityControlRepository.AddAsync(qualityCheck);
    }

    // 품질 검사 결과 업데이트
    public async Task UpdateQualityResultAsync(decimal checkId, string result, decimal defectRate, decimal reworkRate, decimal yieldRate)
    {
        var qualityCheck = await _qualityControlRepository.GetByIdAsync(checkId);
        if (qualityCheck == null)
            throw new AppException($"Quality check with ID {checkId} not found", ErrorCodes.NotFound);

        // 결과 유효성 검증
        ValidateQualityResult(result, defectRate, reworkRate, yieldRate);

        await _qualityControlRepository.UpdateQualityResultAsync(checkId, result, defectRate, reworkRate, yieldRate);
    }

    // 품질 검사 완료
    public async Task CompleteQualityCheckAsync(decimal checkId, string result, string comments = null)
    {
        var qualityCheck = await _qualityControlRepository.GetByIdAsync(checkId);
        if (qualityCheck == null)
            throw new AppException($"Quality check with ID {checkId} not found", ErrorCodes.NotFound);

        if (qualityCheck.Result != "Pending")
            throw new InvalidOperationException($"Quality check is already completed with result: {qualityCheck.Result}");

        // 결과별 기본값 설정
        decimal defectRate = 0, reworkRate = 0, yieldRate = 100;
        
        switch (result.ToLower())
        {
            case "pass":
                yieldRate = 100;
                break;
            case "fail":
                defectRate = 100;
                yieldRate = 0;
                break;
            case "partial":
                defectRate = 20; // 기본값, 실제로는 계산 필요
                reworkRate = 10;
                yieldRate = 70;
                break;
            default:
                throw new AppException($"Invalid quality result: {result}", ErrorCodes.ValidationError);
        }

        qualityCheck.Comments = comments;
        await _qualityControlRepository.UpdateQualityResultAsync(checkId, result, defectRate, reworkRate, yieldRate);
    }

    // 불량 등록
    public async Task<Defect> RegisterDefectAsync(Defect defect)
    {
        // 유효성 검증
        await ValidateDefectAsync(defect);
        
        return await _defectRepository.AddAsync(defect);
    }

    // 작업지시별 품질 검사 조회
    public async Task<IEnumerable<Qualitycontrol>> GetQualityChecksByWorkorderAsync(decimal orderId)
    {
        return await _qualityControlRepository.GetByWorkorderAsync(orderId);
    }

    // 날짜 범위별 품질 검사 조회
    public async Task<IEnumerable<Qualitycontrol>> GetQualityChecksByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _qualityControlRepository.GetByDateRangeAsync(startDate, endDate);
    }

    // 결과별 품질 검사 조회
    public async Task<IEnumerable<Qualitycontrol>> GetQualityChecksByResultAsync(string result)
    {
        return await _qualityControlRepository.GetByResultAsync(result);
    }

    // 검사원별 품질 검사 조회
    public async Task<IEnumerable<Qualitycontrol>> GetQualityChecksByInspectorAsync(decimal inspectorId)
    {
        return await _qualityControlRepository.GetByInspectorAsync(inspectorId);
    }

    // 불량 검사 조회
    public async Task<IEnumerable<Qualitycontrol>> GetDefectChecksAsync()
    {
        return await _qualityControlRepository.GetDefectsAsync();
    }

    // 불량률 범위별 검사 조회
    public async Task<IEnumerable<Qualitycontrol>> GetQualityChecksByDefectRateRangeAsync(decimal minRate, decimal maxRate)
    {
        return await _qualityControlRepository.GetByDefectRateRangeAsync(minRate, maxRate);
    }

    // 수율 범위별 검사 조회
    public async Task<IEnumerable<Qualitycontrol>> GetQualityChecksByYieldRateRangeAsync(decimal minYield, decimal maxYield)
    {
        return await _qualityControlRepository.GetByYieldRateRangeAsync(minYield, maxYield);
    }

    // 품질 통계 계산
    public async Task<QualityStatistics> CalculateQualityStatisticsAsync(DateTime startDate, DateTime endDate)
    {
        var qualityChecks = await _qualityControlRepository.GetByDateRangeAsync(startDate, endDate);
        
        if (!qualityChecks.Any())
            return new QualityStatistics { StartDate = startDate, EndDate = endDate };

        var totalChecks = qualityChecks.Count();
        var passChecks = qualityChecks.Count(qc => qc.Result?.ToLower() == "pass");
        var failChecks = qualityChecks.Count(qc => qc.Result?.ToLower() == "fail");
        var partialChecks = qualityChecks.Count(qc => qc.Result?.ToLower() == "partial");

        var averageDefectRate = qualityChecks.Where(qc => qc.Defectrate.HasValue).Average(qc => qc.Defectrate.Value);
        var averageReworkRate = qualityChecks.Where(qc => qc.Reworkrate.HasValue).Average(qc => qc.Reworkrate.Value);
        var averageYieldRate = qualityChecks.Where(qc => qc.Yieldrate.HasValue).Average(qc => qc.Yieldrate.Value);

        return new QualityStatistics
        {
            StartDate = startDate,
            EndDate = endDate,
            TotalChecks = totalChecks,
            PassChecks = passChecks,
            FailChecks = failChecks,
            PartialChecks = partialChecks,
            PassRate = totalChecks > 0 ? (decimal)passChecks / totalChecks * 100 : 0,
            FailRate = totalChecks > 0 ? (decimal)failChecks / totalChecks * 100 : 0,
            AverageDefectRate = averageDefectRate,
            AverageReworkRate = averageReworkRate,
            AverageYieldRate = averageYieldRate
        };
    }

    // 품질 트렌드 분석
    public async Task<QualityTrendReport> AnalyzeQualityTrendAsync(DateTime startDate, DateTime endDate)
    {
        var qualityChecks = await _qualityControlRepository.GetByDateRangeAsync(startDate, endDate);
        var trendData = new List<QualityTrendPoint>();

        // 일별 트렌드 계산
        var dailyGroups = qualityChecks
            .GroupBy(qc => DateTime.Parse(qc.Date ?? DateTime.Now.ToString("yyyy-MM-dd")).Date)
            .OrderBy(g => g.Key);

        foreach (var group in dailyGroups)
        {
            var dailyChecks = group.ToList();
            var totalChecks = dailyChecks.Count;
            var passChecks = dailyChecks.Count(qc => qc.Result?.ToLower() == "pass");
            var averageYield = dailyChecks.Where(qc => qc.Yieldrate.HasValue).Average(qc => qc.Yieldrate.Value);

            trendData.Add(new QualityTrendPoint
            {
                Date = group.Key,
                TotalChecks = totalChecks,
                PassChecks = passChecks,
                PassRate = totalChecks > 0 ? (decimal)passChecks / totalChecks * 100 : 0,
                AverageYieldRate = averageYield
            });
        }

        return new QualityTrendReport
        {
            StartDate = startDate,
            EndDate = endDate,
            TrendData = trendData,
            OverallPassRate = trendData.Any() ? trendData.Average(t => t.PassRate) : 0,
            OverallYieldRate = trendData.Any() ? trendData.Average(t => t.AverageYieldRate) : 0
        };
    }

    // 품질 알림 생성
    public async Task<IEnumerable<QualityAlert>> GenerateQualityAlertsAsync()
    {
        var alerts = new List<QualityAlert>();
        var recentChecks = await _qualityControlRepository.GetByDateRangeAsync(
            DateTime.Now.AddDays(-7), DateTime.Now);

        // 높은 불량률 알림
        var highDefectChecks = recentChecks.Where(qc => qc.Defectrate > 10);
        foreach (var check in highDefectChecks)
        {
            alerts.Add(new QualityAlert
            {
                CheckId = check.Checkid,
                OrderId = check.Orderid,
                AlertType = "HighDefectRate",
                Message = $"High defect rate detected: {check.Defectrate}% for workorder {check.Orderid}",
                Severity = "Warning",
                CreatedDate = DateTime.Now
            });
        }

        // 낮은 수율 알림
        var lowYieldChecks = recentChecks.Where(qc => qc.Yieldrate < 80);
        foreach (var check in lowYieldChecks)
        {
            alerts.Add(new QualityAlert
            {
                CheckId = check.Checkid,
                OrderId = check.Orderid,
                AlertType = "LowYieldRate",
                Message = $"Low yield rate detected: {check.Yieldrate}% for workorder {check.Orderid}",
                Severity = "Warning",
                CreatedDate = DateTime.Now
            });
        }

        return alerts;
    }

    // 품질 검사 유효성 검증
    private async Task ValidateQualityCheckAsync(Qualitycontrol qualityCheck)
    {
        // 작업지시 존재 확인
        var workorder = await _workorderRepository.GetByIdAsync(qualityCheck.Orderid);
        if (workorder == null)
            throw new AppException($"Workorder with ID {qualityCheck.Orderid} not found", ErrorCodes.NotFound);

        // 검사원 존재 확인 (제공된 경우)
        if (qualityCheck.Inspectorid.HasValue)
        {
            var inspector = await _employeeRepository.GetByIdAsync(qualityCheck.Inspectorid.Value);
            if (inspector == null)
                throw new AppException($"Inspector with ID {qualityCheck.Inspectorid} not found", ErrorCodes.NotFound);
        }

        // 비율 값 검증
        if (qualityCheck.Defectrate.HasValue && (qualityCheck.Defectrate < 0 || qualityCheck.Defectrate > 100))
            throw new AppException("Defect rate must be between 0 and 100", ErrorCodes.ValidationError);

        if (qualityCheck.Reworkrate.HasValue && (qualityCheck.Reworkrate < 0 || qualityCheck.Reworkrate > 100))
            throw new AppException("Rework rate must be between 0 and 100", ErrorCodes.ValidationError);

        if (qualityCheck.Yieldrate.HasValue && (qualityCheck.Yieldrate < 0 || qualityCheck.Yieldrate > 100))
            throw new AppException("Yield rate must be between 0 and 100", ErrorCodes.ValidationError);
    }

    // 불량 유효성 검증
    private async Task ValidateDefectAsync(Defect defect)
    {
        // 품질 검사 존재 확인
        var qualityCheck = await _qualityControlRepository.GetByIdAsync(defect.Checkid);
        if (qualityCheck == null)
            throw new AppException($"Quality check with ID {defect.Checkid} not found", ErrorCodes.NotFound);

        // 필수 필드 검증
        if (string.IsNullOrEmpty(defect.Defecttype))
            throw new AppException("Defect type is required", ErrorCodes.ValidationError);

        if (defect.Severity.HasValue && (defect.Severity < 1 || defect.Severity > 5))
            throw new AppException("Severity must be between 1 and 5", ErrorCodes.ValidationError);

        if (defect.Quantity.HasValue && defect.Quantity < 0)
            throw new AppException("Defect quantity cannot be negative", ErrorCodes.ValidationError);
    }

    // 품질 결과 유효성 검증
    private void ValidateQualityResult(string result, decimal defectRate, decimal reworkRate, decimal yieldRate)
    {
        if (defectRate < 0 || defectRate > 100)
            throw new AppException("Defect rate must be between 0 and 100", ErrorCodes.ValidationError);

        if (reworkRate < 0 || reworkRate > 100)
            throw new AppException("Rework rate must be between 0 and 100", ErrorCodes.ValidationError);

        if (yieldRate < 0 || yieldRate > 100)
            throw new AppException("Yield rate must be between 0 and 100", ErrorCodes.ValidationError);

        // 비율 합계 검증
        if (defectRate + reworkRate + yieldRate > 100)
            throw new AppException("Sum of defect rate, rework rate, and yield rate cannot exceed 100%", ErrorCodes.ValidationError);
    }
}

// 품질 통계 클래스
public class QualityStatistics
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalChecks { get; set; }
    public int PassChecks { get; set; }
    public int FailChecks { get; set; }
    public int PartialChecks { get; set; }
    public decimal PassRate { get; set; }
    public decimal FailRate { get; set; }
    public decimal AverageDefectRate { get; set; }
    public decimal AverageReworkRate { get; set; }
    public decimal AverageYieldRate { get; set; }
}

// 품질 트렌드 리포트 클래스
public class QualityTrendReport
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<QualityTrendPoint> TrendData { get; set; } = new();
    public decimal OverallPassRate { get; set; }
    public decimal OverallYieldRate { get; set; }
}

public class QualityTrendPoint
{
    public DateTime Date { get; set; }
    public int TotalChecks { get; set; }
    public int PassChecks { get; set; }
    public decimal PassRate { get; set; }
    public decimal AverageYieldRate { get; set; }
}

// 품질 알림 클래스
public class QualityAlert
{
    public decimal CheckId { get; set; }
    public decimal OrderId { get; set; }
    public string AlertType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
} 