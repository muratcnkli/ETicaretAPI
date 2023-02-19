using ETicaretAPI.Application.Abstractions;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Persistence.Concretes;
using ETicaretAPI.Persistence.Contexts;
using ETicaretAPI.Persistence.Repositeries;
using ETicaretAPI.Persistence.Repositeries.File;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceServices(this IServiceCollection services)
        {
			//services.AddSingleton<IProductService, ProductService>();
			services.AddDbContext<ETicaretAPIDbContext>(options =>options.UseNpgsql(Configuration.ConnectionString));
			
			services.AddScoped<ICustomerReadRepository, CustomerReadRepository>();
            services.AddScoped<ICustomerWriteRepository, CustomerWriteRepository>();
            services.AddScoped<IOrderReadRepository, OrderReadRepository>();
            services.AddScoped<IOrderWriteRepository, OrderWriteRepository>(); 
            services.AddScoped<IProductReadRepository, ProductReadRepository>();
			services.AddScoped<IProductWriteRepository, ProductWriteRepository>();
			services.AddScoped<IProductImageFileReadRepository, ProductImageFileReadRepository>();
			services.AddScoped<IProductImageFileWriteRepository, ProductImageFileWriteRepository>();
			services.AddScoped<IInvoiceFileReadRepository, InvoiceFileReadRepository>();
			services.AddScoped<IInvoiceFileWriteRepository, InvoiceFileWriteRepository>();
			services.AddScoped<IFileReadRepository, FİleReadRepository>();
			services.AddScoped<IFileWriteRepository, FİleWriteRepository>();
		}
    }
}
 