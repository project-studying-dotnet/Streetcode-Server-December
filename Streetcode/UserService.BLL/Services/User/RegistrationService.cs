using System.Threading.Tasks;
using AutoMapper;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UserService.BLL.DTO.Users;
using UserService.BLL.DTO.User;
using UserService.BLL.Interfaces.User;
using UserEntity = UserService.DAL.Entities.Users.User;
using UserService.BLL.Interfaces.Azure;
using MongoDB.Bson.IO;
using System;
using UserService.BLL.DTO.PublishDtos;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace UserService.BLL.Services.User;

public class RegistrationService : IUserService
{
    private readonly UserManager<UserEntity> _userManager;
    private readonly IMapper _mapper;
    private readonly ILogger<RegistrationService> _logger;
    private readonly IAzureServiceBus _bus;
    private readonly IConfiguration _config;

    public RegistrationService(
        UserManager<UserEntity> userManager,
        IMapper mapper,
        ILogger<RegistrationService> logger,
        IAzureServiceBus bus,
        IConfiguration config)
    {
        _userManager = userManager;
        _mapper = mapper;
        _logger = logger;
        _bus = bus;
        _config = config;
    }
    public async Task<Result<UserDto>> Registration(RegistrationDto registrationDto)
    {

        if (registrationDto.Password != registrationDto.PasswordConfirm)
        {
            const string errMsg = "Password isn't equal";
            _logger.LogWarning(errMsg);
            return Result.Fail(errMsg);
        }

        var user = _mapper.Map<RegistrationDto, UserEntity>(registrationDto);

        if (user is null)
        {
            const string errMsg = "Cannot convert null to user";
            _logger.LogWarning(errMsg);
            return Result.Fail(errMsg);
        }

        var create = await _userManager.CreateAsync(user, registrationDto.Password);

        if (!create.Succeeded)
        {
            const string errMsg = "Cannot create user";
            _logger.LogWarning(errMsg);
            return Result.Fail(errMsg);
        }

        var newUser = await _userManager.FindByIdAsync(user.Id.ToString());
        if (newUser is null)
        {
            const string errMsg = "Cannot find user in db";
            _logger.LogWarning(errMsg);
            return Result.Fail(errMsg);
        }

        var assignRole = await _userManager.AddToRoleAsync(newUser, "User");
        if (!assignRole.Succeeded)
        {
            const string errMsg = "Cannot assign role to user";
            _logger.LogWarning(errMsg);
            return Result.Fail(errMsg);
        }

        // Generate a unique confirmation token
        var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        // Sending a message to Service Bus for email
        var senderEmail = _config["Email:From"];
        var confirmationUrl = $"{_config["Email:ConfirmationUrl"]}?userId={user.Id}&token={Uri.EscapeDataString(emailToken)}";

        var emailMessage = new EmailMessagePublishDto
        {
            To = registrationDto.Email,
            From = senderEmail,
            Subject = "Confirm your email",
            Content = $"Please confirm your email by clicking the link: " + $"{confirmationUrl}"
        };

        var message = Newtonsoft.Json.JsonConvert.SerializeObject(emailMessage);
        await _bus.SendMessage("emailQueue", message);

        _logger.LogInformation("Confirmation email sent to {Email}", registrationDto.Email);
        return Result.Ok(_mapper.Map<UserEntity, UserDto>(user));
    }
}