using ETicaretAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Queries.Product.GetByIdProduct
{
	public class GetByIdProductQueryResponse
	{
		public string Name { get; set; }
		public int Stock { get; set; }
		public decimal Price { get; set; }
		//public ICollection<Order> Orders { get; set; }
		//public ICollection<ProductImageFile> ProductImageFiles { get; set; }
	}
}
