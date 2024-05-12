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

    public Task<PaginatedList<Aircraft>> GetAircraftsFilteredInternal()
    {
        throw new NotImplementedException();
    }

    public async Task<AddToBasketResponse> AddToBasketInternal(List<int> ids)
    {
        // ToDo allow for multiple of the same aircraft
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

    public Task RemoveFromBasketInternal()
    {
        throw new NotImplementedException();
    }

    public async Task<Invoice> CheckoutBasketInternal()
    {
        var basket = await _basketContext.Basket.ToListAsync().ConfigureAwait(false);
        var invoice = new Invoice();
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
                invoice.TotalPrice += pa.Price;
                purchasedItems.Add(pa);
            }
        }

        invoice.PurchasedAircrafts = purchasedItems;
        
        // ToDo clear basket
        return invoice;
    }
}