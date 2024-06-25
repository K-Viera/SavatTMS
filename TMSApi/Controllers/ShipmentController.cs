using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TMSApi;
[ApiController]
[Route("[controller]")]
[Authorize]
public class ShipmentController : ControllerBase
{
    private readonly IShipmentService _shipmentService;

    public ShipmentController(IShipmentService shipmentService)
    {
        _shipmentService = shipmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var shipments = await _shipmentService.GetAllAsync();
            return Ok(shipments);
        }
        catch  (Exception ex)
        {
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
                return NotFound();
            }
            return Ok(shipment);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] ShipmentDTO shipment)
    {
        try
        {
            await _shipmentService.AddAsync(shipment);
            return CreatedAtAction(nameof(GetByTrackingNumber), new { trackingNumber = shipment.TrackingNumber }, shipment);
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
