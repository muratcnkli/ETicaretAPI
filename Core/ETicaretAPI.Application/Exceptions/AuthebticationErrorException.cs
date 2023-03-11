using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Exceptions
{
	public class AuthebticationErrorException : Exception
	{
		public AuthebticationErrorException() : base("Kimlik doğrulama hatası.") 
		{
		}

		public AuthebticationErrorException(string? message) : base(message)
		{
		}

		public AuthebticationErrorException(string? message, Exception? innerException) : base(message, innerException)
		{
		}
	}
}
