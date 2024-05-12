using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThunderWings.Models;
using ThunderWings.Services;

namespace ThunderWings.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AircraftController : ControllerBase
    {
        private readonly AircraftContext _aircraftContext;
        private readonly BasketContext _basketContext;
        private readonly IDataService _dataService;

        public AircraftController(AircraftContext aircraftContext, BasketContext basketContext, IDataService dataService)
        {
            _aircraftContext = aircraftContext;
            _basketContext = basketContext;
            _dataService = dataService;
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddMultipleAircraft(List<Aircraft> aircrafts)
        {
            var count = _dataService.AddMultipleAircraftInternal(aircrafts);
            return Ok($"Added {count} aircraft(s)");
        }

        [HttpPost]
        [Route("aircraftfiltered")]
        public async Task<PaginatedList<Aircraft>> GetAircraftsFiltered([FromBody]GetAircraftsFilteredRequest filters, int page, int pageSize)
        {
            var query = _aircraftContext.Aircrafts.AsQueryable().AsNoTracking();
            
            if (filters.FilterOptions == null && string.IsNullOrEmpty(filters.SortBy)) return await PaginatedList<Aircraft>.CreateAsync(query, page, pageSize);

            if (filters.FilterOptions != null)
            {
                foreach (var filter in filters.FilterOptions)
                {
                    switch (filter.Key.ToLower().Trim())
                    {
                        case "name":
                            query = query.Where(n => n.Name.ToLower().Trim().Contains(filter.Value.ToLower().Trim()));
                            break;
                        case "manufacturer":
                            query = query.Where(m =>
                                m.Manufacturer.ToLower().Trim().Contains(filter.Value.ToLower().Trim()));
                            break;
                        case "country":
                            query = query.Where(c =>
                                c.Country.ToLower().Trim().Contains(filter.Value.ToLower().Trim()));
                            break;
                        case "role":
                            query = query.Where(r => r.Role.ToLower().Trim().Contains(filter.Value.ToLower().Trim()));
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
            }

            if (!string.IsNullOrEmpty(filters.SortBy))
            {
                switch (filters.SortBy.ToLower().Trim())
                {
                    case "name":
                        var a = await PaginatedList<Aircraft>.Create(query.ToList().OrderBy(n => n.Name), page, pageSize);
                        return a;
                    case "manufacturer":
                        return await PaginatedList<Aircraft>.Create(query.ToList().OrderBy(m => m.Manufacturer), page, pageSize);
                    case "country":
                        return await PaginatedList<Aircraft>.Create(query.ToList().OrderBy(c => c.Country), page, pageSize);
                    case "role":
                        return await PaginatedList<Aircraft>.Create(query.ToList().OrderBy(r => r.Role), page, pageSize);
                    case "topspeed":
                        return await PaginatedList<Aircraft>.Create(query.ToList().OrderBy(t => t.TopSpeed), page, pageSize);
                    case "price":
                        return await PaginatedList<Aircraft>.Create(query.ToList().OrderBy(p => p.Price), page, pageSize);
                }
            }
            return await PaginatedList<Aircraft>.CreateAsync(query, page, pageSize);
        }

        [HttpPost]
        [Route("basket/add")]
        public async Task<ActionResult<AddToBasketResponse>> AddToBasket(List<int> ids)
        {
            return await _dataService.AddToBasketInternal(ids);
        }

        [HttpPost]
        [Route("basket/checkout")]
        public async Task<ActionResult<Invoice>> CheckoutBasket()
        {
            return await _dataService.CheckoutBasketInternal();
        }
        
        /// <summary>
        /// Since I used an in memory db, the idea here is that to persist the basket you would save the returned list of id's from here
        /// and save it back to the db the next time you run the app using the list and the "basket/add" endpoint (after repopulating the aircraft table of course)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("basket/save")]
        public async Task<ActionResult<IEnumerable<Basket>>> SaveBasket()
        {
            return await _basketContext.Basket.ToListAsync();
        }
    }
}
