using ETicaretAPI.Application.Features.Commands.Basket.AddItemToBasket;
using ETicaretAPI.Application.Features.Commands.Basket.RemoveBasketItem;
using ETicaretAPI.Application.Features.Commands.Basket.UpdateQuantity;
using ETicaretAPI.Application.Features.Queries.ProductImageFile.GetProductImages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(AuthenticationSchemes ="Admin")]
	public class BasketsController : ControllerBase
	{
		readonly IMediator _mediator;

		public BasketsController(IMediator mediator)
		{

			_mediator = mediator;
		}
		[HttpGet]
		public async Task <IActionResult> GetBasketItems([FromQuery]GetProductImagesQueryRequest getProductImagesQueryRequest)
		{
			List<GetProductImagesQueryResponse> result=await _mediator.Send(getProductImagesQueryRequest);
			return Ok(result);
		}
		[HttpPost]
		public async Task<IActionResult> AddItemToBasket(AddItemToBasketCommandRequest addItemToBasketCommandRequest)
		{
			AddItemToBasketCommandResponse response = await _mediator.Send(addItemToBasketCommandRequest);
			return Ok(response);
		}
		[HttpPut]
		public async Task<IActionResult> UpdateQuantity(UpdateQuantityCommandRequest updateQuantityCommandRequest)
		{
			UpdateQuantityCommandResponse response = await _mediator.Send(updateQuantityCommandRequest);
			return Ok(response);
		}
		[HttpDelete("{BasketItemId}")]
		public async Task<IActionResult> RemoveBasketItem([FromRoute]RemoveBasketItemCommandRequest removeBasketItemCommandRequest)
		{
			RemoveBasketItemCommandResponse response = await _mediator.Send(removeBasketItemCommandRequest);
			return Ok(response);
		}

	}
}
