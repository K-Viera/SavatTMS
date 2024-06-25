namespace TMSApi;

public class ShipmentDTO
{
    public string TrackingNumber { get; set; }
    public string Status { get; set; }
    public string Origin { get; set; }
    public string Destination { get; set; }
    public decimal Weight { get; set; }
    public DateTime ShippingDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public ShipmentDTO(string trackingNumber, string status, string origin, string destination, decimal weight, DateTime shippingDate, DateTime? deliveryDate)
    {
        TrackingNumber = trackingNumber;
        Status = status;
        Origin = origin;
        Destination = destination;
        Weight = weight;
        ShippingDate = shippingDate;
        DeliveryDate = deliveryDate;
    }
}
