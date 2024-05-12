namespace ThunderWings.Models;

public class RemoveBasketItemRequest
{
    public Dictionary<int, bool> ItemsToRemove { get; set; }
}