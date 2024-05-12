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
    
    [HttpPost]
    [Route("remove")]
    public async Task<ActionResult<RemoveBasketItemResponse>> RemoveBasketItem(RemoveBasketItemRequest request)
    {
        return await _dataService.RemoveBasketItemInternal(request);
    }

    [HttpGet]
    [Route("view")]
    public async Task<ActionResult<PaginatedBasketResponse>> ViewBasket(int page, int pageSize)
    {
        return await _dataService.ViewBasketInternal(page, pageSize);
    }
    
    [HttpPost]
    [Route("checkout")]
    public async Task<ActionResult<InvoiceResponse>> CheckoutBasket()
    {
        return await _dataService.CheckoutBasketInternal();
    }
}