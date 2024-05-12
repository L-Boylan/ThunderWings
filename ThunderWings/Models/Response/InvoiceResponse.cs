namespace ThunderWings.Models;

public class InvoiceResponse
{
    public List<Aircraft>? PurchasedAircrafts { get; set; }
    public ulong TotalPrice { get; set; }
    public string Message { get; set; }
}