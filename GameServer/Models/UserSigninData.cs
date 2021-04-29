using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
  class UserSigninData
  {
    public int Id { get; set; }
    public string UserName { get; set; }
    public List<PlayerCharacter> Characters { get; set; }

  }
}
