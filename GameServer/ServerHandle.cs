using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using GameServer.Models;
using ServerDB;

namespace GameServer
{
  class ServerHandle
  {
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
      int _clientIdCheck = _packet.ReadInt();
      bool _newUser = _packet.ReadBool();
      string _username = _packet.ReadString();
      string _passwordAttempt = _packet.ReadString();
      try
      {
        if (!_newUser)
        {
          using (var context = ContextLayer.GetContext())
          {
            if (!ContextLayer.UserCredentialsCorrect(_username, _passwordAttempt, context))
            {
              Console.WriteLine("Invalid User");
              ServerSend.Alert(_clientIdCheck, "Username / password are invalid or User does not exist.");
            }
            else
            {
              var user = ContextLayer.GetUser(_username, context);
              Console.WriteLine($"Client: {Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and {_username} is now player {_fromClient}.");
              if (_fromClient != _clientIdCheck)
              {
                Console.WriteLine($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
              }
              // TODO: send player into game
              var characters = PlayerCharacter.ToCharacterList(ContextLayer.GetUserCharacters(user.Id, context));
              ServerSend.LoginSuccess(_clientIdCheck, new UserSigninData
              {
                UserName = user.Name,
                Characters = characters
              });
              //Server.clients[_fromClient].SendIntoGame(_username);
            }
          }
        }
      }
      catch (Exception e)
      {

      }
    }
    public static void SendPlayerIntoGame(int _fromClient, Packet _packet)
    {
      using (var context = ContextLayer.GetContext())
      {
        int _clientIdCheck = _packet.ReadInt();
        var _username = _packet.ReadString();
        var _characterId = _packet.ReadLong();
        //change to use token / other
        var user = ContextLayer.GetUser(_username, context);
        var characters = PlayerCharacter.ToCharacterList(ContextLayer.GetUserCharacters(user.Id, context));
        Server.clients[_fromClient].SendIntoGame(_username, characters.FirstOrDefault(c=>c.Id == _characterId));
      }
    }
    public static void PlayerMovement(int _fromClient, Packet _packet)
    {
      //bool[] _inputs = new bool[_packet.ReadInt()];
      //for (int i = 0; i < _inputs.Length; i++)
      //{
      //  _inputs[i] = _packet.ReadBool();
      //}
      //float _yPos = _packet.ReadFloat();
      Vector3 _position = _packet.ReadVector3();
      Quaternion _rotation = _packet.ReadQuaternion();

      if (Server.clients[_fromClient].player != null)
      {
        Server.clients[_fromClient].player.SetPosition(_position, _rotation);
        //Server.clients[_fromClient].player.SetInput(_inputs, _rotation, _yPos);
      }
    }
    public static void UDPTest(int _fromClient, Packet _packet)
    {
      string _msg = _packet.ReadString();

      Console.WriteLine($"Received packet via UDP. Contains message: {_msg}");
    }
    public static void NewUser(int _fromClient, Packet _packet)
    {
      int _clientIdCheck = _packet.ReadInt();
      string _username = _packet.ReadString();
      string _email = _packet.ReadString();
      string _password = _packet.ReadString();

      try
      {
        using (var context = ContextLayer.GetContext())
        {
          Console.WriteLine("New User: " + _username + " - " + _email);
          
          if (!ContextLayer.DoesUserEmailExist(_email, context))
          {
            //ServerSend.SendNewUserEmail();
            //
            ContextLayer.AddNewUser(_username, _email, _password, context);
            ServerSend.Alert(_clientIdCheck, "User created check your email to verify your account before logging in.");
          }
          else
          {
            ServerSend.Alert(_clientIdCheck, "An Error occured or user already exists.");
          }

        }
      }
      catch (Exception e)
      {

      }
    }
    public static void NewCharacter(int _fromClient, Packet _packet)
    {
      int _clientIdCheck = _packet.ReadInt();
      string _userName = _packet.ReadString();
      string _newCharacterName = _packet.ReadString();

      try
      {
        using (var context = ContextLayer.GetContext())
        {
          Console.WriteLine("New Character: " + _newCharacterName);
          var NewCharacter = ContextLayer.AddNewCharacter(_userName, _newCharacterName, context);
          if(NewCharacter != null)
            ServerSend.NewCharacterCreated(_fromClient, new PlayerCharacter(NewCharacter.Id, NewCharacter.Name));
        }
      }
      catch (Exception e)
      {

      }
    }
  }
}
