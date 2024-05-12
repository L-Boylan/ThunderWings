using Microsoft.AspNetCore.Mvc;
using ThunderWings.Models;

namespace ThunderWings.Services;

public interface IDataService
{
    Task<int> AddMultipleAircraftInternal(List<Aircraft> aircraft);
    Task<PaginatedAircraftResponse> GetAircraftsFilteredInternal(GetAircraftsFilteredRequest filtersRequest, int page, int pageSize);
    Task<AddToBasketResponse> AddToBasketInternal(List<int> ids);
    Task<RemoveBasketItemResponse> RemoveBasketItemInternal(RemoveBasketItemRequest request);
    Task<InvoiceResponse> CheckoutBasketInternal();
    Task<PaginatedBasketResponse> ViewBasketInternal(int page, int pageSize);
}