using ServerDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
  public class PlayerCharacter
  {
    public long Id { get; set; }
    public string Name { get; set; }

    public PlayerCharacter(long _id, string _name)
    {
      Id = _id;
      Name = _name;
    }

    public static List<PlayerCharacter> ToCharacterList(List<usercharacter> characters)
    {
      var result = new List<PlayerCharacter>();
      foreach(var c in characters)
      {
        result.Add(new PlayerCharacter(c.Id, c.Name));
      }
      return result;
    }
  }
}
