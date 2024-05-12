namespace ThunderWings.Models;

public class GetAircraftsFilteredRequest
{
    public string SortBy { get; set; }
    public Dictionary<string, string>? FilterOptions { get; set; }
}