using Microsoft.AspNetCore.Mvc;
using ThunderWings.Models;
using ThunderWings.Services;

namespace ThunderWings.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AircraftController : ControllerBase
    {
        private readonly IDataService _dataService;

        public AircraftController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddMultipleAircraft(List<Aircraft> aircrafts)
        {
            var count = await _dataService.AddMultipleAircraftInternal(aircrafts);
            return Ok($"Added {count} aircraft(s)");
        }

        [HttpPost]
        [Route("aircraftfiltered")]
        public async Task<PaginatedAircraftResponse> GetAircraftsFiltered([FromBody]GetAircraftsFilteredRequest filters, int page, int pageSize)
        {
            return await _dataService.GetAircraftsFilteredInternal(filters, page, pageSize);
        }
    }
}
