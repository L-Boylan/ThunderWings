using Microsoft.AspNetCore.Mvc;
using ThunderWings.Models;

namespace ThunderWings.Services;

public interface IDataService
{
    Task<int> AddMultipleAircraftInternal(List<Aircraft> aircraft);
    Task<PaginatedList<Aircraft>> GetAircraftsFilteredInternal();
    Task<AddToBasketResponse> AddToBasketInternal(List<int> ids);
    Task RemoveFromBasketInternal();
    Task<Invoice> CheckoutBasketInternal();
    // Consider whether to bother with "savebasket"
}