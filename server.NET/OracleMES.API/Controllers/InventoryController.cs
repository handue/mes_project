using Microsoft.AspNetCore.Mvc;
using OracleMES.Core.DTOs;
using OracleMES.Core.Services;
using OracleMES.Core.Entities;

namespace OracleMES.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryService _inventoryService;

        public InventoryController(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryResponseDTO>>> GetAllInventory()
        {
            try
            {
                var inventory = await _inventoryService.GetAllInventoryAsync();
                var response = inventory.Select(i => new InventoryResponseDTO
                {
                    ItemId = i.Itemid.ToString(),
                    Name = i.Name,
                    Category = i.Category,
                    Quantity = i.Quantity,
                    ReorderLevel = i.Reorderlevel,
                    SupplierId = i.Supplierid?.ToString(),
                    LeadTime = i.Leadtime,
                    Cost = i.Cost,
                    LotNumber = i.Lotnumber,
                    Location = i.Location,
                    LastReceivedDate = i.Lastreceiveddate
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "재고 목록을 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryResponseDTO>> GetInventory(string id)
        {
            try
            {
                if (!decimal.TryParse(id, out decimal itemId))
                    return BadRequest(new { message = "올바른 재고 ID를 입력해주세요." });

                // 실제로는 GetByIdAsync 메서드가 필요하지만 현재 없으므로 임시로 처리
                var inventory = await _inventoryService.GetAllInventoryAsync();
                var item = inventory.FirstOrDefault(i => i.Itemid == itemId);
                
                if (item == null)
                    return NotFound(new { message = "재고를 찾을 수 없습니다." });

                var response = new InventoryResponseDTO
                {
                    ItemId = item.Itemid.ToString(),
                    Name = item.Name,
                    Category = item.Category,
                    Quantity = item.Quantity,
                    ReorderLevel = item.Reorderlevel,
                    SupplierId = item.Supplierid?.ToString(),
                    LeadTime = item.Leadtime,
                    Cost = item.Cost,
                    LotNumber = item.Lotnumber,
                    Location = item.Location,
                    LastReceivedDate = item.Lastreceiveddate
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "재고를 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<InventoryResponseDTO>> CreateInventory(CreateInventoryDTO createDto)
        {
            try
            {
                var inventory = new Inventory
                {
                    Itemid = decimal.Parse(createDto.ItemId),
                    Name = createDto.Name,
                    Category = createDto.Category,
                    Quantity = createDto.Quantity,
                    Reorderlevel = createDto.ReorderLevel,
                    Supplierid = !string.IsNullOrEmpty(createDto.SupplierId) ? decimal.Parse(createDto.SupplierId) : null,
                    Leadtime = createDto.LeadTime,
                    Cost = createDto.Cost,
                    Lotnumber = createDto.LotNumber,
                    Location = createDto.Location,
                    Lastreceiveddate = createDto.LastReceivedDate
                };

                var createdInventory = await _inventoryService.CreateInventoryItemAsync(inventory);

                var response = new InventoryResponseDTO
                {
                    ItemId = createdInventory.Itemid.ToString(),
                    Name = createdInventory.Name,
                    Category = createdInventory.Category,
                    Quantity = createdInventory.Quantity,
                    ReorderLevel = createdInventory.Reorderlevel,
                    SupplierId = createdInventory.Supplierid?.ToString(),
                    LeadTime = createdInventory.Leadtime,
                    Cost = createdInventory.Cost,
                    LotNumber = createdInventory.Lotnumber,
                    Location = createdInventory.Location,
                    LastReceivedDate = createdInventory.Lastreceiveddate
                };
                return CreatedAtAction(nameof(GetInventory), new { id = createdInventory.Itemid }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "재고를 생성하는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<InventoryResponseDTO>> UpdateInventory(string id, UpdateInventoryDTO updateDto)
        {
            try
            {
                if (!decimal.TryParse(id, out decimal itemId))
                    return BadRequest(new { message = "올바른 재고 ID를 입력해주세요." });

                // 실제로는 GetByIdAsync 메서드가 필요하지만 현재 없으므로 임시로 처리
                var inventory = await _inventoryService.GetAllInventoryAsync();
                var existingItem = inventory.FirstOrDefault(i => i.Itemid == itemId);
                
                if (existingItem == null)
                    return NotFound(new { message = "재고를 찾을 수 없습니다." });

                // 업데이트할 속성들만 변경
                if (!string.IsNullOrEmpty(updateDto.Name))
                    existingItem.Name = updateDto.Name;
                if (!string.IsNullOrEmpty(updateDto.Category))
                    existingItem.Category = updateDto.Category;
                if (updateDto.Quantity.HasValue)
                    existingItem.Quantity = updateDto.Quantity.Value;
                if (updateDto.ReorderLevel.HasValue)
                    existingItem.Reorderlevel = updateDto.ReorderLevel.Value;
                if (!string.IsNullOrEmpty(updateDto.SupplierId))
                    existingItem.Supplierid = decimal.Parse(updateDto.SupplierId);
                if (updateDto.LeadTime.HasValue)
                    existingItem.Leadtime = updateDto.LeadTime.Value;
                if (updateDto.Cost.HasValue)
                    existingItem.Cost = updateDto.Cost.Value;
                if (!string.IsNullOrEmpty(updateDto.LotNumber))
                    existingItem.Lotnumber = updateDto.LotNumber;
                if (!string.IsNullOrEmpty(updateDto.Location))
                    existingItem.Location = updateDto.Location;
                if (!string.IsNullOrEmpty(updateDto.LastReceivedDate))
                    existingItem.Lastreceiveddate = updateDto.LastReceivedDate;

                await _inventoryService.UpdateInventoryItemAsync(existingItem);

                var response = new InventoryResponseDTO
                {
                    ItemId = existingItem.Itemid.ToString(),
                    Name = existingItem.Name,
                    Category = existingItem.Category,
                    Quantity = existingItem.Quantity,
                    ReorderLevel = existingItem.Reorderlevel,
                    SupplierId = existingItem.Supplierid?.ToString(),
                    LeadTime = existingItem.Leadtime,
                    Cost = existingItem.Cost,
                    LotNumber = existingItem.Lotnumber,
                    Location = existingItem.Location,
                    LastReceivedDate = existingItem.Lastreceiveddate
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "재고를 업데이트하는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpPut("{id}/adjust-stock")]
        public async Task<ActionResult> AdjustStock(string id, [FromBody] AdjustStockRequest request)
        {
            try
            {
                if (!decimal.TryParse(id, out decimal itemId))
                    return BadRequest(new { message = "올바른 재고 ID를 입력해주세요." });

                await _inventoryService.AdjustStockAsync(itemId, request.Adjustment, request.Reason);
                return Ok(new { message = "재고가 조정되었습니다." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "재고 조정 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        public class AdjustStockRequest
        {
            public decimal Adjustment { get; set; }
            public string Reason { get; set; } = string.Empty;
        }

        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<InventoryResponseDTO>>> GetLowStockItems()
        {
            try
            {
                var inventory = await _inventoryService.GetLowStockItemsAsync();
                var response = inventory.Select(i => new InventoryResponseDTO
                {
                    ItemId = i.Itemid.ToString(),
                    Name = i.Name,
                    Category = i.Category,
                    Quantity = i.Quantity,
                    ReorderLevel = i.Reorderlevel,
                    SupplierId = i.Supplierid?.ToString(),
                    LeadTime = i.Leadtime,
                    Cost = i.Cost,
                    LotNumber = i.Lotnumber,
                    Location = i.Location,
                    LastReceivedDate = i.Lastreceiveddate
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "재고 부족 항목을 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpGet("out-of-stock")]
        public async Task<ActionResult<IEnumerable<InventoryResponseDTO>>> GetOutOfStockItems()
        {
            try
            {
                var inventory = await _inventoryService.GetOutOfStockItemsAsync();
                var response = inventory.Select(i => new InventoryResponseDTO
                {
                    ItemId = i.Itemid.ToString(),
                    Name = i.Name,
                    Category = i.Category,
                    Quantity = i.Quantity,
                    ReorderLevel = i.Reorderlevel,
                    SupplierId = i.Supplierid?.ToString(),
                    LeadTime = i.Leadtime,
                    Cost = i.Cost,
                    LotNumber = i.Lotnumber,
                    Location = i.Location,
                    LastReceivedDate = i.Lastreceiveddate
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "재고 소진 항목을 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<InventoryResponseDTO>>> GetInventoryByCategory(string category)
        {
            try
            {
                var inventory = await _inventoryService.GetInventoryByCategoryAsync(category);
                var response = inventory.Select(i => new InventoryResponseDTO
                {
                    ItemId = i.Itemid.ToString(),
                    Name = i.Name,
                    Category = i.Category,
                    Quantity = i.Quantity,
                    ReorderLevel = i.Reorderlevel,
                    SupplierId = i.Supplierid?.ToString(),
                    LeadTime = i.Leadtime,
                    Cost = i.Cost,
                    LotNumber = i.Lotnumber,
                    Location = i.Location,
                    LastReceivedDate = i.Lastreceiveddate
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "카테고리별 재고를 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpGet("supplier/{supplierId}")]
        public async Task<ActionResult<IEnumerable<InventoryResponseDTO>>> GetInventoryBySupplier(string supplierId)
        {
            try
            {
                if (!decimal.TryParse(supplierId, out decimal supplierIdDecimal))
                    return BadRequest(new { message = "올바른 공급업체 ID를 입력해주세요." });

                var inventory = await _inventoryService.GetInventoryBySupplierAsync(supplierIdDecimal);
                var response = inventory.Select(i => new InventoryResponseDTO
                {
                    ItemId = i.Itemid.ToString(),
                    Name = i.Name,
                    Category = i.Category,
                    Quantity = i.Quantity,
                    ReorderLevel = i.Reorderlevel,
                    SupplierId = i.Supplierid?.ToString(),
                    LeadTime = i.Leadtime,
                    Cost = i.Cost,
                    LotNumber = i.Lotnumber,
                    Location = i.Location,
                    LastReceivedDate = i.Lastreceiveddate
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "공급업체별 재고를 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }
    }
} 