using ETicaretAPI.Application.Abstractions.Token;
using ETicaretAPI.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.AppUser.FacebookLogin
{
	public class FacebookLoginCommandHandler : IRequestHandler<FacebookLoginCommandRequest, FacebookLoginCommandResponse>
	{
		readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
		readonly ITokenHandler _tokenHandler;
		readonly HttpClient _httpClient;

		public FacebookLoginCommandHandler(UserManager<Domain.Entities.Identity.AppUser> userManager, ITokenHandler tokenHandler, IHttpClientFactory httpClientFactory)
		{
			_userManager = userManager;
			_tokenHandler = tokenHandler;
			_httpClient = httpClientFactory.CreateClient();
		}

		public Task<FacebookLoginCommandResponse> Handle(FacebookLoginCommandRequest request, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
