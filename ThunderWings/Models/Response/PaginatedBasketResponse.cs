namespace ThunderWings.Models;

public class PaginatedBasketResponse
{
    public PaginatedList<Basket> BasketItems { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}