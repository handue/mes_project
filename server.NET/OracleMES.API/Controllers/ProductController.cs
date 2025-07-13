using Microsoft.AspNetCore.Mvc;
using OracleMES.Core.DTOs;
using OracleMES.Core.Services;
using OracleMES.Core.Entities;

namespace OracleMES.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponseDTO>>> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetActiveProductsAsync();
                var response = products.Select(p => new ProductResponseDTO
                {
                    ProductId = p.Productid.ToString(),
                    Name = p.Name,
                    Description = p.Description,
                    Category = p.Category,
                    Cost = p.Cost,
                    StandardProcessTime = p.Standardprocesstime,
                    IsActive = p.Isactive
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "제품 목록을 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponseDTO>> GetProduct(string id)
        {
            try
            {
                if (!decimal.TryParse(id, out decimal productId))
                    return BadRequest(new { message = "올바른 제품 ID를 입력해주세요." });

                // 실제로는 GetByIdAsync 메서드가 필요하지만 현재 없으므로 임시로 처리
                var products = await _productService.GetActiveProductsAsync();
                var product = products.FirstOrDefault(p => p.Productid == productId);
                
                if (product == null)
                    return NotFound(new { message = "제품을 찾을 수 없습니다." });

                var response = new ProductResponseDTO
                {
                    ProductId = product.Productid.ToString(),
                    Name = product.Name,
                    Description = product.Description,
                    Category = product.Category,
                    Cost = product.Cost,
                    StandardProcessTime = product.Standardprocesstime,
                    IsActive = product.Isactive
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "제품을 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ProductResponseDTO>> CreateProduct(CreateProductDTO createDto)
        {
            try
            {
                var product = new Product
                {
                    Productid = decimal.Parse(createDto.ProductId),
                    Name = createDto.Name,
                    Description = createDto.Description,
                    Category = createDto.Category,
                    Cost = createDto.Cost,
                    Standardprocesstime = createDto.StandardProcessTime,
                    Isactive = createDto.IsActive ?? 1
                };

                var createdProduct = await _productService.CreateProductAsync(product);

                var response = new ProductResponseDTO
                {
                    ProductId = createdProduct.Productid.ToString(),
                    Name = createdProduct.Name,
                    Description = createdProduct.Description,
                    Category = createdProduct.Category,
                    Cost = createdProduct.Cost,
                    StandardProcessTime = createdProduct.Standardprocesstime,
                    IsActive = createdProduct.Isactive
                };
                return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Productid }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "제품을 생성하는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProductResponseDTO>> UpdateProduct(string id, UpdateProductDTO updateDto)
        {
            try
            {
                if (!decimal.TryParse(id, out decimal productId))
                    return BadRequest(new { message = "올바른 제품 ID를 입력해주세요." });

                // 실제로는 GetByIdAsync 메서드가 필요하지만 현재 없으므로 임시로 처리
                var products = await _productService.GetActiveProductsAsync();
                var existingProduct = products.FirstOrDefault(p => p.Productid == productId);
                
                if (existingProduct == null)
                    return NotFound(new { message = "제품을 찾을 수 없습니다." });

                // 업데이트할 속성들만 변경
                if (!string.IsNullOrEmpty(updateDto.Name))
                    existingProduct.Name = updateDto.Name;
                if (!string.IsNullOrEmpty(updateDto.Description))
                    existingProduct.Description = updateDto.Description;
                if (!string.IsNullOrEmpty(updateDto.Category))
                    existingProduct.Category = updateDto.Category;
                if (updateDto.Cost.HasValue)
                    existingProduct.Cost = updateDto.Cost.Value;
                if (updateDto.StandardProcessTime.HasValue)
                    existingProduct.Standardprocesstime = updateDto.StandardProcessTime.Value;
                if (updateDto.IsActive.HasValue)
                    existingProduct.Isactive = updateDto.IsActive.Value;

                await _productService.UpdateProductAsync(existingProduct);

                var response = new ProductResponseDTO
                {
                    ProductId = existingProduct.Productid.ToString(),
                    Name = existingProduct.Name,
                    Description = existingProduct.Description,
                    Category = existingProduct.Category,
                    Cost = existingProduct.Cost,
                    StandardProcessTime = existingProduct.Standardprocesstime,
                    IsActive = existingProduct.Isactive
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "제품을 업데이트하는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult> UpdateProductStatus(string id, [FromBody] bool isActive)
        {
            try
            {
                if (!decimal.TryParse(id, out decimal productId))
                    return BadRequest(new { message = "올바른 제품 ID를 입력해주세요." });

                await _productService.UpdateProductStatusAsync(productId, isActive);
                return Ok(new { message = "제품 상태가 업데이트되었습니다." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "제품 상태를 업데이트하는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<ProductResponseDTO>>> GetProductsByCategory(string category)
        {
            try
            {
                var products = await _productService.GetProductsByCategoryAsync(category);
                var response = products.Select(p => new ProductResponseDTO
                {
                    ProductId = p.Productid.ToString(),
                    Name = p.Name,
                    Description = p.Description,
                    Category = p.Category,
                    Cost = p.Cost,
                    StandardProcessTime = p.Standardprocesstime,
                    IsActive = p.Isactive
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "카테고리별 제품을 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpGet("cost-range")]
        public async Task<ActionResult<IEnumerable<ProductResponseDTO>>> GetProductsByCostRange([FromQuery] decimal minCost, [FromQuery] decimal maxCost)
        {
            try
            {
                var products = await _productService.GetProductsByCostRangeAsync(minCost, maxCost);
                var response = products.Select(p => new ProductResponseDTO
                {
                    ProductId = p.Productid.ToString(),
                    Name = p.Name,
                    Description = p.Description,
                    Category = p.Category,
                    Cost = p.Cost,
                    StandardProcessTime = p.Standardprocesstime,
                    IsActive = p.Isactive
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "비용 범위별 제품을 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }
    }
} 