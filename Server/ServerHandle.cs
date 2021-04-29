using System;
using System.Collections.Generic;
using System.Text;
using ServerDB;

namespace Server
{
  class ServerHandle
  {
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
      int _clientIdCheck = _packet.ReadInt();
      string _username = _packet.ReadString();
      string _passwordAttempt = _packet.ReadString();

      if (!ContextLayer.DoesUserExist(_username))
      {
        Console.WriteLine("New User");
        Server.clients[_fromClient].tcp.socket.Client.Disconnect(true);
      }
      else
      {
        Console.WriteLine($"Client: {Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and {_username} is now player {_fromClient}.");
        if (_fromClient != _clientIdCheck)
        {
          Console.WriteLine($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
        }
        // TODO: send player into game
      }
    }
  }
}
