using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
  class Player
  {
    public int id;
    public string username;
    public PlayerCharacter character;

    public Vector3 direction;
    //public float moveSpeed = 5f / Constants.TICKS_PER_SEC;

    //private float yPos;

    public Vector3 position;
    public Quaternion rotation;

    //private bool[] inputs;

    public Player(int _id, string _username, Vector3 _spawnPosition, PlayerCharacter _character)
    {
      id = _id;
      username = _username;
      position = _spawnPosition;
      rotation = Quaternion.Identity;
      character = _character;
      //inputs = new bool[4];
    }

    public void Update()
    {
      //Vector2 _inputDirection = Vector2.Zero;
      //if (inputs[0])                  //Forward
      //{
      //  _inputDirection.Y += 1;
      //}
      //if (inputs[1])                  //Backward
      //{
      //  _inputDirection.Y -= 1;
      //}
      //if (inputs[2])                  //Left
      //{
      //  _inputDirection.X += 1;
      //}
      //if (inputs[3])                  //Right
      //{
      //  _inputDirection.X -= 1;
      //}

      Move();
    }

    private void Move()//Move(Vector2 _inputDirection)
    {
      //Vector3 _forward = Vector3.Transform(new Vector3(0, 0, 1), rotation);
      //Vector3 _right = Vector3.Normalize(Vector3.Cross(_forward, new Vector3(0, 1, 0)));

      //direction = _right * _inputDirection.X + _forward * _inputDirection.Y;
      ////position += direction * moveSpeed;
      //direction *= moveSpeed;

      //position.Y = yPos;

      ServerSend.PlayerPosition(this);
      ServerSend.PlayerRotation(this);
    }
    //Fix Client authority on grounded
    public void SetPosition(Vector3 _position, Quaternion _rotation)
    {
      position = _position;
      rotation = _rotation;
    }
    public void SetInput(bool[] _inputs, Quaternion _rotation, float _ypos)
    {
      //inputs = _inputs;
      ////position = _position;
      //rotation = _rotation;
      //yPos = _ypos;
    }

  }
}
