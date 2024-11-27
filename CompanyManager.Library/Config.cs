using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyManager.Library;

public static class Config
{
    public const string Host = "http://localhost:5170/";
    public const string ApiHost = $"{Host}api/";
    public const string NotesPrefix = "notes/";
}
