﻿using ETicaretAPI.Application.Abstractions.Token;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Insfrastructure.Services.Token
{
	public class TokenHandler : ITokenHandler
	{
		readonly IConfiguration _configuration;

		public TokenHandler(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public Application.DTOs.Token CreateAccessToken(int second)
		{
			Application.DTOs.Token token = new();
			//Security Key'in simetriğini alıyoruz.
			SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_configuration["Token:SecurityKey"]));
			//Şifrelenmiş kimliği oluşturıyoruz.
			SigningCredentials signingCredentials = new(securityKey,SecurityAlgorithms.HmacSha256);
			//Oluşturulacak token ayarlarını veriyoruz.
			token.Expiration=DateTime.UtcNow.AddSeconds(second);
			JwtSecurityToken SecurityToken = new(
				audience: _configuration["Token:Audience"],
				issuer: _configuration["Token:Issuer"],
				expires:token.Expiration,
				notBefore:DateTime.UtcNow,
				signingCredentials:signingCredentials
				);
			//Token oluşturucu sınıfından bir örnek alıyoruz.
			JwtSecurityTokenHandler tokenHandler = new();
			token.AccessToken=tokenHandler.WriteToken(SecurityToken);	
			token.RefreshToken = CreateRefreshToken(); 
			return token;
		}

		public string CreateRefreshToken()
		{
			byte[] number = new byte[32];
			using RandomNumberGenerator  randım= RandomNumberGenerator.Create();
			randım.GetBytes(number);
			return Convert.ToBase64String(number);
		}
	}
}