using Microsoft.AspNetCore.Mvc;
using ThunderWings.Models;
using ThunderWings.Services;

namespace ThunderWings.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
public class BasketController : Controller
{
    private readonly IDataService _dataService;

    public BasketController(IDataService dataService)
    {
        _dataService = dataService;
    }
    
    [HttpPost]
    [Route("add")]
    public async Task<ActionResult<AddToBasketResponse>> AddToBasket(List<int> ids)
    {
        return await _dataService.AddToBasketInternal(ids);
    }
    
    /// <summary>
    /// The dictionary key refers to the aircraftId of you wish to remove,
    /// while the value is used to determine whether you want to delete one instance or all instances (true for all, false for one)
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("remove")]
    public async Task<ActionResult<RemoveBasketItemResponse>> RemoveBasketItem(RemoveBasketItemRequest request)
    {
        return await _dataService.RemoveBasketItemInternal(request);
    }
    
    [HttpPost]
    [Route("checkout")]
    public async Task<ActionResult<InvoiceResponse>> CheckoutBasket()
    {
        return await _dataService.CheckoutBasketInternal();
    }
}