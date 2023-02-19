using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Repositeries.File
{
	public class FİleWriteRepository : WriteRepository<Domain.Entities.File>, IFileWriteRepository
	{
		public FİleWriteRepository(ETicaretAPIDbContext context) : base(context)
		{
		}
	}
}
