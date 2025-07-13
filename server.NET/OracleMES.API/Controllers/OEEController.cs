using Microsoft.AspNetCore.Mvc;
using OracleMES.Core.DTOs;
using OracleMES.Core.Services;
using OracleMES.Core.Entities;


namespace OracleMES.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OEEController : ControllerBase
    {
        private readonly OEEService _oeeService;

        public OEEController(OEEService oeeService)
        {
            _oeeService = oeeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OEEResponseDTO>>> GetAllOEE()
        {
            try
            {
                var oeeMetrics = await _oeeService.GetTodayOEEMetricsAsync();
                var response = oeeMetrics.Select(o => new OEEResponseDTO
                {
                    MetricId = o.Metricid.ToString(),
                    MachineId = o.Machineid.ToString(),
                    Date = o.Date,
                    Availability = o.Availability,
                    Performance = o.Performance,
                    Quality = o.Quality,
                    Oee = o.Oee,
                    PlannedProductionTime = o.Plannedproductiontime,
                    ActualProductionTime = o.Actualproductiontime,
                    Downtime = o.Downtime,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = null
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "OEE 목록을 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OEEResponseDTO>> GetOEE(string id)
        {
            try
            {
                if (!decimal.TryParse(id, out decimal metricId))
                    return BadRequest(new { message = "올바른 OEE ID를 입력해주세요." });

                // 실제로는 GetByIdAsync 메서드가 필요하지만 현재 없으므로 임시로 처리
                var oeeMetrics = await _oeeService.GetTodayOEEMetricsAsync();
                var oeeMetric = oeeMetrics.FirstOrDefault(o => o.Metricid == metricId);
                
                if (oeeMetric == null)
                    return NotFound(new { message = "OEE를 찾을 수 없습니다." });

                var response = new OEEResponseDTO
                {
                    MetricId = oeeMetric.Metricid.ToString(),
                    MachineId = oeeMetric.Machineid.ToString(),
                    Date = oeeMetric.Date,
                    Availability = oeeMetric.Availability,
                    Performance = oeeMetric.Performance,
                    Quality = oeeMetric.Quality,
                    Oee = oeeMetric.Oee,
                    PlannedProductionTime = oeeMetric.Plannedproductiontime,
                    ActualProductionTime = oeeMetric.Actualproductiontime,
                    Downtime = oeeMetric.Downtime,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = null
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "OEE를 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<OEEResponseDTO>> CreateOEE(CreateOEEDTO createDto)
        {
            try
            {
                var oeeMetric = new Oeemetric
                {
                    Metricid = decimal.Parse(createDto.MetricId),
                    Machineid = decimal.Parse(createDto.MachineId),
                    Date = createDto.Date ?? DateTime.Now.ToString("yyyy-MM-dd"),
                    Availability = createDto.Availability,
                    Performance = createDto.Performance,
                    Quality = createDto.Quality,
                    Oee = createDto.Oee,
                    Plannedproductiontime = createDto.PlannedProductionTime,
                    Actualproductiontime = createDto.ActualProductionTime,
                    Downtime = createDto.Downtime
                };

                // 실제로는 CreateOEEMetricAsync 메서드가 없으므로 임시로 처리
                // var createdOEE = await _oeeService.CreateOEEMetricAsync(oeeMetric);

                var response = new OEEResponseDTO
                {
                    MetricId = oeeMetric.Metricid.ToString(),
                    MachineId = oeeMetric.Machineid.ToString(),
                    Date = oeeMetric.Date,
                    Availability = oeeMetric.Availability,
                    Performance = oeeMetric.Performance,
                    Quality = oeeMetric.Quality,
                    Oee = oeeMetric.Oee,
                    PlannedProductionTime = oeeMetric.Plannedproductiontime,
                    ActualProductionTime = oeeMetric.Actualproductiontime,
                    Downtime = oeeMetric.Downtime,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = null
                };
                return CreatedAtAction(nameof(GetOEE), new { id = oeeMetric.Metricid }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "OEE를 생성하는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<OEEResponseDTO>> UpdateOEE(string id, UpdateOEEDTO updateDto)
        {
            try
            {
                if (!decimal.TryParse(id, out decimal metricId))
                    return BadRequest(new { message = "올바른 OEE ID를 입력해주세요." });

                // 실제로는 GetByIdAsync 메서드가 필요하지만 현재 없으므로 임시로 처리
                var oeeMetrics = await _oeeService.GetTodayOEEMetricsAsync();
                var existingOEEMetric = oeeMetrics.FirstOrDefault(o => o.Metricid == metricId);
                
                if (existingOEEMetric == null)
                    return NotFound(new { message = "OEE를 찾을 수 없습니다." });

                // 업데이트할 속성들만 변경
                if (!string.IsNullOrEmpty(updateDto.MachineId))
                    existingOEEMetric.Machineid = decimal.Parse(updateDto.MachineId);
                if (!string.IsNullOrEmpty(updateDto.Date))
                    existingOEEMetric.Date = updateDto.Date;
                if (updateDto.Availability.HasValue)
                    existingOEEMetric.Availability = updateDto.Availability.Value;
                if (updateDto.Performance.HasValue)
                    existingOEEMetric.Performance = updateDto.Performance.Value;
                if (updateDto.Quality.HasValue)
                    existingOEEMetric.Quality = updateDto.Quality.Value;
                if (updateDto.Oee.HasValue)
                    existingOEEMetric.Oee = updateDto.Oee.Value;
                if (updateDto.PlannedProductionTime.HasValue)
                    existingOEEMetric.Plannedproductiontime = updateDto.PlannedProductionTime.Value;
                if (updateDto.ActualProductionTime.HasValue)
                    existingOEEMetric.Actualproductiontime = updateDto.ActualProductionTime.Value;
                if (updateDto.Downtime.HasValue)
                    existingOEEMetric.Downtime = updateDto.Downtime.Value;

                // 실제로는 UpdateOEEMetricAsync 메서드가 없으므로 임시로 처리
                // await _oeeService.UpdateOEEMetricAsync(existingOEEMetric);

                var response = new OEEResponseDTO
                {
                    MetricId = existingOEEMetric.Metricid.ToString(),
                    MachineId = existingOEEMetric.Machineid.ToString(),
                    Date = existingOEEMetric.Date,
                    Availability = existingOEEMetric.Availability,
                    Performance = existingOEEMetric.Performance,
                    Quality = existingOEEMetric.Quality,
                    Oee = existingOEEMetric.Oee,
                    PlannedProductionTime = existingOEEMetric.Plannedproductiontime,
                    ActualProductionTime = existingOEEMetric.Actualproductiontime,
                    Downtime = existingOEEMetric.Downtime,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = null
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "OEE를 업데이트하는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOEE(string id)
        {
            try
            {
                if (!decimal.TryParse(id, out decimal metricId))
                    return BadRequest(new { message = "올바른 OEE ID를 입력해주세요." });

                // 실제로는 DeleteOEEMetricAsync 메서드가 없으므로 임시로 처리
                // var result = await _oeeService.DeleteOEEMetricAsync(metricId);
                // if (!result)
                //     return NotFound(new { message = "OEE를 찾을 수 없습니다." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "OEE를 삭제하는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpGet("machine/{machineId}")]
        public async Task<ActionResult<IEnumerable<OEEResponseDTO>>> GetOEEByMachine(string machineId)
        {
            try
            {
                if (!decimal.TryParse(machineId, out decimal machineIdDecimal))
                    return BadRequest(new { message = "올바른 설비 ID를 입력해주세요." });

                var oeeMetrics = await _oeeService.GetOEEMetricsByMachineAsync(machineIdDecimal);
                var response = oeeMetrics.Select(o => new OEEResponseDTO
                {
                    MetricId = o.Metricid.ToString(),
                    MachineId = o.Machineid.ToString(),
                    Date = o.Date,
                    Availability = o.Availability,
                    Performance = o.Performance,
                    Quality = o.Quality,
                    Oee = o.Oee,
                    PlannedProductionTime = o.Plannedproductiontime,
                    ActualProductionTime = o.Actualproductiontime,
                    Downtime = o.Downtime,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = null
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "설비별 OEE를 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpGet("date/{date}")]
        public async Task<ActionResult<IEnumerable<OEEResponseDTO>>> GetOEEByDate(string date)
        {
            try
            {
                if (!DateTime.TryParse(date, out DateTime targetDate))
                    return BadRequest(new { message = "올바른 날짜를 입력해주세요." });

                var oeeMetrics = await _oeeService.GetOEEMetricsByDateRangeAsync(targetDate, targetDate);
                var response = oeeMetrics.Select(o => new OEEResponseDTO
                {
                    MetricId = o.Metricid.ToString(),
                    MachineId = o.Machineid.ToString(),
                    Date = o.Date,
                    Availability = o.Availability,
                    Performance = o.Performance,
                    Quality = o.Quality,
                    Oee = o.Oee,
                    PlannedProductionTime = o.Plannedproductiontime,
                    ActualProductionTime = o.Actualproductiontime,
                    Downtime = o.Downtime,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = null
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "날짜별 OEE를 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpGet("high")]
        public async Task<ActionResult<IEnumerable<OEEResponseDTO>>> GetHighOEE([FromQuery] decimal threshold = 85)
        {
            try
            {
                var oeeMetrics = await _oeeService.GetHighOEEMachinesAsync(threshold);
                var response = oeeMetrics.Select(o => new OEEResponseDTO
                {
                    MetricId = o.Metricid.ToString(),
                    MachineId = o.Machineid.ToString(),
                    Date = o.Date,
                    Availability = o.Availability,
                    Performance = o.Performance,
                    Quality = o.Quality,
                    Oee = o.Oee,
                    PlannedProductionTime = o.Plannedproductiontime,
                    ActualProductionTime = o.Actualproductiontime,
                    Downtime = o.Downtime,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = null
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "높은 OEE를 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpGet("low")]
        public async Task<ActionResult<IEnumerable<OEEResponseDTO>>> GetLowOEE([FromQuery] decimal threshold = 70)
        {
            try
            {
                var oeeMetrics = await _oeeService.GetLowOEEMachinesAsync(threshold);
                var response = oeeMetrics.Select(o => new OEEResponseDTO
                {
                    MetricId = o.Metricid.ToString(),
                    MachineId = o.Machineid.ToString(),
                    Date = o.Date,
                    Availability = o.Availability,
                    Performance = o.Performance,
                    Quality = o.Quality,
                    Oee = o.Oee,
                    PlannedProductionTime = o.Plannedproductiontime,
                    ActualProductionTime = o.Actualproductiontime,
                    Downtime = o.Downtime,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = null
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "낮은 OEE를 가져오는 중 오류가 발생했습니다.", error = ex.Message });
            }
        }
    }
} 