﻿using Azure.Core;
using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.DTOs.User;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Application.Features.Commands.AppUser.CreateUser;
using ETicaretAPI.Application.Helpers;
using ETicaretAPI.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Services
{
	public class UserService : IUserService
	{
		readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;

		public UserService(UserManager<AppUser> usermanager)
		{
			_userManager = usermanager;
		}

		public async Task<CreateUserResponse> CreateAsync(CreateUser model)
		{
			IdentityResult result = await _userManager.CreateAsync(new()
			{
				Id = Guid.NewGuid().ToString(),
				UserName = model.Username,
				Email = model.Email,
				NameSurname = model.NameSurName,

			}, model.Password);
			CreateUserResponse response = new() { Succeeded = result.Succeeded };
			if (result.Succeeded)
				response.Message = "Kullanıcı başarıyla oluşturulmuştur.";
			else
				foreach (var error in result.Errors)
					response.Message += $"{error.Code}-{error.Description}";
			return response;
		}

		public async Task UpdateRefreshToken(string refreshToken,AppUser user, DateTime accessTokenDate,int addOnAccessTokenDate)
		{
			if (user != null) 
			{
				user.RefreshToken = refreshToken;
				user.RefreshTokenEndDate = accessTokenDate.AddSeconds(addOnAccessTokenDate);
				await _userManager.UpdateAsync(user);
			}
			else
				throw new NotFoundUserException();
		}
		public async Task UpdatePasswordAsync(string userId, string resetToken, string newPassword)
		{
			AppUser user=await _userManager.FindByIdAsync(userId);
			if (user != null)
			{
				resetToken = resetToken.UrlDecode();
				IdentityResult result=await _userManager.ResetPasswordAsync(user,resetToken,newPassword);
				if (result.Succeeded)
				{
					await _userManager.UpdateSecurityStampAsync(user);
				}
				else
				{
					throw new PasswordChangeFailedException();
				}
			}
		}
	}
}
