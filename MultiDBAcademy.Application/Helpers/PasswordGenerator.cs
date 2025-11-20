using System.Security.Cryptography;

namespace MultiDBAcademy.Application.Helpers;

public static class PasswordGenerator
{
    public static string Generate(int length = 16)
    {
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%";
        var result = new char[length];
        
        using var rng = RandomNumberGenerator.Create();
        var buffer = new byte[length];
        rng.GetBytes(buffer);
        
        for (int i = 0; i < length; i++)
        {
            result[i] = validChars[buffer[i] % validChars.Length];
        }
        
        return new string(result);
    }
}