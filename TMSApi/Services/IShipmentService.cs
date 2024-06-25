using Microsoft.EntityFrameworkCore;

namespace TMSApi;

public interface IShipmentService
{
    Task<IEnumerable<ShipmentDTO>> GetAllAsync();
    Task<ShipmentDTO?> GetByTrackingNumberAsync(string trackingNumber);
    Task AddAsync(ShipmentDTO shipment);
    Task UpdateAsync(ShipmentDTO shipment);
    Task DeleteByTrackingNumberAsync(string trackingNumber);
    Task<IEnumerable<ShipmentDTO>> SearchAsync(
        decimal? weight,
        DateTime? shippingDate,
        DateTime? deliveryDate,
        string? origin,
        string? destination,
        string? status
        );

}

public class ShipmentService : IShipmentService
{
    private readonly ApplicationDbContext _context;

    public ShipmentService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<ShipmentDTO>> GetAllAsync()
    {
        var shipments = await _context.Shipments.ToListAsync();
        var shipmentDTOs = shipments.Select(shipment => shipment.ToDTO());
        return shipmentDTOs;
    }

    public async Task<ShipmentDTO?> GetByTrackingNumberAsync(string trackingNumber)
    {
        var shipment = await _context.Shipments.FirstOrDefaultAsync(shipment => shipment.TrackingNumber == trackingNumber);
        return shipment?.ToDTO();
    }

    public async Task AddAsync(ShipmentDTO shipment)
    {
        //Check if the shipment already exists
        if (await _context.Shipments.AnyAsync(s => s.TrackingNumber == shipment.TrackingNumber))
        {
            throw new InvalidOperationException("Shipment already exists");
        }
        _context.Shipments.Add(new Shipment(shipment));
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ShipmentDTO shipment)
    {
        var existingShipment = await _context.Shipments.FirstOrDefaultAsync(s => s.TrackingNumber == shipment.TrackingNumber);
        if (existingShipment == null)
        {
            throw new InvalidOperationException("Shipment does not exist");
        }
        existingShipment.Update(shipment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteByTrackingNumberAsync(string trackingNumber)
    {
        var shipment = await _context.Shipments.FirstOrDefaultAsync(s => s.TrackingNumber == trackingNumber);
        if (shipment == null)
        {
            throw new InvalidOperationException("Shipment does not exist");
        }
        _context.Shipments.Remove(shipment);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ShipmentDTO>> SearchAsync(decimal? weight, DateTime? shippingDate, DateTime? deliveryDate, string? origin, string? destination, string? status)
    {
        var query = _context.Shipments.AsQueryable();

        if (weight.HasValue)
        {
            query = query.Where(s => s.Weight == weight.Value);
        }

        if (shippingDate.HasValue)
        {
            query = query.Where(s => s.ShippingDate.Date == shippingDate.Value.Date);
        }

        if (deliveryDate.HasValue)
        {
            query = query.Where(s => s.DeliveryDate.HasValue && s.DeliveryDate.Value.Date == deliveryDate.Value.Date);
        }

        if (!string.IsNullOrWhiteSpace(origin))
        {
            query = query.Where(s => s.Origin == origin);
        }

        if (!string.IsNullOrWhiteSpace(destination))
        {
            query = query.Where(s => s.Destination == destination);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(s => s.Status == status);
        }

        return await query.Select(s => s.ToDTO()).ToListAsync();
    }
}
