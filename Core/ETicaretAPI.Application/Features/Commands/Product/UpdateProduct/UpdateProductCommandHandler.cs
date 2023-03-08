using ETicaretAPI.Application.Repositories;
using P= ETicaretAPI.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.Product.UpdateProduct
{
	public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommandRequest, UpdateProductCommandResponse>
	{
		IProductReadRepository _productReadRepository;
		IProductWriteRepository _productWriteRepository;

		public UpdateProductCommandHandler(IProductReadRepository productReadRepository, IProductWriteRepository productWriteRepository = null)
		{
			_productReadRepository = productReadRepository;
			_productWriteRepository = productWriteRepository;
		}

		public async Task<UpdateProductCommandResponse> Handle(UpdateProductCommandRequest request, CancellationToken cancellationToken)
		{
			P.Product product = await _productReadRepository.GetByIdAsync(request.Id);
			product.Name = request.Name;
			product.Price = request.Price;
			product.Stock = request.Stock;
			await _productWriteRepository.SaveAsync();
			return new();
		}
	}
}
