using OracleMES.Core.Entities;
using OracleMES.Core.Interfaces.Repositories;
using OracleMES.Core.Exceptions;

namespace OracleMES.Core.Services;

public class InventoryService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IMaterialconsumptionRepository _materialConsumptionRepository;

    public InventoryService(
        IInventoryRepository inventoryRepository,
        ISupplierRepository supplierRepository,
        IMaterialconsumptionRepository materialConsumptionRepository)
    {
        _inventoryRepository = inventoryRepository;
        _supplierRepository = supplierRepository;
        _materialConsumptionRepository = materialConsumptionRepository;
    }

    public async Task<IEnumerable<Inventory>> GetAllInventoryAsync()
    {
        return await _inventoryRepository.GetAllAsync();
    }
    // 재고 항목 생성
    public async Task<Inventory> CreateInventoryItemAsync(Inventory inventory)
    {
        // 유효성 검증
        await ValidateInventoryItemAsync(inventory);
        
        // 기본값 설정
        if (string.IsNullOrEmpty(inventory.Lastreceiveddate))
            inventory.Lastreceiveddate = DateTime.Now.ToString("yyyy-MM-dd");
        
        return await _inventoryRepository.AddAsync(inventory);
    }

    public async Task UpdateInventoryItemAsync(Inventory inventory){
        await ValidateInventoryItemAsync(inventory);

        await _inventoryRepository.UpdateAsync(inventory);
    }

    // 재고 수량 업데이트
    public async Task UpdateStockQuantityAsync(decimal itemId, decimal newQuantity)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(itemId);
        if (inventory == null)
            throw new AppException($"Inventory item with ID {itemId} not found", ErrorCodes.NotFound);

        if (newQuantity < 0)
            throw new AppException("Stock quantity cannot be negative", ErrorCodes.ValidationError);

        await _inventoryRepository.UpdateStockAsync(itemId, newQuantity);
    }

    // 재고 입고
    public async Task ReceiveStockAsync(decimal itemId, decimal quantity, string? lotNumber = null)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(itemId);
        if (inventory == null)
            throw new AppException($"Inventory item with ID {itemId} not found", ErrorCodes.NotFound);

        if (quantity <= 0)
            throw new AppException("Receive quantity must be greater than 0", ErrorCodes.ValidationError);

        var newQuantity = inventory.Quantity + quantity;
        await _inventoryRepository.UpdateStockAsync(itemId, newQuantity);
        
        // 로트번호 업데이트 (제공된 경우)
        if (!string.IsNullOrEmpty(lotNumber))
        {
            // 로트번호 업데이트 로직 (Repository에 메서드 추가 필요)
        }
        
        // 마지막 입고 날짜 업데이트
        inventory.Lastreceiveddate = DateTime.Now.ToString("yyyy-MM-dd");
    }

    // 재고 출고
    public async Task IssueStockAsync(decimal itemId, decimal quantity)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(itemId);
        if (inventory == null)
            throw new AppException($"Inventory item with ID {itemId} not found", ErrorCodes.NotFound);

        if (quantity <= 0)
            throw new AppException("Issue quantity must be greater than 0", ErrorCodes.ValidationError);

        if (inventory.Quantity < quantity)
            throw new AppException($"Insufficient stock. Available: {inventory.Quantity}, Requested: {quantity}", ErrorCodes.ValidationError);

        var newQuantity = inventory.Quantity - quantity;
        await _inventoryRepository.UpdateStockAsync(itemId, newQuantity);
    }

    // 재고 조정
    public async Task AdjustStockAsync(decimal itemId, decimal adjustment, string reason)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(itemId);
        if (inventory == null)
            throw new AppException($"Inventory item with ID {itemId} not found", ErrorCodes.NotFound);

        var newQuantity = inventory.Quantity + adjustment;
        if (newQuantity < 0)
            throw new AppException("Stock adjustment would result in negative quantity", ErrorCodes.ValidationError);

        await _inventoryRepository.UpdateStockAsync(itemId, newQuantity);
    }

    // 재고 부족 항목 조회
    public async Task<IEnumerable<Inventory>> GetLowStockItemsAsync()
    {
        return await _inventoryRepository.GetLowStockItemsAsync();
    }

    // 재고 소진 항목 조회
    public async Task<IEnumerable<Inventory>> GetOutOfStockItemsAsync()
    {
        return await _inventoryRepository.GetOutOfStockItemsAsync();
    }

    // 발주 필요 항목 조회
    public async Task<IEnumerable<Inventory>> GetItemsNeedingReorderAsync()
    {
        return await _inventoryRepository.GetItemsNeedingReorderAsync();
    }

    // 카테고리별 재고 조회
    public async Task<IEnumerable<Inventory>> GetInventoryByCategoryAsync(string category)
    {
        return await _inventoryRepository.GetByCategoryAsync(category);
    }

    // 공급업체별 재고 조회
    public async Task<IEnumerable<Inventory>> GetInventoryBySupplierAsync(decimal supplierId)
    {
        return await _inventoryRepository.GetBySupplierAsync(supplierId);
    }

    // 위치별 재고 조회
    public async Task<IEnumerable<Inventory>> GetInventoryByLocationAsync(string location)
    {
        return await _inventoryRepository.GetByLocationAsync(location);
    }

    // 비용 범위별 재고 조회
    public async Task<IEnumerable<Inventory>> GetInventoryByCostRangeAsync(decimal minCost, decimal maxCost)
    {
        return await _inventoryRepository.GetByCostRangeAsync(minCost, maxCost);
    }

    // 리드타임 범위별 재고 조회
    public async Task<IEnumerable<Inventory>> GetInventoryByLeadTimeRangeAsync(decimal minLeadTime, decimal maxLeadTime)
    {
        return await _inventoryRepository.GetByLeadTimeRangeAsync(minLeadTime, maxLeadTime);
    }

    // 재고 가치 계산
    public async Task<decimal> CalculateInventoryValueAsync()
    {
        var allInventory = await _inventoryRepository.GetAllAsync();
        return allInventory.Sum(item => item.Quantity * item.Cost);
    }

    // 재고 회전율 계산
    public async Task<InventoryTurnoverReport> CalculateInventoryTurnoverAsync(DateTime startDate, DateTime endDate)
    {
        var allInventory = await _inventoryRepository.GetAllAsync();
        var materialConsumptions = await _materialConsumptionRepository.GetByDateRangeAsync(startDate, endDate);

        var turnoverData = new List<InventoryTurnoverItem>();

        foreach (var item in allInventory)
        {
            var consumption = materialConsumptions
                .Where(mc => mc.Itemid == item.Itemid)
                .Sum(mc => mc.Actualquantity ?? 0);

            var averageInventory = item.Quantity; // 단순화된 계산
            var turnover = averageInventory > 0 ? consumption / averageInventory : 0;

            turnoverData.Add(new InventoryTurnoverItem
            {
                ItemId = item.Itemid,
                ItemName = item.Name,
                AverageInventory = averageInventory,
                Consumption = consumption,
                TurnoverRate = turnover,
                Cost = item.Cost,
                TotalValue = item.Quantity * item.Cost
            });
        }

        return new InventoryTurnoverReport
        {
            StartDate = startDate,
            EndDate = endDate,
            Items = turnoverData,
            TotalInventoryValue = turnoverData.Sum(item => item.TotalValue),
            AverageTurnoverRate = turnoverData.Any() ? turnoverData.Average(item => item.TurnoverRate) : 0
        };
    }

    // 재고 알림 생성
    public async Task<IEnumerable<InventoryAlert>> GenerateInventoryAlertsAsync()
    {
        var alerts = new List<InventoryAlert>();
        var lowStockItems = await GetLowStockItemsAsync();
        var outOfStockItems = await GetOutOfStockItemsAsync();

        // 재고 부족 알림
        foreach (var item in lowStockItems)
        {
            alerts.Add(new InventoryAlert
            {
                ItemId = item.Itemid,
                ItemName = item.Name,
                AlertType = "LowStock",
                Message = $"Low stock alert: {item.Name} has {item.Quantity} units remaining (Reorder level: {item.Reorderlevel})",
                Severity = "Warning",
                CreatedDate = DateTime.Now
            });
        }

        // 재고 소진 알림
        foreach (var item in outOfStockItems)
        {
            alerts.Add(new InventoryAlert
            {
                ItemId = item.Itemid,
                ItemName = item.Name,
                AlertType = "OutOfStock",
                Message = $"Out of stock alert: {item.Name} has no units remaining",
                Severity = "Critical",
                CreatedDate = DateTime.Now
            });
        }

        return alerts;
    }

    // 재고 유효성 검증
    private async Task ValidateInventoryItemAsync(Inventory inventory)
    {
        // 공급업체 존재 확인
        if (inventory.Supplierid.HasValue)
        {
            var supplier = await _supplierRepository.GetByIdAsync(inventory.Supplierid.Value);
            if (supplier == null)
                throw new AppException($"Supplier with ID {inventory.Supplierid} not found", ErrorCodes.NotFound);
        }

        // 필수 필드 검증
        if (string.IsNullOrEmpty(inventory.Name))
            throw new AppException("Item name is required", ErrorCodes.ValidationError);

        if (inventory.Quantity < 0)
            throw new AppException("Initial quantity cannot be negative", ErrorCodes.ValidationError);

        if (inventory.Reorderlevel < 0)
            throw new AppException("Reorder level cannot be negative", ErrorCodes.ValidationError);

        if (inventory.Leadtime < 0)
            throw new AppException("Lead time cannot be negative", ErrorCodes.ValidationError);

        if (inventory.Cost < 0)
            throw new AppException("Cost cannot be negative", ErrorCodes.ValidationError);
    }
}

// 재고 회전율 리포트 클래스
public class InventoryTurnoverReport
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<InventoryTurnoverItem> Items { get; set; } = new();
    public decimal TotalInventoryValue { get; set; }
    public decimal AverageTurnoverRate { get; set; }
}

public class InventoryTurnoverItem
{
    public decimal ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal AverageInventory { get; set; }
    public decimal Consumption { get; set; }
    public decimal TurnoverRate { get; set; }
    public decimal Cost { get; set; }
    public decimal TotalValue { get; set; }
}

// 재고 알림 클래스
public class InventoryAlert
{
    public decimal ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string AlertType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
} 