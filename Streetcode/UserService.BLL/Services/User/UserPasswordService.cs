using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.BLL.DTO.PublishDtos;
using UserService.BLL.DTO.User;
using UserService.BLL.Interfaces.Azure;
using UserService.BLL.Interfaces.User;
using Newtonsoft.Json;
using UserEntity = UserService.DAL.Entities.Users.User;

namespace UserService.BLL.Services.User
{
    public class UserPasswordService : IUserPasswordService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IAzureServiceBus _bus;

        public UserPasswordService(UserManager<UserEntity> userManager, IAzureServiceBus bus)
        {
            _userManager = userManager;
            _bus = bus;
        }

        public async Task<Result> ChangePassword(PassChangeDto passChangeDto, string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return Result.Fail("User does not exist!");

            if (passChangeDto.Password != passChangeDto.PasswordConfirm)
            {
                return Result.Fail("Password and Confirm Password fields do not match.");
            }

            if (!await _userManager.CheckPasswordAsync(user, passChangeDto.OldPassword))
            {
                return Result.Fail("Invalid Old Password credential.");
            }

            var changePassword = await _userManager.ChangePasswordAsync(user, passChangeDto.OldPassword, passChangeDto.Password);

            if (!changePassword.Succeeded)
            {
                return Result.Fail("Cannot change password for user");
            }

            var messagePublishDto = new EmailMessagePublishDto
            {
                From = "Streetcode",
                To = user.Email,
                Content = "You succesfully changed your password. If that was not you, than contact our administration:{FrontEnd link to Contacting Administration}",
                Subject = "ChangePassword"
            };

            var objAsText = JsonConvert.SerializeObject(messagePublishDto);

            await _bus.SendMessage("emailQueue", objAsText);

            return Result.Ok();
        }

        public async Task<Result> ForgotPassword(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return Result.Fail("User does not exist!");

                var code = await _userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);

                var callbackUrl = $"https://frontend-domain/password-reset-page/{email}/{code}";
                var messagePublishDto = new EmailMessagePublishDto
                {
                    From = "Streetcode",
                    To = user.Email,
                    Content = callbackUrl,
                    Subject = "ResetPassword"
                };

                var objAsText = JsonConvert.SerializeObject(messagePublishDto);

                await _bus.SendMessage("emailQueue", objAsText);
                return Result.Ok();
            }
            catch (Exception e)
            {
                return Result.Fail(e.Message);
            }
        }

        public async Task<Result> ResetPassword(PassResetDto passResetDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(passResetDto.Email);
                if (user == null)
                {
                    return Result.Fail("You do not have Permision!");
                }
                var result = await _userManager.ResetPasswordAsync(user, passResetDto.Code, passResetDto.Password);
                if (!result.Succeeded)
                {
                    return Result.Fail("You do not have Permision!");
                }

                var messagePublishDto = new EmailMessagePublishDto
                {
                    From = "Streetcode",
                    To = user.Email,
                    Content = "You succesfully changed your password. If that was not you, than contact our administration:{FrontEnd link to Contacting Administration}",
                    Subject = "ResetPassword"
                };

                var objAsText = JsonConvert.SerializeObject(messagePublishDto);

                await _bus.SendMessage("emailQueue", objAsText);
                return Result.Ok();
            }
            catch (Exception e)
            {
                return Result.Fail(e.Message);
            }
        }
    }
}
