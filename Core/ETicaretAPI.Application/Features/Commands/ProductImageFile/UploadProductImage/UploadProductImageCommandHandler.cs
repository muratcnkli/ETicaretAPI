using ETicaretAPI.Application.Abstractions.Storage;
using ETicaretAPI.Application.Repositories;
using P = ETicaretAPI.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.ProductImageFile.UploadProductImage
{
	public class UploadProductImageCommandHandler : IRequestHandler<UploadProductImageCommandRequest, UploadProductImageCommandResponse>
	{
		readonly IStorageService _storageService;
		readonly IProductReadRepository _productReadRepository;
		readonly IProductImageFileWriteRepository _productImageFileWriteRepository;

		public UploadProductImageCommandHandler(IProductImageFileWriteRepository productImageFileWriteRepository, IProductReadRepository productReadRepository = null, IStorageService storageService = null)
		{
			_productImageFileWriteRepository = productImageFileWriteRepository;
			_productReadRepository = productReadRepository;
			_storageService = storageService;
		}

		public async Task<UploadProductImageCommandResponse> Handle(UploadProductImageCommandRequest request, CancellationToken cancellationToken)
		{
			var datas = await _storageService.UploadAsync("files", request.Files);
			P.Product product = await _productReadRepository.GetByIdAsync(request.Id);
			await _productImageFileWriteRepository.AddRangeAsync(datas.Select(d => new P.ProductImageFile()
			{
				FileName = d.fileName,
				Path = d.pathOrContainerName,
				Storage = _storageService.StorageName,
				Products = new List<P.Product>() { product }
			}).ToList());
			await _productImageFileWriteRepository.SaveAsync();
			return new();
			
		}
	}
}
