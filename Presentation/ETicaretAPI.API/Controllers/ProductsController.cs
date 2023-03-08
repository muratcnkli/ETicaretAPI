using ETicaretAPI.Application.Abstractions.Storage;
using ETicaretAPI.Application.Features.Commands.Product.CreateProduct;
using ETicaretAPI.Application.Features.Queries.Product.GetAllProduct;
using ETicaretAPI.Application.Features.Queries.Product.GetByIdProduct;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.RequestParameters;
using ETicaretAPI.Application.ViewModels.Products;
using ETicaretAPI.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
	public class ProductsController : ControllerBase
	{
		private readonly IProductWriteRepository _productWriteRepository;
		private readonly IProductReadRepository _productReadRepository;
		private readonly IWebHostEnvironment _webHostEnvironment;		
		private readonly IFileWriteRepository _fileWriteRepository;
		private readonly IFileReadRepository _fileReadRepository;
		private readonly IProductImageFileReadRepository _productImageFileReadRepository;
		private readonly IProductImageFileWriteRepository _productImageFileWriteRepository;
		private readonly IInvoiceFileReadRepository _ınvoiceFileReadRepository;
		private readonly IInvoiceFileWriteRepository _ınvoiceFileWriteRepository;
		private readonly IStorageService _storageService;
		private readonly IConfiguration _configuration;
		private readonly IMediator _mediator;

		public ProductsController(IProductWriteRepository productWriteRepository,
			IProductReadRepository productReadRepository,
			IWebHostEnvironment webHostEnvironment,
			IFileWriteRepository fileWriteRepository,
			IFileReadRepository fileReadRepository,
			IProductImageFileReadRepository productImageFileReadRepository,
			IProductImageFileWriteRepository productImageFileWriteRepository,
			IInvoiceFileReadRepository ınvoiceFileReadRepository,
			IInvoiceFileWriteRepository ınvoiceFileWriteRepository,
			IStorageService storageService,
			IConfiguration configuration,
			IMediator mediator)
		{
			_productWriteRepository = productWriteRepository;
			_productReadRepository = productReadRepository;
			_webHostEnvironment = webHostEnvironment;
			_fileWriteRepository = fileWriteRepository;
			_fileReadRepository = fileReadRepository;
			_productImageFileReadRepository = productImageFileReadRepository;
			_productImageFileWriteRepository = productImageFileWriteRepository;
			_ınvoiceFileReadRepository = ınvoiceFileReadRepository;
			_ınvoiceFileWriteRepository = ınvoiceFileWriteRepository;
			_storageService = storageService;
			_configuration = configuration;
			_mediator = mediator;
		}
		[HttpGet]
		public async Task<IActionResult> Get([FromQuery] GetAllProductQueryRequest getAllProductQueryRequest)
		{
			GetAllProductQueryResponse response= await _mediator.Send(getAllProductQueryRequest);
			return Ok(response);
			
		}
		[HttpGet("{Id}")]
		public async Task<IActionResult> Get([FromRoute] GetByIdProductQueryRequest getByIdProductQueryRequest)
		{
			GetByIdProductQueryResponse response = await _mediator.Send(getByIdProductQueryRequest);
			return Ok(response);
		}

		[HttpPost]
		public async Task<IActionResult> Post(CreateProductCommandRequest createProductCommandRequest)
		{
			CreateProductCommandResponse response= await _mediator.Send(createProductCommandRequest);
			return StatusCode((int)HttpStatusCode.Created);
		}
		[HttpPut]
		public async Task<IActionResult> Put(VM_Update_Product model)
		{
			var product= await _productReadRepository.GetByIdAsync(model.Id);
			product.Name=model.Name;
			product.Price=model.Price;
			product.Stock=model.Stock;
			await _productWriteRepository.SaveAsync();
			return Ok();
		}
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(string id)
		{
			await _productWriteRepository.RemoveAsync(id);
			await _productWriteRepository.SaveAsync();
			return Ok();

		}
		[HttpPost("[action]")]
		public async Task<IActionResult> Upload(string id)
		{
			var datas = await _storageService.UploadAsync("files", Request.Form.Files);
			Product product = await _productReadRepository.GetByIdAsync(id);
			await _productImageFileWriteRepository.AddRangeAsync(datas.Select(d => new ProductImageFile()
			{
				FileName = d.fileName,
				Path = d.pathOrContainerName,
				Storage=_storageService.StorageName,
				Products=new List<Product>() { product}				 
			}).ToList());
			await _productImageFileWriteRepository.SaveAsync();
			return Ok();
		}
		[HttpGet("[action]/{id}")]
		public async Task<IActionResult> GetProductImages(string id)
		{
			Product? product=await _productReadRepository.Table.Include(p=>p.ProductImageFiles)
				.FirstOrDefaultAsync(p=>p.Id==Guid.Parse(id));
			
			return Ok(product.ProductImageFiles.Select(p => new
			{
				Path = $"{_configuration["BaseStorageUrl"]}/{p.Path}",
				p.FileName
			}));
		}
		[HttpDelete("[action]/{id}")]
		public async Task<IActionResult> DeleteProductImage(string id,string imageId)
		{
			Product? product = await _productReadRepository.Table.Include(p=>p.ProductImageFiles)
				.FirstOrDefaultAsync(p=>p.Id==Guid.Parse(id));
			ProductImageFile productImageFile=product.ProductImageFiles.FirstOrDefault(p=>p.Id==Guid.Parse(imageId));
			product.ProductImageFiles.Remove(productImageFile);
			await _productWriteRepository.SaveAsync();

			return Ok();
		}

	}
}
