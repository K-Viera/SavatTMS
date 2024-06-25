using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TMSApi;
[ApiController]
[Route("[controller]")]
[Authorize]
public class ShipmentController : ControllerBase
{
    private readonly IShipmentService _shipmentService;
    private readonly ILogger<ShipmentController> _logger;

    public ShipmentController(IShipmentService shipmentService, ILogger<ShipmentController> logger)
    {
        _shipmentService = shipmentService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var shipments = await _shipmentService.GetAllAsync();
            _logger.LogInformation("All shipments were retrieved");
            return Ok(shipments);
        }
        catch  (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting all shipments");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("{trackingNumber}")]
    public async Task<IActionResult> GetByTrackingNumber(string trackingNumber)
    {
        try
        {
            var shipment = await _shipmentService.GetByTrackingNumberAsync(trackingNumber);
            if (shipment == null)
            {
                _logger.LogWarning($"Shipment with tracking number {trackingNumber} was not found");
                return NotFound();
            }
            _logger.LogInformation($"Shipment with tracking number {trackingNumber} was retrieved");
            return Ok(shipment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting shipment by tracking number");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] ShipmentDTO shipment)
    {
        try
        {
            await _shipmentService.AddAsync(shipment);
            _logger.LogInformation($"Shipment with tracking number {shipment.TrackingNumber} was added");
            return CreatedAtAction(nameof(GetByTrackingNumber), new { trackingNumber = shipment.TrackingNumber }, shipment);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "An error occurred while adding a shipment");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adding a shipment");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPut("{trackingNumber}")]
    public async Task<IActionResult> Update([FromBody] UpdateShipmentRequest shipment,[FromRoute]string trackingNumber)
    {
        try
        {
            ShipmentDTO? existingShipment = await _shipmentService.GetByTrackingNumberAsync(trackingNumber);
            if (existingShipment == null)
            {
                return BadRequest("Shipment not found");
            }
            if (!UpdateIfNecessary(existingShipment, shipment))
            {
                return Ok("No changes made");
            }
            await _shipmentService.UpdateAsync(existingShipment);
            _logger.LogInformation($"Shipment with tracking number {trackingNumber} was updated");
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpDelete("{trackingNumber}")]
    public async Task<IActionResult> DeleteByTrackingNumber(string trackingNumber)
    {
        try
        {
            await _shipmentService.DeleteByTrackingNumberAsync(trackingNumber);
            _logger.LogInformation($"Shipment with tracking number {trackingNumber} was deleted");
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting a shipment");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
    
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] decimal? weight, 
        [FromQuery] DateTime? shippingDate, 
        [FromQuery] DateTime? deliveryDate, 
        [FromQuery] string? origin, 
        [FromQuery] string? destination,
        [FromQuery] string? status
        )
    {
        try
        {
            var shipments = await _shipmentService.SearchAsync(weight, shippingDate, deliveryDate, origin, destination,status);
            _logger.LogInformation($"Shipments were searched, fond {shipments.Count()} shipments");
            return Ok(shipments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while searching shipments");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    private bool UpdateIfNecessary(ShipmentDTO shipment, UpdateShipmentRequest updateShipmentRequest)
    {
        if (shipment.Status != updateShipmentRequest.Status)
        {
            shipment.Status = updateShipmentRequest.Status;
            return true;
        }
        if (shipment.Origin != updateShipmentRequest.Origin)
        {
            shipment.Origin = updateShipmentRequest.Origin;
            return true;
        }
        if (shipment.Destination != updateShipmentRequest.Destination)
        {
            shipment.Destination = updateShipmentRequest.Destination;
            return true;
        }
        if (shipment.Weight != updateShipmentRequest.Weight)
        {
            shipment.Weight = updateShipmentRequest.Weight;
            return true;
        }
        if (shipment.ShippingDate != updateShipmentRequest.ShippingDate)
        {
            shipment.ShippingDate = updateShipmentRequest.ShippingDate;
            return true;
        }
        if (shipment.DeliveryDate != updateShipmentRequest.DeliveryDate)
        {
            shipment.DeliveryDate = updateShipmentRequest.DeliveryDate;
            return true;
        }
        return false;
    }
}
