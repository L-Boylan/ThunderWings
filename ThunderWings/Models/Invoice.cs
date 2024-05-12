namespace ThunderWings.Models;

public class Invoice
{
    public List<Aircraft> PurchasedAircrafts { get; set; }
    public int TotalPrice { get; set; }
}