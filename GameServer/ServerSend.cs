using GameServer.Models;
using ServerDB;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace GameServer
{
  class ServerSend
  {
    private static void SendTCPData(int _toClient, Packet _packet)
    {
      _packet.WriteLength();
      Server.clients[_toClient].tcp.SendData(_packet);
    }
    private static void SendUDPData(int _toClient, Packet _packet)
    {
      _packet.WriteLength();
      Server.clients[_toClient].udp.SendData(_packet);
    }
    private static void SendTCPDataToAll(Packet _packet)
    {
      _packet.WriteLength();
      for (int i = 1; i <= Server.MaxPlayers; i++)
      {
        Server.clients[i].tcp.SendData(_packet);
      }
    }
    private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
    {
      _packet.WriteLength();
      for (int i = 1; i <= Server.MaxPlayers; i++)
      {
        if (i != _exceptClient)
        {
          Server.clients[i].tcp.SendData(_packet);
        }
      }
    }
    private static void SendUDPDataToAll(Packet _packet)
    {
      _packet.WriteLength();
      for (int i = 1; i <= Server.MaxPlayers; i++)
      {
        Server.clients[i].udp.SendData(_packet);
      }
    }
    private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
    {
      _packet.WriteLength();
      for (int i = 1; i <= Server.MaxPlayers; i++)
      {
        if (i != _exceptClient)
        {
          Server.clients[i].udp.SendData(_packet);
        }
      }
    }

    #region
    //send to client
    public static void Welcome(int _toClient, string _msg)
    {
      using (Packet _packet = new Packet((int)ServerPackets.welcome))
      {
        _packet.Write(_msg);
        _packet.Write(_toClient);

        SendTCPData(_toClient, _packet);
      }
    }

    public static void SendNewUserEmail()
    {
      //TODO: setup new user email send receive
            
    }

    public static void InvalidLogin(int _toClient)
    {
      using (Packet _packet = new Packet((int)ServerPackets.invalidLogin))
      {
        var _client = Server.clients[_toClient].tcp.socket.Client;
        _packet.Write("Username / password are invalid or User does not exist.");
        _packet.Write(_toClient);

        SendTCPData(_toClient, _packet);
        //_client.Shutdown(SocketShutdown.Both);
        //_client.Disconnect(true);
        //Server.clients[_toClient] = new Client(_toClient);
      }
    }
    public static void LoginSuccess(int _toClient, UserSigninData d)
    {
      using (Packet _packet = new Packet((int)ServerPackets.loginSuccess))
      {
        var _client = Server.clients[_toClient].tcp.socket.Client;
        _packet.Write(_toClient);
        _packet.Write(d.UserName);
        _packet.Write(d.Characters.Count);
        foreach (var c in d.Characters)
        {
          _packet.Write(c.Id);
          _packet.Write(c.Name);
        }

        SendTCPData(_toClient, _packet);
      }
    }
    public static void NewCharacterCreated(int _toClient, PlayerCharacter character)
    {
      using (Packet _packet = new Packet((int)ServerPackets.newCharacter))
      {
        _packet.Write(_toClient);
        _packet.Write(character.Name);

        SendTCPData(_toClient, _packet);
      }
    }
    public static void SpawnPlayer(int _toClient, Player _player)
    {
      using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
      {
        _packet.Write(_player.id);
        _packet.Write(_player.username);
        _packet.Write(_player.character.Id);
        _packet.Write(_player.character.Name);
        _packet.Write(_player.position);
        _packet.Write(_player.rotation);

        Console.WriteLine($"Client: {_toClient} Player: {_player.username} Id: {_player.id} Spawned --- Postion: {_player.position}");

        SendTCPData(_toClient, _packet);
      }
    }
    public static void PlayerPosition(Player _player)
    {
      using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
      {
        _packet.Write(_player.id);
        _packet.Write(_player.position);

        SendUDPDataToAll(_player.id, _packet);
      }
    }

    public static void PlayerRotation(Player _player)
    {
      using (Packet _packet = new Packet((int)ServerPackets.playerRotation))
      {
        _packet.Write(_player.id);
        _packet.Write(_player.rotation);

        SendUDPDataToAll(_player.id, _packet);
      }
    }

    public static void ClientDisconnected(int _id)
    {
      using (Packet _packet = new Packet((int)ServerPackets.clientDisconncet))
      {
        _packet.Write(_id);
        SendTCPDataToAll(_packet);
      }
    }

    public static void UDPTest(int _toClient)
    {
      using (Packet _packet = new Packet((int)ServerPackets.udpTest))
      {
        _packet.Write("A test packet for UDP.");

        SendUDPData(_toClient, _packet);
      }
    }
    #endregion
  }
}
