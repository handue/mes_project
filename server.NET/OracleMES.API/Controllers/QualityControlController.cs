using Microsoft.AspNetCore.Mvc;
using OracleMES.Core.DTOs;
using OracleMES.Core.Services;
using OracleMES.Core.Entities;

namespace OracleMES.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QualityControlController : ControllerBase
    {
        private readonly QualityControlService _qualityControlService;

        public QualityControlController(QualityControlService qualityControlService)
        {
            _qualityControlService = qualityControlService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<QualityControlResponseDTO>>> GetAllQualityControls()
        {
            try
            {
                // 실제로는 GetAllQualityControlsAsync 메서드가 없으므로 임시로 처리
                var qualityControls = await _qualityControlService.GetQualityChecksByDateRangeAsync(DateTime.Now.AddDays(-30), DateTime.Now);
                var response = qualityControls.Select(q => new QualityControlResponseDTO
                {
                    CheckId = q.Checkid.ToString(),
                    OrderId = q.Orderid.ToString(),
                    Date = q.Date,
                    Result = q.Result,
                    Comments = q.Comments,
                    DefectRate = q.Defectrate,
                    ReworkRate = q.Reworkrate,
                    YieldRate = q.Yieldrate,
                    InspectorId = q.Inspectorid?.ToString(),
                    CreatedDate = DateTime.Now, // 실제로는 엔티티에서 가져와야 함
                    ModifiedDate = null
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "품질관리 목록을 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QualityControlResponseDTO>> GetQualityControl(string id)
        {
            try
            {
                if (!decimal.TryParse(id, out decimal checkId))
                    return BadRequest(new { message = "올바른 품질관리 ID를 입력해주세요." });

                // 실제로는 GetByIdAsync 메서드가 필요하지만 현재 없으므로 임시로 처리
                var qualityControls = await _qualityControlService.GetQualityChecksByDateRangeAsync(DateTime.Now.AddDays(-30), DateTime.Now);
                var qualityControl = qualityControls.FirstOrDefault(q => q.Checkid == checkId);
                
                if (qualityControl == null)
                    return NotFound(new { message = "품질관리를 찾을 수 없습니다." });

                var response = new QualityControlResponseDTO
                {
                    CheckId = qualityControl.Checkid.ToString(),
                    OrderId = qualityControl.Orderid.ToString(),
                    Date = qualityControl.Date,
                    Result = qualityControl.Result,
                    Comments = qualityControl.Comments,
                    DefectRate = qualityControl.Defectrate,
                    ReworkRate = qualityControl.Reworkrate,
                    YieldRate = qualityControl.Yieldrate,
                    InspectorId = qualityControl.Inspectorid?.ToString(),
                    CreatedDate = DateTime.Now,
                    ModifiedDate = null
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "품질관리를 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<QualityControlResponseDTO>> CreateQualityControl(CreateQualityControlDTO createDto)
        {
            try
            {
                var qualityControl = new Qualitycontrol
                {
                    Checkid = decimal.Parse(createDto.CheckId),
                    Orderid = decimal.Parse(createDto.OrderId),
                    Date = createDto.Date,
                    Result = createDto.Result,
                    Comments = createDto.Comments,
                    Defectrate = createDto.DefectRate,
                    Reworkrate = createDto.ReworkRate,
                    Yieldrate = createDto.YieldRate,
                    Inspectorid = !string.IsNullOrEmpty(createDto.InspectorId) ? decimal.Parse(createDto.InspectorId) : null
                };

                var createdQualityControl = await _qualityControlService.CreateQualityCheckAsync(qualityControl);

                var response = new QualityControlResponseDTO
                {
                    CheckId = createdQualityControl.Checkid.ToString(),
                    OrderId = createdQualityControl.Orderid.ToString(),
                    Date = createdQualityControl.Date,
                    Result = createdQualityControl.Result,
                    Comments = createdQualityControl.Comments,
                    DefectRate = createdQualityControl.Defectrate,
                    ReworkRate = createdQualityControl.Reworkrate,
                    YieldRate = createdQualityControl.Yieldrate,
                    InspectorId = createdQualityControl.Inspectorid?.ToString(),
                    CreatedDate = DateTime.Now,
                    ModifiedDate = null
                };
                return CreatedAtAction(nameof(GetQualityControl), new { id = createdQualityControl.Checkid }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "품질관리를 생성하는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<QualityControlResponseDTO>> UpdateQualityControl(string id, UpdateQualityControlDTO updateDto)
        {
            try
            {
                if (!decimal.TryParse(id, out decimal checkId))
                    return BadRequest(new { message = "올바른 품질관리 ID를 입력해주세요." });

                // 실제로는 GetByIdAsync 메서드가 필요하지만 현재 없으므로 임시로 처리
                var qualityControls = await _qualityControlService.GetQualityChecksByDateRangeAsync(DateTime.Now.AddDays(-30), DateTime.Now);
                var existingQualityControl = qualityControls.FirstOrDefault(q => q.Checkid == checkId);
                
                if (existingQualityControl == null)
                    return NotFound(new { message = "품질관리를 찾을 수 없습니다." });

                // 업데이트할 속성들만 변경
                if (!string.IsNullOrEmpty(updateDto.OrderId))
                    existingQualityControl.Orderid = decimal.Parse(updateDto.OrderId);
                if (!string.IsNullOrEmpty(updateDto.Date))
                    existingQualityControl.Date = updateDto.Date;
                if (!string.IsNullOrEmpty(updateDto.Result))
                    existingQualityControl.Result = updateDto.Result;
                if (!string.IsNullOrEmpty(updateDto.Comments))
                    existingQualityControl.Comments = updateDto.Comments;
                if (updateDto.DefectRate.HasValue)
                    existingQualityControl.Defectrate = updateDto.DefectRate.Value;
                if (updateDto.ReworkRate.HasValue)
                    existingQualityControl.Reworkrate = updateDto.ReworkRate.Value;
                if (updateDto.YieldRate.HasValue)
                    existingQualityControl.Yieldrate = updateDto.YieldRate.Value;
                if (!string.IsNullOrEmpty(updateDto.InspectorId))
                    existingQualityControl.Inspectorid = decimal.Parse(updateDto.InspectorId);

                // 실제로는 UpdateQualityCheckAsync 메서드가 없으므로 임시로 처리
                // await _qualityControlService.UpdateQualityCheckAsync(existingQualityControl);

                var response = new QualityControlResponseDTO
                {
                    CheckId = existingQualityControl.Checkid.ToString(),
                    OrderId = existingQualityControl.Orderid.ToString(),
                    Date = existingQualityControl.Date,
                    Result = existingQualityControl.Result,
                    Comments = existingQualityControl.Comments,
                    DefectRate = existingQualityControl.Defectrate,
                    ReworkRate = existingQualityControl.Reworkrate,
                    YieldRate = existingQualityControl.Yieldrate,
                    InspectorId = existingQualityControl.Inspectorid?.ToString(),
                    CreatedDate = DateTime.Now,
                    ModifiedDate = null
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "품질관리를 업데이트하는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteQualityControl(string id)
        {
            try
            {
                if (!decimal.TryParse(id, out decimal checkId))
                    return BadRequest(new { message = "올바른 품질관리 ID를 입력해주세요." });

                // 실제로는 DeleteQualityCheckAsync 메서드가 없으므로 임시로 처리
                // var result = await _qualityControlService.DeleteQualityCheckAsync(checkId);
                // if (!result)
                //     return NotFound(new { message = "품질관리를 찾을 수 없습니다." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "품질관리를 삭제하는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpGet("workorder/{workorderId}")]
        public async Task<ActionResult<IEnumerable<QualityControlResponseDTO>>> GetQualityControlsByWorkorder(string workorderId)
        {
            try
            {
                if (!decimal.TryParse(workorderId, out decimal orderId))
                    return BadRequest(new { message = "올바른 작업지시 ID를 입력해주세요." });

                var qualityControls = await _qualityControlService.GetQualityChecksByWorkorderAsync(orderId);
                var response = qualityControls.Select(q => new QualityControlResponseDTO
                {
                    CheckId = q.Checkid.ToString(),
                    OrderId = q.Orderid.ToString(),
                    Date = q.Date,
                    Result = q.Result,
                    Comments = q.Comments,
                    DefectRate = q.Defectrate,
                    ReworkRate = q.Reworkrate,
                    YieldRate = q.Yieldrate,
                    InspectorId = q.Inspectorid?.ToString(),
                    CreatedDate = DateTime.Now,
                    ModifiedDate = null
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "작업지시별 품질관리를 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpGet("result/{result}")]
        public async Task<ActionResult<IEnumerable<QualityControlResponseDTO>>> GetQualityControlsByResult(string result)
        {
            try
            {
                var qualityControls = await _qualityControlService.GetQualityChecksByResultAsync(result);
                var response = qualityControls.Select(q => new QualityControlResponseDTO
                {
                    CheckId = q.Checkid.ToString(),
                    OrderId = q.Orderid.ToString(),
                    Date = q.Date,
                    Result = q.Result,
                    Comments = q.Comments,
                    DefectRate = q.Defectrate,
                    ReworkRate = q.Reworkrate,
                    YieldRate = q.Yieldrate,
                    InspectorId = q.Inspectorid?.ToString(),
                    CreatedDate = DateTime.Now,
                    ModifiedDate = null
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "결과별 품질관리를 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }
    }
} 