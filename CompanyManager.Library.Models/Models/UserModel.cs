using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyManager.Library.Models.Models;

public class UserModel
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public AuthToken? Token { get; set; }
}

public class AuthToken
{
    public string Token { get; set; } = string.Empty;
    public DateTime ValidTo { get; set; }

    public AuthToken(string token, DateTime validTo)
    {
        Token = token;
        ValidTo = validTo;
    }
}
