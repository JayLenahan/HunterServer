using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDB
{
  public static class ContextLayer
  {
    public static hunterEntities GetContext()
    {
      hunterEntities context = new hunterEntities();
      ((IObjectContextAdapter)context).ObjectContext.CommandTimeout = 180;

      //#if DEBUG
      //context.Database.Log = (s) => deb
      //#endif

      return context;
    }

    public static bool UserCredentialsCorrect(string userName, string pAttempt, hunterEntities context)
    {
      var user = context.users.FirstOrDefault(u => u.Name == userName || u.Email == userName);
      return user != null ? PasswordHelper.IsPasswordValid(user.Password, pAttempt) : false;
    }

    public static void AddNewUser(string username, string email, string password, hunterEntities context)
    {

      var u = new user
      {
        Name = username,
        Email = email
      };

      u.Password = PasswordHelper.GeneratePasswordSaltAndKey(password);
      //PasswordHelper.GeneratePasswordSaltAndKey(new RandomPasswordGenerator().Generate());

      context.users.Add(u);
      context.SaveChanges();
    }

    public static void SendNewUserEmail()
    {

    }

    public static usercharacter AddNewCharacter(string _userName, string _newCharacterName, hunterEntities context)
    {
      var user = context.users.FirstOrDefault(u => u.Name == _userName || u.Email == _userName);
      var newCharacter = new usercharacter
      {
        Name = _newCharacterName
      };
      if (user != null)
      {
        user.usercharacters.Add(newCharacter);
        context.SaveChanges();
        return newCharacter;
      }
      else
      {
        //Throw Error can't create user
        return null;
      }
    }

    public static user GetUser(string username, hunterEntities context)
    {
      return context.users.FirstOrDefault(u => u.Name == username || u.Email == username);
    }

    public static List<usercharacter> GetUserCharacters(long _userId, hunterEntities context)
    {
      return context.usercharacters.Where(p => p.UserId == _userId).ToList();
    }
    
    public static bool DoesUserEmailExist(string email, hunterEntities context)
    {
      return context.users.Any(u => u.Email == email);
    }
  }
}
