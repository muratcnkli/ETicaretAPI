using ETicaretAPI.Application.DTOs;

namespace ETicaretAPI.Application.Features.Commands.AppUser.LoginUser
{
	public class LoginUserSuccessCommandResponse: LoginUserCommandResponse
	{
		public Token Token { get; set; }
	}

}
