namespace TMSApi;

public class UpdateShipmentRequest
{
    public string Status { get; set; } = null!;
    public string Origin { get; set; } = null!;
    public string Destination { get; set; } = null!;
    public decimal Weight { get; set; }
    public DateTime ShippingDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
}
