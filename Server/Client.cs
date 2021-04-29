﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Server
{
  class Client
  {
    public static int dataBufferSize = 4096;

    public int id;
    public TCP tcp;

    public Client(int _clientId)
    {
      id = _clientId;
      tcp = new TCP(id);
    }
    public class TCP
    {
      public TcpClient socket;

      private readonly int id;
      private NetworkStream stream;
      private Packet receivedData;
      private byte[] receiveBuffer;

      public TCP(int _id)
      {
        id = _id;
      }

      public void Connect(TcpClient _socket)
      {
        //on connect here SECOND
        socket = _socket;
        socket.ReceiveBufferSize = dataBufferSize;
        socket.SendBufferSize = dataBufferSize;

        stream = socket.GetStream();

        receivedData = new Packet();
        receiveBuffer = new byte[dataBufferSize];

        stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

        ServerSend.Welcome(id, $"Welcome to the server Client: {id}");
      }

      public void SendData(Packet _packet)
      {
        try
        {
          if (socket != null)
          {
            stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
          }
        }
        catch (Exception _ex)
        {
          Console.WriteLine($"Error sending data to player {id} via TCP: {_ex}");
        }
      }

      private void ReceiveCallback(IAsyncResult _result)
      {
        try
        {
          int _byteLength = stream.EndRead(_result);
          if(_byteLength <= 0)
          {
            //TODO: disconnect
            socket.Client.Disconnect(true);
            return;
          }

          byte[] _data = new byte[_byteLength];
          Array.Copy(receiveBuffer, _data, _byteLength);

          receivedData.Reset(HandleData(_data));
          stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }
        catch(Exception _ex)
        {
          Console.WriteLine($"Error receiving TCP data: {_ex}");
        }
      }

      private bool HandleData(byte[] _data)
      {
        int _packLength = 0;

        receivedData.SetBytes(_data);

        if (receivedData.UnreadLength() >= 4)
        {
          _packLength = receivedData.ReadInt();
          if (_packLength <= 0)
          {
            return true;
          }
        }

        while (_packLength > 0 && _packLength <= receivedData.UnreadLength())
        {
          byte[] _packageBytes = receivedData.ReadBytes(_packLength);
          ThreadManager.ExecuteOnMainThread(() =>
          {
            using (Packet _packet = new Packet(_packageBytes))
            {
              int _packetId = _packet.ReadInt();
              Server.packetHandlers[_packetId](id, _packet);
            }
          });

          _packLength = 0;
          if (receivedData.UnreadLength() >= 4)
          {
            _packLength = receivedData.ReadInt();
            if (_packLength <= 0)
            {
              return true;
            }
          }
        }

        if (_packLength <= 1)
        {
          return true;
        }

        return false;
      }
    }
  }
}