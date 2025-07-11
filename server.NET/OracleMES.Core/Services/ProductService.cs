using OracleMES.Core.Entities;
using OracleMES.Core.Interfaces.Repositories;
using OracleMES.Core.Exceptions;

namespace OracleMES.Core.Services;

public class ProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IBillofmaterialRepository _bomRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IWorkorderRepository _workorderRepository;

    public ProductService(
        IProductRepository productRepository,
        IBillofmaterialRepository bomRepository,
        IInventoryRepository inventoryRepository,
        IWorkorderRepository workorderRepository)
    {
        _productRepository = productRepository;
        _bomRepository = bomRepository;
        _inventoryRepository = inventoryRepository;
        _workorderRepository = workorderRepository;
    }

    // 제품 생성
    public async Task<Product> CreateProductAsync(Product product)
    {
        // 유효성 검증
        await ValidateProductAsync(product);
        
        // 기본값 설정
        if (product.Isactive == null)
            product.Isactive = 1; // 활성 상태로 설정
        
        return await _productRepository.AddAsync(product);
    }

    // 제품 정보 업데이트
    public async Task UpdateProductAsync(Product product)
    {
        var existingProduct = await _productRepository.GetByIdAsync(product.Productid);
        if (existingProduct == null)
            throw new AppException($"Product with ID {product.Productid} not found", ErrorCodes.NotFound);

        await ValidateProductAsync(product);
        await _productRepository.UpdateAsync(product);
    }

    // 제품 활성화/비활성화
    public async Task UpdateProductStatusAsync(decimal productId, bool isActive)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            throw new AppException($"Product with ID {productId} not found", ErrorCodes.NotFound);

        product.Isactive = isActive ? 1 : 0;
        await _productRepository.UpdateAsync(product);
    }

    // 제품 표준 공정 시간 업데이트
    public async Task UpdateProductProcessTimeAsync(decimal productId, decimal standardProcessTime)
    {
        if (standardProcessTime < 0)
            throw new AppException("Standard process time cannot be negative", ErrorCodes.ValidationError);

        await _productRepository.UpdateProcessTimeAsync(productId, standardProcessTime);
    }

    // 활성 제품 조회
    public async Task<IEnumerable<Product>> GetActiveProductsAsync()
    {
        return await _productRepository.GetActiveProductsAsync();
    }

    // 카테고리별 제품 조회
    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
    {
        return await _productRepository.GetByCategoryAsync(category);
    }

    // 비용 범위별 제품 조회
    public async Task<IEnumerable<Product>> GetProductsByCostRangeAsync(decimal minCost, decimal maxCost)
    {
        return await _productRepository.GetByCostRangeAsync(minCost, maxCost);
    }

    // 공정 시간 범위별 제품 조회
    public async Task<IEnumerable<Product>> GetProductsByProcessTimeRangeAsync(decimal minTime, decimal maxTime)
    {
        return await _productRepository.GetByProcessTimeRangeAsync(minTime, maxTime);
    }

    // BOM 생성
    public async Task<Billofmaterial> CreateBOMAsync(Billofmaterial bom)
    {
        // 유효성 검증
        await ValidateBOMAsync(bom);
        
        return await _bomRepository.AddAsync(bom);
    }

    // 제품별 BOM 조회
    public async Task<IEnumerable<Billofmaterial>> GetBOMByProductAsync(decimal productId)
    {
        return await _bomRepository.GetByProductAsync(productId);
    }

    // 제품별 BOM 트리 구조 생성
    public async Task<ProductBOMTree> GetProductBOMTreeAsync(decimal productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            throw new AppException($"Product with ID {productId} not found", ErrorCodes.NotFound);

        var bomItems = await _bomRepository.GetByProductAsync(productId);
        var bomTree = new ProductBOMTree
        {
            ProductId = productId,
            ProductName = product.Name,
            BOMItems = new List<BOMItemInfo>()
        };

        foreach (var bom in bomItems)
        {
            var material = await _inventoryRepository.GetByIdAsync(bom.Componentid);
            bomTree.BOMItems.Add(new BOMItemInfo
            {
                MaterialId = bom.Componentid,
                MaterialName = material?.Name ?? "Unknown",
                Quantity = bom.Quantity,
                Unit = "PCS" // 기본값으로 설정
            });
        }

        return bomTree;
    }

    // 제품 비용 계산 (BOM 기반)
    public async Task<ProductCostAnalysis> CalculateProductCostAsync(decimal productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            throw new AppException($"Product with ID {productId} not found", ErrorCodes.NotFound);

        var bomItems = await _bomRepository.GetByProductAsync(productId);
        var materialCosts = new List<MaterialCostInfo>();
        var totalMaterialCost = 0m;

        foreach (var bom in bomItems)
        {
            var material = await _inventoryRepository.GetByIdAsync(bom.Componentid);
            if (material != null)
            {
                var materialCost = bom.Quantity * material.Cost;
                materialCosts.Add(new MaterialCostInfo
                {
                    MaterialId = bom.Componentid,
                    MaterialName = material.Name,
                    Quantity = bom.Quantity,
                    UnitCost = material.Cost,
                    TotalCost = materialCost
                });
                totalMaterialCost += materialCost;
            }
        }

        // 제품 비용 = 재료비 + 제조 간접비 (간단한 계산)
        var manufacturingOverhead = totalMaterialCost * 0.3m; // 재료비의 30%로 가정
        var totalProductCost = totalMaterialCost + manufacturingOverhead;

        return new ProductCostAnalysis
        {
            ProductId = productId,
            ProductName = product.Name,
            MaterialCosts = materialCosts,
            TotalMaterialCost = totalMaterialCost,
            ManufacturingOverhead = manufacturingOverhead,
            TotalProductCost = totalProductCost,
            StandardProcessTime = product.Standardprocesstime ?? 0
        };
    }

    // 제품 생산 이력 조회
    public async Task<IEnumerable<Workorder>> GetProductProductionHistoryAsync(decimal productId, DateTime startDate, DateTime endDate)
    {
        var workorders = await _workorderRepository.GetByProductAsync(productId);
        return workorders.Where(w => 
            DateTime.TryParse(w.Actualstarttime, out var start) && 
            start >= startDate && start <= endDate);
    }

    // 제품 생산성 분석
    public async Task<ProductProductionReport> AnalyzeProductProductionAsync(decimal productId, DateTime startDate, DateTime endDate)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            throw new AppException($"Product with ID {productId} not found", ErrorCodes.NotFound);

        var workorders = await _workorderRepository.GetByProductAsync(productId);
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

        return new ProductProductionReport
        {
            ProductId = productId,
            ProductName = product.Name,
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
            StandardProcessTime = product.Standardprocesstime ?? 0
        };
    }

    // 제품 카테고리별 통계
    public async Task<ProductCategoryReport> GenerateCategoryReportAsync()
    {
        var allProducts = await _productRepository.GetAllAsync();
        var categoryGroups = allProducts.GroupBy(p => p.Category ?? "Uncategorized");

        var categoryReports = new List<CategoryInfo>();
        foreach (var group in categoryGroups)
        {
            var products = group.ToList();
            var activeProducts = products.Count(p => p.Isactive == 1);
            var totalCost = products.Sum(p => p.Cost);
            var averageCost = products.Any() ? totalCost / products.Count : 0;

            categoryReports.Add(new CategoryInfo
            {
                Category = group.Key,
                TotalProducts = products.Count,
                ActiveProducts = activeProducts,
                TotalCost = totalCost,
                AverageCost = Math.Round(averageCost, 2)
            });
        }

        return new ProductCategoryReport
        {
            CategoryReports = categoryReports,
            TotalProducts = allProducts.Count(),
            TotalActiveProducts = allProducts.Count(p => p.Isactive == 1),
            OverallAverageCost = allProducts.Any() ? Math.Round(allProducts.Average(p => p.Cost), 2) : 0
        };
    }

    // 제품 유효성 검증
    private async Task ValidateProductAsync(Product product)
    {
        // 필수 필드 검증
        if (string.IsNullOrEmpty(product.Name))
            throw new AppException("Product name is required", ErrorCodes.ValidationError);

        if (product.Cost < 0)
            throw new AppException("Product cost cannot be negative", ErrorCodes.ValidationError);

        if (product.Standardprocesstime.HasValue && product.Standardprocesstime < 0)
            throw new AppException("Standard process time cannot be negative", ErrorCodes.ValidationError);

        // 카테고리 유효성 검증 (선택적)
        var validCategories = new[] { "Raw Material", "Semi-Finished", "Finished Good", "Spare Part", "Tool" };
        if (!string.IsNullOrEmpty(product.Category) && !validCategories.Contains(product.Category))
            throw new AppException($"Invalid category: {product.Category}. Valid categories are: {string.Join(", ", validCategories)}", ErrorCodes.ValidationError);
    }

    // BOM 유효성 검증
    private async Task ValidateBOMAsync(Billofmaterial bom)
    {
        // 제품 존재 확인
        var product = await _productRepository.GetByIdAsync(bom.Productid);
        if (product == null)
            throw new AppException($"Product with ID {bom.Productid} not found", ErrorCodes.NotFound);

        // 재료 존재 확인
        var material = await _inventoryRepository.GetByIdAsync(bom.Componentid);
        if (material == null)
            throw new AppException($"Material with ID {bom.Componentid} not found", ErrorCodes.NotFound);

        // 수량 검증
        if (bom.Quantity <= 0)
            throw new AppException("BOM quantity must be greater than 0", ErrorCodes.ValidationError);
    }
}

// 제품 BOM 트리 클래스
public class ProductBOMTree
{
    public decimal ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public List<BOMItemInfo> BOMItems { get; set; } = new();
}

public class BOMItemInfo
{
    public decimal MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}

// 제품 비용 분석 클래스
public class ProductCostAnalysis
{
    public decimal ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public List<MaterialCostInfo> MaterialCosts { get; set; } = new();
    public decimal TotalMaterialCost { get; set; }
    public decimal ManufacturingOverhead { get; set; }
    public decimal TotalProductCost { get; set; }
    public decimal StandardProcessTime { get; set; }
}

public class MaterialCostInfo
{
    public decimal MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
}

// 제품 생산 리포트 클래스
public class ProductProductionReport
{
    public decimal ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
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
    public decimal StandardProcessTime { get; set; }
}

// 제품 카테고리 리포트 클래스
public class ProductCategoryReport
{
    public List<CategoryInfo> CategoryReports { get; set; } = new();
    public int TotalProducts { get; set; }
    public int TotalActiveProducts { get; set; }
    public decimal OverallAverageCost { get; set; }
}

public class CategoryInfo
{
    public string Category { get; set; } = string.Empty;
    public int TotalProducts { get; set; }
    public int ActiveProducts { get; set; }
    public decimal TotalCost { get; set; }
    public decimal AverageCost { get; set; }
} 