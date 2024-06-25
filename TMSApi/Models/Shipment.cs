using System.ComponentModel.DataAnnotations;

namespace TMSApi;

public class Shipment
{
    [Key]
    public int ShipmentId { get; set; }
    
    [Required]
    [StringLength(50)]
    public string TrackingNumber { get; set; } = null!;
    
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = null!;
    
    [Required]
    [StringLength(100)]
    public string Origin { get; set; } = null!;
    
    [Required]
    [StringLength(100)]
    public string Destination { get; set; } = null!;
    
    [Required]
    [Range(0, 9999999999.99)]
    public decimal Weight { get; set; }
    
    [Required]
    public DateTime ShippingDate { get; set; }
    
    public DateTime? DeliveryDate { get; set; }
    public ShipmentDTO ToDTO()
    {
        return new ShipmentDTO(TrackingNumber, Status, Origin, Destination, Weight, ShippingDate, DeliveryDate);
    }
    public Shipment(){
        
    }
    public Shipment(ShipmentDTO shipmentDTO)
    {
        TrackingNumber = shipmentDTO.TrackingNumber;
        Status = shipmentDTO.Status;
        Origin = shipmentDTO.Origin;
        Destination = shipmentDTO.Destination;
        Weight = shipmentDTO.Weight;
        ShippingDate = shipmentDTO.ShippingDate;
        DeliveryDate = shipmentDTO.DeliveryDate;
    }

    public void Update(ShipmentDTO shipmentDTO)
    {
        Status = shipmentDTO.Status;
        Origin = shipmentDTO.Origin;
        Destination = shipmentDTO.Destination;
        Weight = shipmentDTO.Weight;
        ShippingDate = shipmentDTO.ShippingDate;
        DeliveryDate = shipmentDTO.DeliveryDate;
    }
}
