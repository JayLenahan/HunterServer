using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServerDB
{
  class PasswordHelper
  {
    public static bool IsPasswordValid(string real_password_salt_and_key, string password_attempt)
    {
      // Strip the iteration count off the front.
      var split_index = real_password_salt_and_key.IndexOf("$");
      var iteration_count = Convert.ToInt32(real_password_salt_and_key.Substring(0, split_index));
      real_password_salt_and_key = real_password_salt_and_key.Substring(split_index + 1);

      // Split the remaining string into the salt and the key.
      split_index = real_password_salt_and_key.IndexOf("$");
      var salt = Convert.FromBase64String(real_password_salt_and_key.Substring(0, split_index));
      var key = Convert.FromBase64String(real_password_salt_and_key.Substring(split_index + 1));

      using (var deriveBytes = new Rfc2898DeriveBytes(password_attempt, salt, iteration_count))
      {
        return deriveBytes.GetBytes(20).SequenceEqual(key);
      }
    }
    public static string GeneratePasswordSaltAndKey(string password)
    {
      var iteration_count = 50000;
      using (var deriveBytes = new Rfc2898DeriveBytes(password, 20, iteration_count))
      {
        byte[] salt = deriveBytes.Salt;
        byte[] key = deriveBytes.GetBytes(20);

        return Convert.ToString(iteration_count) + "$" + Convert.ToBase64String(salt) + "$" + Convert.ToBase64String(key);
      }
    }
  }
}
