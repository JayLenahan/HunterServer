using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
  class GameLogic
  {
    public static void Update()
    {
      foreach (Client _client in Server.clients.Values)
      {
        if (_client.player != null)
        {
          //Console.WriteLine($"client: {_client.id} - position: {_client.player.position}");
          _client.player.Update();
        }
      }
      ThreadManager.UpdateMain();
    }
  }
}
