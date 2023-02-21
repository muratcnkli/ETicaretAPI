using ETicaretAPI.Application.Abstractions.Storage;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.RequestParameters;
using ETicaretAPI.Application.ViewModels.Products;
using ETicaretAPI.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

		public ProductsController(IProductWriteRepository productWriteRepository,
			IProductReadRepository productReadRepository,
			IWebHostEnvironment webHostEnvironment,
			IFileWriteRepository fileWriteRepository,
			IFileReadRepository fileReadRepository,
			IProductImageFileReadRepository productImageFileReadRepository,
			IProductImageFileWriteRepository productImageFileWriteRepository,
			IInvoiceFileReadRepository ınvoiceFileReadRepository,
			IInvoiceFileWriteRepository ınvoiceFileWriteRepository,
			IStorageService storageService)
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
		}
		[HttpGet]
		public async Task<IActionResult> Get([FromQuery]Pagination pagination)
		{
			var totalCount=_productReadRepository.GetAll(false).Count();
			var products= _productReadRepository.GetAll(false)
				.Skip(pagination.Page * pagination.Size)
				.Take(pagination.Size).Select(p => new
			{
				p.Id,
				p.Name,
				p.Stock,
				p.Price,
				p.CreatedDate,
				p.UpdatedDate
			}).ToList();

			return Ok(new
			{
				totalCount,
				products,
			});
		}
		[HttpGet("{id}")]
		public async Task<IActionResult> Get(string id)
		{
			return Ok(_productReadRepository.GetByIdAsync(id,false));
		}

		[HttpPost]
		public async Task<IActionResult> Post(VM_Create_Product model)
		{
			_productWriteRepository.AddAsync(new()
			{
				Name=model.Name,
				Price=model.Price,
				Stock=model.Stock	
			});
			await _productWriteRepository.SaveAsync();
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

	}
}
