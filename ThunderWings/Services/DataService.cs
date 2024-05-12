using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThunderWings.Models;

namespace ThunderWings.Services;

public class DataService : IDataService
{
    private readonly AircraftContext _aircraftContext;
    private readonly BasketContext _basketContext;

    public DataService(AircraftContext aircraftContext, BasketContext basketContext)
    {
        _aircraftContext = aircraftContext;
        _basketContext = basketContext;
    }
    
    public async Task<int> AddMultipleAircraftInternal(List<Aircraft> aircrafts)
    {
        foreach (var aircraft in aircrafts)
        {
            _aircraftContext.Aircrafts.Add(aircraft);
        }

        return await _aircraftContext.SaveChangesAsync();
    }

    public async Task<PaginatedAircraftResponse> GetAircraftsFilteredInternal(GetAircraftsFilteredRequest filtersRequest,
        int page, int pageSize)
    {
        var response = new PaginatedAircraftResponse();
        
        var query = _aircraftContext.Aircrafts.AsQueryable().AsNoTracking();

        if (filtersRequest.FilterOptions == null && string.IsNullOrEmpty(filtersRequest.SortBy))
        {
            response.Aircraft = await PaginatedList<Aircraft>.CreateQueryableAsync(query, page, pageSize);
            response.CurrentPage = response.Aircraft.PageIndex;
            response.TotalPages = response.Aircraft.TotalPages;
            return response;
        }

        if (filtersRequest.FilterOptions != null)
        {
            query = AddFilters(filtersRequest, query);
        }

        if (!string.IsNullOrEmpty(filtersRequest.SortBy))
        {
            response.Aircraft = await SortResponse(query, filtersRequest.SortBy, page, pageSize);
            response.CurrentPage = response.Aircraft.PageIndex;
            response.TotalPages = response.Aircraft.TotalPages;
            return response;
        }

        response.Aircraft = await PaginatedList<Aircraft>.CreateQueryableAsync(query, page, pageSize);
        response.CurrentPage = response.Aircraft.PageIndex;
        response.TotalPages = response.Aircraft.TotalPages;
        return response;
    }

    

    public async Task<AddToBasketResponse> AddToBasketInternal(List<int> ids)
    {
        var addToBasketResponse = new AddToBasketResponse
        {
            AddedAircraft = new List<Aircraft>(),
            FailedToAdd = ""
        };
        
        var failedToAdd = new List<int>();
        
        foreach (var id in ids)
        {
            var aircraft = await _aircraftContext.Aircrafts.FirstOrDefaultAsync(a => a.Id == id);
                
            if (aircraft == null) failedToAdd.Add(id);
            else
            {
                addToBasketResponse.AddedAircraft.Add(aircraft);
                await _basketContext.Basket.AddAsync(new Basket{AircraftId = id});
            }
        }
        
        await _basketContext.SaveChangesAsync();
        
        if (failedToAdd.Count > 0)
        {
            addToBasketResponse.FailedToAdd = "Failed to add Id(s): ";
            foreach (var id in failedToAdd)
            {
                addToBasketResponse.FailedToAdd += $"{id}, ";
            }
        }

        return addToBasketResponse;
    }

    public async Task<RemoveBasketItemResponse> RemoveBasketItemInternal(RemoveBasketItemRequest request)
    {
        var response = new RemoveBasketItemResponse
        {
            RemovedAircraftIds = new List<int>()
        };
        var failedToRemove = "";

        foreach (var item in request.ItemsToRemove)
        {
            if (item.Value)
            {
                var itemsToDelete = _basketContext.Basket.AsQueryable().Where(a => a.AircraftId == item.Key);
                
                if (itemsToDelete.Count() <= 0)
                {
                    failedToRemove += $"{item.Key}, ";
                }
                else
                {
                    foreach (var itemToDelete in itemsToDelete)
                    {
                        response.RemovedAircraftIds.Add(itemToDelete.AircraftId);
                        _basketContext.Remove(itemToDelete);
                    }
                }
            }
            else
            {
                var itemToRemove = await _basketContext.Basket.FirstOrDefaultAsync(a => a.AircraftId == item.Key);
                if (itemToRemove == null)
                {
                    failedToRemove += $"{item.Key}, ";
                }
                else
                {
                    response.RemovedAircraftIds.Add(itemToRemove.AircraftId);
                    _basketContext.Remove(itemToRemove);
                }
            }
        }
        await _basketContext.SaveChangesAsync();

        if (!string.IsNullOrEmpty(failedToRemove))
        {
            response.FailedToRemove = $"Failed to remove Ids: {failedToRemove}\nBasket did not contain elements with these ids";
            return response;
        }
        
        return response;
    }

    public async Task<InvoiceResponse> CheckoutBasketInternal()
    {
        var basket = await _basketContext.Basket.ToListAsync().ConfigureAwait(false);
        var invoice = new InvoiceResponse();
        var purchasedItems = new List<Aircraft>();
            
        if (basket.Count <= 0)
        {
            invoice.Message = "No items in basket";
            return invoice;
        }

        foreach (var item in basket)
        {
            var pa = _aircraftContext.Aircrafts.FirstOrDefault(a => a.Id == item.AircraftId);
            if (pa != null)
            {
                invoice.TotalPrice += (ulong)pa.Price;
                purchasedItems.Add(pa);
            }
        }

        invoice.PurchasedAircrafts = purchasedItems;
        
        // ToDo clear basket
        return invoice;
    }

    public async Task<PaginatedBasketResponse> ViewBasketInternal(int page, int pageSize)
    {
        var response = new PaginatedBasketResponse();
        var basketItems = await _basketContext.Basket.ToListAsync();

        response.BasketItems = await PaginatedList<Basket>.CreateEnumerableAsync(basketItems, page, pageSize);
        response.CurrentPage = response.BasketItems.PageIndex;
        response.TotalPages = response.BasketItems.TotalPages;
        
        return response;
    }
    
    private IQueryable<Aircraft> AddFilters(GetAircraftsFilteredRequest filtersRequest, IQueryable<Aircraft> query)
         {
             foreach (var filter in filtersRequest.FilterOptions)
             {
                 switch (filter.Key.ToLower().Trim())
                 {
                     case "name":
                         query = query.Where(n => n.Name != null && n.Name.ToLower().Trim().Contains(filter.Value.ToLower().Trim()));
                         break;
                     case "manufacturer":
                         query = query.Where(m =>
                             m.Manufacturer != null && m.Manufacturer.ToLower().Trim().Contains(filter.Value.ToLower().Trim()));
                         break;
                     case "country":
                         query = query.Where(c =>
                             c.Country != null && c.Country.ToLower().Trim().Contains(filter.Value.ToLower().Trim()));
                         break;
                     case "role":
                         query = query.Where(r => r.Role != null && r.Role.ToLower().Trim().Contains(filter.Value.ToLower().Trim()));
                         break;
                     case "topspeed":
                         int topspeed;
                         int.TryParse(filter.Value, out topspeed);
                         query = query.Where(t => t.TopSpeed == topspeed);
                         break;
                     case "price":
                         int price;
                         int.TryParse(filter.Value, out price);
                         query = query.Where(p => p.Price == price);
                         break;
                 }
             }
     
             return query;
         }

    private async Task<PaginatedList<Aircraft>> SortResponse(IQueryable<Aircraft> query, string sortBy, int page, int pageSize)
    {
        switch (sortBy)
        {
            case "name":
                return await PaginatedList<Aircraft>.CreateOrderedEnumerableAsync(query.ToList().OrderBy(n => n.Name),
                    page, pageSize);
            case "manufacturer":
                return await PaginatedList<Aircraft>.CreateOrderedEnumerableAsync(
                    query.ToList().OrderBy(m => m.Manufacturer), page,
                    pageSize);
            case "country":
                return await PaginatedList<Aircraft>.CreateOrderedEnumerableAsync(
                    query.ToList().OrderBy(c => c.Country), page, pageSize);
            case "role":
                return await PaginatedList<Aircraft>.CreateOrderedEnumerableAsync(query.ToList().OrderBy(r => r.Role),
                    page, pageSize);
            case "topspeed":
                return await PaginatedList<Aircraft>.CreateOrderedEnumerableAsync(
                    query.ToList().OrderBy(t => t.TopSpeed), page,
                    pageSize);
            case "price":
                return await PaginatedList<Aircraft>.CreateOrderedEnumerableAsync(query.ToList().OrderBy(p => p.Price),
                    page, pageSize);
            default:
                return await PaginatedList<Aircraft>.CreateQueryableAsync(query, page, pageSize);
        }
    }
}