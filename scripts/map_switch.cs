using Godot;
using System;

public partial class map_switch : Node2D
{
	private Node2D _player1;
	private Node2D _player2;
	private Node2D _player3;
	private Node2D _player4;
	private Area2D _player1CollisionCheck;
	private Area2D _player2CollisionCheck;
	private Area2D _player3CollisionCheck;
	private Area2D _player4CollisionCheck;
	private int activeMap = 1;
	
	public override void _Ready()
	{
		_player1 = GetNode("Map1").GetNode<Node2D>("player1");
		_player2 = GetNode("Map2").GetNode<Node2D>("player2");
		_player3 = GetNode("Map3").GetNode<Node2D>("player3");
		_player4 = GetNode("Map4").GetNode<Node2D>("player4");
		_player1CollisionCheck = _player1.GetNode<Area2D>("CollisionCheck");                                     
		_player2CollisionCheck = _player2.GetNode<Area2D>("CollisionCheck");
		_player3CollisionCheck = _player3.GetNode<Area2D>("CollisionCheck");
		_player4CollisionCheck = _player4.GetNode<Area2D>("CollisionCheck");
		map1();
		GetParent().GetNode<MultiplayerSpawner>("MultiplayerSpawner").SpawnPath = this.GetPath();
	}

	public override void _PhysicsProcess(double delta)
	{
		switch(activeMap){
			case 1:
				_player2.Position = _player1.Position;
				_player3.Position = _player1.Position;
				_player4.Position = _player1.Position;
				break;
			case 2:
				_player1.Position = _player2.Position;
				_player3.Position = _player2.Position;
				_player4.Position = _player2.Position;
				break;
			case 3:
				_player2.Position = _player3.Position;
				_player1.Position = _player3.Position;
				_player4.Position = _player3.Position;
				break;
			case 4:
				_player2.Position = _player4.Position;
				_player3.Position = _player4.Position;
				_player1.Position = _player4.Position;
				break;
		}
		
	}
	
	public void map1()
	{
		if(!(bool)_player1CollisionCheck.Call("Overlapps"))
		{
			_player1.Call("activate");
			_player2.Call("deactivate");
			_player3.Call("deactivate");
			_player4.Call("deactivate");
			activeMap = 1;
		}
	}
	
	public void map2()
	{
		if(!(bool)_player2CollisionCheck.Call("Overlapps"))
		{//t56ju
			_player2.Call("activate");
			_player1.Call("deactivate");
			_player3.Call("deactivate");
			_player4.Call("deactivate");
			activeMap = 2;
		}
	}
	
	public void map3()
	{
		if(!(bool)_player3CollisionCheck.Call("Overlapps"))
		{
			_player3.Call("activate");
			_player2.Call("deactivate");
			_player1.Call("deactivate");
			_player4.Call("deactivate");
			activeMap = 3;
		}
	}
	
	public void map4()
	{
		if(!(bool)_player4CollisionCheck.Call("Overlapps"))
		{
			_player4.Call("activate");
			_player2.Call("deactivate");
			_player3.Call("deactivate");
			_player1.Call("deactivate");
			activeMap = 4;
		}
	}
	
	public void hasBulletsToggle(bool hasBullets)
	{
		_player1.GetNode("PlayerBody").Call("hasBulletsToggle", hasBullets);
		_player2.GetNode("PlayerBody").Call("hasBulletsToggle", hasBullets);
		_player3.GetNode("PlayerBody").Call("hasBulletsToggle", hasBullets);
		_player4.GetNode("PlayerBody").Call("hasBulletsToggle", hasBullets);
	}
	
	public void die()
	{
		switch(activeMap){
			case 1:
				_player1.GetNode("PlayerBody").Call("die");
				break;
			case 2:
				_player2.GetNode("PlayerBody").Call("die");
				break;
			case 3:
				_player3.GetNode("PlayerBody").Call("die");
				break;
			case 4:
				_player4.GetNode("PlayerBody").Call("die");
				break;
		}
	}
	
}
