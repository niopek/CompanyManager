using CompanyManager.API.Library.Common;
using CompanyManager.Library.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CompanyManager.API.Library.Services;

public class UsersService
{
    private readonly Database _database;
    private readonly IConfiguration _configuration;

    public UsersService(Database database, IConfiguration configuration)
    {
        _database = database;
        _configuration = configuration;
    }

    public async Task<int> CreateUser(string username, string email, string password)
    {
        // Generowanie Salt i Hash dla hasła
        using var hmac = new HMACSHA512();
        var salt = hmac.Key;
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        // Wywołanie procedury składowanej
        var result = await _database.SaveDataSP("CreateUser", new
        {
            Username = username,
            Email = email,
            PasswordHash = hash,
            PasswordSalt = salt
        });

        return result;
    }

    public async Task<UserModel?> ValidateLogin(string username, string password)
    {
        try
        {
            var salt = await _database.LoadDataAsyncSP<byte[], dynamic>(
                "GetUserSalt",
                new { Username = username });

            var pickedSalt = salt.FirstOrDefault();

            if (pickedSalt == null)
            {
                return null;
            }

            byte[]? hash = null;

            using (var hmac = new HMACSHA512(pickedSalt))
            {
                hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            if(hash == null)
            {
                return null;
            }
              
            var result = await _database.LoadDataAsyncSP<UserModel, dynamic>(
                "ValidateUserLogin",
                new { Username = username, Password = hash }
                );

            var user = result.FirstOrDefault();

            if(user == null)
            {
                return null;//exceptiony zamiast null
            }

            // Generowanie tokenu
            var token = GenerateToken(user);

            if(token == null)
            {
                return null;
            }

            user.Token = token;
            // Przechowaj token w bazie
            //await StoreToken(token, user.UserId);

            return user; 
        }
        catch
        {
            return null; 
        }
    }

    public string? GenerateToken(UserModel user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtKey = _configuration["Jwt:Key"];
        
        if(jwtKey == null)
        {
            return null;
        }

        var key = Encoding.UTF8.GetBytes(jwtKey); // Klucz z konfiguracji
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            }),
            Expires = DateTime.UtcNow.AddMinutes(30), // Czas ważności tokenu
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task StoreToken(string token, int userId)
    {
        await _database.SaveDataSP("StoreUserToken", new
        {
            UserId = userId,
            Token = token,
            Expiration = DateTime.UtcNow.AddMinutes(30)
        });
    }
}
