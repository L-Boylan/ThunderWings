namespace ThunderWings.Models;

public class AddToBasketResponse
{
    public List<Aircraft> AddedAircraft { get; set; }
    public string FailedToAdd { get; set; }
}