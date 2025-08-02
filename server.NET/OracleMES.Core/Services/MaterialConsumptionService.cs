using OracleMES.Core.Entities;
using OracleMES.Core.Interfaces.Repositories;
using OracleMES.Core.Exceptions;

namespace OracleMES.Core.Services;

public class MaterialConsumptionService
{
    private readonly IMaterialconsumptionRepository _materialConsumptionRepository;
    private readonly IWorkorderRepository _workorderRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IProductRepository _productRepository;

    public MaterialConsumptionService(
        IMaterialconsumptionRepository materialConsumptionRepository,
        IWorkorderRepository workorderRepository,
        IInventoryRepository inventoryRepository,
        IProductRepository productRepository)
    {
        _materialConsumptionRepository = materialConsumptionRepository;
        _workorderRepository = workorderRepository;
        _inventoryRepository = inventoryRepository;
        _productRepository = productRepository;
    }

    // 자재 소비 등록
    public async Task<Materialconsumption> RegisterMaterialConsumptionAsync(Materialconsumption consumption)
    {
        // 유효성 검증
        await ValidateMaterialConsumptionAsync(consumption);
        
        // 기본값 설정
        if (string.IsNullOrEmpty(consumption.Consumptiondate))
            consumption.Consumptiondate = DateTime.Now.ToString("yyyy-MM-dd");
        
        // 실제 소비량이 제공된 경우 분산율 계산
        if (consumption.Actualquantity.HasValue)
        {
            consumption.Variancepercent = consumption.Plannedquantity > 0 
                ? ((consumption.Actualquantity.Value - consumption.Plannedquantity) / consumption.Plannedquantity) * 100 
                : 0;
        }
        
        return await _materialConsumptionRepository.AddAsync(consumption);
    }

    // 자재 소비 업데이트
    public async Task UpdateMaterialConsumptionAsync(Materialconsumption consumption)
    {
        var existingConsumption = await _materialConsumptionRepository.GetByIdAsync(consumption.Consumptionid);
        if (existingConsumption == null)
            throw new AppException($"Material consumption with ID {consumption.Consumptionid} not found", ErrorCodes.NotFound);

        await ValidateMaterialConsumptionAsync(consumption);
        
        // 분산율 재계산
        if (consumption.Actualquantity.HasValue)
        {
            consumption.Variancepercent = consumption.Plannedquantity > 0 
                ? ((consumption.Actualquantity.Value - consumption.Plannedquantity) / consumption.Plannedquantity) * 100 
                : 0;
        }
        
        await _materialConsumptionRepository.UpdateAsync(consumption);
    }

    // 작업지시별 자재 소비 조회
    public async Task<IEnumerable<Materialconsumption>> GetConsumptionByWorkorderAsync(decimal orderId)
    {
        return await _materialConsumptionRepository.GetByWorkorderAsync(orderId);
    }

    // 자재별 소비 조회
    public async Task<IEnumerable<Materialconsumption>> GetConsumptionByMaterialAsync(decimal itemId)
    {
        var allConsumptions = await _materialConsumptionRepository.GetAllAsync();
        return allConsumptions.Where(c => c.Itemid == itemId);
    }

    // 날짜 범위별 자재 소비 조회
    public async Task<IEnumerable<Materialconsumption>> GetConsumptionByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _materialConsumptionRepository.GetByDateRangeAsync(startDate, endDate);
    }

    // 로트번호별 자재 소비 조회
    public async Task<IEnumerable<Materialconsumption>> GetConsumptionByLotNumberAsync(string lotNumber)
    {
        var allConsumptions = await _materialConsumptionRepository.GetAllAsync();
        return allConsumptions.Where(c => c.Lotnumber == lotNumber);
    }

    // 자재 소비 분석 리포트 생성
    public async Task<MaterialConsumptionReport> AnalyzeMaterialConsumptionAsync(DateTime startDate, DateTime endDate)
    {
        var consumptions = await _materialConsumptionRepository.GetByDateRangeAsync(startDate, endDate);
        
        if (!consumptions.Any())
            return new MaterialConsumptionReport { StartDate = startDate, EndDate = endDate };

        var totalPlannedQuantity = consumptions.Sum(c => c.Plannedquantity);
        
        var totalActualQuantity = consumptions.Where(c => c.Actualquantity.HasValue).Sum(c => c.Actualquantity!.Value);
        var totalVariance = totalActualQuantity - totalPlannedQuantity;
        var averageVariancePercent = consumptions.Where(c => c.Variancepercent.HasValue).Average(c => c.Variancepercent!.Value);

        // 자재별 분석
        var materialAnalysis = new List<MaterialConsumptionAnalysis>();
        var materialGroups = consumptions.GroupBy(c => c.Itemid);
        
        foreach (var group in materialGroups)
        {
            var material = await _inventoryRepository.GetByIdAsync(group.Key);
            var materialConsumptions = group.ToList();
            
            var plannedQty = materialConsumptions.Sum(c => c.Plannedquantity);
            var actualQty = materialConsumptions.Where(c => c.Actualquantity.HasValue).Sum(c => c.Actualquantity!.Value);
            var variance = actualQty - plannedQty;
            var variancePercent = plannedQty > 0 ? (variance / plannedQty) * 100 : 0;

            materialAnalysis.Add(new MaterialConsumptionAnalysis
            {
                MaterialId = group.Key,
                MaterialName = material?.Name ?? "Unknown",
                PlannedQuantity = plannedQty,
                ActualQuantity = actualQty,
                Variance = variance,
                VariancePercent = Math.Round(variancePercent, 2),
                ConsumptionCount = materialConsumptions.Count,
                UnitCost = material?.Cost ?? 0,
                TotalCost = actualQty * (material?.Cost ?? 0)
            });
        }

        // 작업지시별 분석
        var workorderAnalysis = new List<WorkorderConsumptionAnalysis>();
        var workorderGroups = consumptions.GroupBy(c => c.Orderid);
        
        foreach (var group in workorderGroups)
        {
            var workorder = await _workorderRepository.GetByIdAsync(group.Key);
            var workorderConsumptions = group.ToList();
            
            var plannedQty = workorderConsumptions.Sum(c => c.Plannedquantity);
            var actualQty = workorderConsumptions.Where(c => c.Actualquantity.HasValue).Sum(c => c.Actualquantity!.Value);
            var variance = actualQty - plannedQty;
            var variancePercent = plannedQty > 0 ? (variance / plannedQty) * 100 : 0;

            workorderAnalysis.Add(new WorkorderConsumptionAnalysis
            {
                WorkorderId = group.Key,
                ProductId = workorder?.Productid ?? 0,
                PlannedQuantity = plannedQty,
                ActualQuantity = actualQty,
                Variance = variance,
                VariancePercent = Math.Round(variancePercent, 2),
                MaterialCount = workorderConsumptions.Count,
                TotalVariance = variance
            });
        }

        return new MaterialConsumptionReport
        {
            StartDate = startDate,
            EndDate = endDate,
            TotalPlannedQuantity = totalPlannedQuantity,
            TotalActualQuantity = totalActualQuantity,
            TotalVariance = totalVariance,
            AverageVariancePercent = Math.Round(averageVariancePercent, 2),
            MaterialAnalysis = materialAnalysis.OrderByDescending(m => m.TotalCost).ToList(),
            WorkorderAnalysis = workorderAnalysis.OrderByDescending(w => w.TotalVariance).ToList()
        };
    }

    // 자재 소비 트렌드 분석
    public async Task<MaterialConsumptionTrendReport> AnalyzeConsumptionTrendAsync(decimal itemId, DateTime startDate, DateTime endDate)
    {
        var consumptions = await _materialConsumptionRepository.GetByDateRangeAsync(startDate, endDate);
        var materialConsumptions = consumptions.Where(c => c.Itemid == itemId);
        
        var material = await _inventoryRepository.GetByIdAsync(itemId);
        if (material == null)
            throw new AppException($"Material with ID {itemId} not found", ErrorCodes.NotFound);

        var trendData = new List<ConsumptionTrendPoint>();

        // 일별 트렌드 계산
        var dailyGroups = materialConsumptions
            .GroupBy(c => DateTime.Parse(c.Consumptiondate ?? DateTime.Now.ToString("yyyy-MM-dd")).Date)
            .OrderBy(g => g.Key);

        foreach (var group in dailyGroups)
        {
            var dailyConsumptions = group.ToList();
            var plannedQty = dailyConsumptions.Sum(c => c.Plannedquantity);
            var actualQty = dailyConsumptions.Where(c => c.Actualquantity.HasValue).Sum(c => c.Actualquantity!.Value);
            var variance = actualQty - plannedQty;
            var variancePercent = plannedQty > 0 ? (variance / plannedQty) * 100 : 0;

            trendData.Add(new ConsumptionTrendPoint
            {
                Date = group.Key,
                PlannedQuantity = plannedQty,
                ActualQuantity = actualQty,
                Variance = variance,
                VariancePercent = Math.Round(variancePercent, 2),
                ConsumptionCount = dailyConsumptions.Count
            });
        }

        return new MaterialConsumptionTrendReport
        {
            MaterialId = itemId,
            MaterialName = material.Name,
            StartDate = startDate,
            EndDate = endDate,
            TrendData = trendData,
            OverallPlannedQuantity = trendData.Sum(t => t.PlannedQuantity),
            OverallActualQuantity = trendData.Sum(t => t.ActualQuantity),
            OverallVariance = trendData.Sum(t => t.Variance),
            OverallVariancePercent = trendData.Any() ? trendData.Average(t => t.VariancePercent) : 0
        };
    }

    // 자재 소비 비용 분석
    public async Task<MaterialCostAnalysis> AnalyzeMaterialCostAsync(DateTime startDate, DateTime endDate)
    {
        var consumptions = await _materialConsumptionRepository.GetByDateRangeAsync(startDate, endDate);
        var costAnalysis = new List<MaterialCostInfo>();
        var totalCost = 0m;

        foreach (var consumption in consumptions.Where(c => c.Actualquantity.HasValue))
        {
            var material = await _inventoryRepository.GetByIdAsync(consumption.Itemid);
            if (material != null)
            {
                var cost = consumption.Actualquantity!.Value * material.Cost;
                costAnalysis.Add(new MaterialCostInfo
                {
                    MaterialId = consumption.Itemid,
                    MaterialName = material.Name,
                    Quantity = consumption.Actualquantity!.Value,
                    UnitCost = material.Cost,
                    TotalCost = cost
                });
                totalCost += cost;
            }
        }

        return new MaterialCostAnalysis
        {
            StartDate = startDate,
            EndDate = endDate,
            MaterialCosts = costAnalysis.OrderByDescending(c => c.TotalCost).ToList(),
            TotalCost = totalCost,
            AverageCostPerDay = costAnalysis.Any() ? totalCost / (decimal)(endDate - startDate).TotalDays : 0
        };
    }

    // 자재 소비 알림 생성
    public async Task<IEnumerable<MaterialConsumptionAlert>> GenerateConsumptionAlertsAsync()
    {
        var alerts = new List<MaterialConsumptionAlert>();
        var recentConsumptions = await _materialConsumptionRepository.GetByDateRangeAsync(
            DateTime.Now.AddDays(-7), DateTime.Now);

        // 높은 분산율 알림
        var highVarianceConsumptions = recentConsumptions.Where(c => 
            c.Variancepercent.HasValue && Math.Abs(c.Variancepercent.Value) > 20);

        foreach (var consumption in highVarianceConsumptions)
        {
            var material = await _inventoryRepository.GetByIdAsync(consumption.Itemid);
            var workorder = await _workorderRepository.GetByIdAsync(consumption.Orderid);
            
            alerts.Add(new MaterialConsumptionAlert
            {
                MaterialId = consumption.Itemid,
                MaterialName = material?.Name ?? "Unknown",
                WorkorderId = consumption.Orderid,
                AlertType = "HighVariance",
                Message = $"High consumption variance detected: {consumption.Variancepercent}% for material {material?.Name} in workorder {consumption.Orderid}",
                Severity = "Warning",
                VariancePercent = consumption.Variancepercent ?? 0,
                CreatedDate = DateTime.Now
            });
        }

        return alerts;
    }

    // 자재 소비 유효성 검증
    private async Task ValidateMaterialConsumptionAsync(Materialconsumption consumption)
    {
        // 작업지시 존재 확인
        var workorder = await _workorderRepository.GetByIdAsync(consumption.Orderid);
        if (workorder == null)
            throw new AppException($"Workorder with ID {consumption.Orderid} not found", ErrorCodes.NotFound);

        // 자재 존재 확인
        var material = await _inventoryRepository.GetByIdAsync(consumption.Itemid);
        if (material == null)
            throw new AppException($"Material with ID {consumption.Itemid} not found", ErrorCodes.NotFound);

        // 수량 검증
        if (consumption.Plannedquantity <= 0)
            throw new AppException("Planned quantity must be greater than 0", ErrorCodes.ValidationError);

        if (consumption.Actualquantity.HasValue && consumption.Actualquantity < 0)
            throw new AppException("Actual quantity cannot be negative", ErrorCodes.ValidationError);

        // 재고 확인 (실제 소비량이 계획량보다 많은 경우)
        if (consumption.Actualquantity.HasValue && consumption.Actualquantity > consumption.Plannedquantity)
        {
            if (material.Quantity < (consumption.Actualquantity.Value - consumption.Plannedquantity))
                throw new AppException("Insufficient inventory for additional consumption", ErrorCodes.ValidationError);
        }
    }
}

// 자재 소비 분석 리포트 클래스
public class MaterialConsumptionReport
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPlannedQuantity { get; set; }
    public decimal TotalActualQuantity { get; set; }
    public decimal TotalVariance { get; set; }
    public decimal AverageVariancePercent { get; set; }
    public List<MaterialConsumptionAnalysis> MaterialAnalysis { get; set; } = new();
    public List<WorkorderConsumptionAnalysis> WorkorderAnalysis { get; set; } = new();
}

public class MaterialConsumptionAnalysis
{
    public decimal MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public decimal PlannedQuantity { get; set; }
    public decimal ActualQuantity { get; set; }
    public decimal Variance { get; set; }
    public decimal VariancePercent { get; set; }
    public int ConsumptionCount { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
}

public class WorkorderConsumptionAnalysis
{
    public decimal WorkorderId { get; set; }
    public decimal ProductId { get; set; }
    public decimal PlannedQuantity { get; set; }
    public decimal ActualQuantity { get; set; }
    public decimal Variance { get; set; }
    public decimal VariancePercent { get; set; }
    public int MaterialCount { get; set; }
    public decimal TotalVariance { get; set; }
}

// 자재 소비 트렌드 리포트 클래스
public class MaterialConsumptionTrendReport
{
    public decimal MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<ConsumptionTrendPoint> TrendData { get; set; } = new();
    public decimal OverallPlannedQuantity { get; set; }
    public decimal OverallActualQuantity { get; set; }
    public decimal OverallVariance { get; set; }
    public decimal OverallVariancePercent { get; set; }
}

public class ConsumptionTrendPoint
{
    public DateTime Date { get; set; }
    public decimal PlannedQuantity { get; set; }
    public decimal ActualQuantity { get; set; }
    public decimal Variance { get; set; }
    public decimal VariancePercent { get; set; }
    public int ConsumptionCount { get; set; }
}

// 자재 비용 분석 클래스
public class MaterialCostAnalysis
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<MaterialCostInfo> MaterialCosts { get; set; } = new();
    public decimal TotalCost { get; set; }
    public decimal AverageCostPerDay { get; set; }
}

// 자재 소비 알림 클래스
public class MaterialConsumptionAlert
{
    public decimal MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public decimal WorkorderId { get; set; }
    public string AlertType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public decimal VariancePercent { get; set; }
    public DateTime CreatedDate { get; set; }
} 