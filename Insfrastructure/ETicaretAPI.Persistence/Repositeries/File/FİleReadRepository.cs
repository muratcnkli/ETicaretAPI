using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Repositeries.File
{
	public class FİleReadRepository : ReadRepository<Domain.Entities.File>, IFileReadRepository
	{
		public FİleReadRepository(ETicaretAPIDbContext context) : base(context)
		{
		}
	}
}
