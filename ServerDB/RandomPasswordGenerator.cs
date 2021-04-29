using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServerDB
{
  class RandomPasswordGenerator
  {
    private static int DEFAULT_PASSWORD_LENGTH = 20;

    public static string PASSWORD_CHARS = "abcdefghjkmnpqrstwxyzABCDEFGHJKMNPQRSTWXYZ23456789*$-+?_&=!%{}/";

    public string Generate()
    {
      return Generate(DEFAULT_PASSWORD_LENGTH, PASSWORD_CHARS);
    }

    public string Generate(int passwordLength, string passwordChars)
    {
      return GeneratePassword(passwordLength, passwordChars);
    }

    private static string GeneratePassword(int passwordLength, string passwordCharacters)
    {
      //if (passwordLength < 0)
      //  throw new ArgumentOutOfRangeException("password length");
      //if (string.IsNullOrEmpty(passwordCharacters))
      //  throw new ArgumentOutOfRangeException("password characters");

      var password = new char[passwordLength];

      var random = GetRandom();

      for(int i = 0; i < passwordLength; i++)
      {
        password[i] = passwordCharacters[random.Next(passwordCharacters.Length)];
      }
      return new string(password);
    }

    private static Random GetRandom()
    {
      byte[] randomBytes = new byte[4];

      new RNGCryptoServiceProvider().GetBytes(randomBytes);

      int seed = (randomBytes[0] & 0x7f) << 24 |
                    randomBytes[1] << 16 |
                      randomBytes[2] << 8 |
                        randomBytes[3];
      return new Random(seed);
    }

  }
}
