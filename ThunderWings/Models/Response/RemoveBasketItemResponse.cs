namespace ThunderWings.Models;

public class RemoveBasketItemResponse
{
    public List<int> RemovedAircraftIds { get; set; }
    public string FailedToRemove { get; set; }
}