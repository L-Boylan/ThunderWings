namespace ThunderWings.Models;

public class PaginatedAircraftResponse
{
    public PaginatedList<Aircraft> Aircraft { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}