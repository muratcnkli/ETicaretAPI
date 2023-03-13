using ETicaretAPI.Application.Abstractions.Token;
using ETicaretAPI.Application.DTOs.Facebook;
using ETicaretAPI.Domain.Entities.Identity;
using Google.Apis.Auth.OAuth2;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ETicaretAPI.Application.DTOs;

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

		public async Task<FacebookLoginCommandResponse> Handle(FacebookLoginCommandRequest request, CancellationToken cancellationToken)
		{
			string accessTokenResponse = await _httpClient.GetStringAsync("https://graph.facebook.com/oauth/access_token?client_id=586135356888365&client_secret=e690851992849d277b16e4b14d59b562&grant_type=client_credentials");
			FacebookAccessTokenResponse_DTO facebookAccessTokenResponse =
				JsonSerializer.Deserialize<FacebookAccessTokenResponse_DTO>(accessTokenResponse);
			string userAccessTokenValidation= await _httpClient.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={request.AuthToken}&access_token={facebookAccessTokenResponse.AccessToken}");
			FacebokkAccessTokenValidation validation = 
				JsonSerializer.Deserialize<FacebokkAccessTokenValidation>(userAccessTokenValidation);
			if (validation.Data.IsValid)
			{
				string userInfoReponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/me?fields=email,name&access_token={request.AuthToken}");
				FacebookUserInfoResponse userInfo=JsonSerializer.Deserialize<FacebookUserInfoResponse>(userInfoReponse);

				var info = new UserLoginInfo("FACEBOOK", validation.Data.UserId, "FACEBOOK");
				var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
				bool result = user != null;
				if (user == null)
				{
					user = await _userManager.FindByEmailAsync(userInfo.Email);
					if (user == null)
					{
						user = new()
						{
							Id = Guid.NewGuid().ToString(),
							Email = userInfo.Email,
							NameSurname = userInfo.Name,

						};
						var identityResult = await _userManager.CreateAsync(user);
						result = identityResult.Succeeded;
					}
				}
				if (result)
				{
					await _userManager.AddLoginAsync(user, info);
					Token token = _tokenHandler.CreateAccessToken(5);
					return new()
					{
						Token=token,
					};
				}
			}
			throw new Exception("Inlavid external authentication");
		}
		
	}
}
