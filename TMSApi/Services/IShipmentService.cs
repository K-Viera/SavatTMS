using Microsoft.EntityFrameworkCore;

namespace TMSApi;

public interface IShipmentService
{
    Task<IEnumerable<ShipmentDTO>> GetAllAsync();
    Task<ShipmentDTO?> GetByTrackingNumberAsync(string trackingNumber);
    Task AddAsync(ShipmentDTO shipment);
    Task UpdateAsync(ShipmentDTO shipment);
    Task DeleteByTrackingNumberAsync(string trackingNumber);
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
}
