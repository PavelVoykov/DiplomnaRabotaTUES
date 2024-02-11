using Godot;
using System;

public partial class map_switch : Node2D
{
	private Node2D player1;
	private Node2D player2;
	private Node2D player3;
	private Node2D player4;
	private Area2D p1check;
	private Area2D p2check;
	private Area2D p3check;
	private Area2D p4check;
	private int activeMap = 1;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player1 = GetNode("TileMap").GetNode<Node2D>("1");
		p1check = player1.GetNode<Area2D>("Area2D");
		player2 = GetNode("TileMap2").GetNode<Node2D>("1");
		p2check = player2.GetNode<Area2D>("Area2D");
		player3 = GetNode("TileMap3").GetNode<Node2D>("1");
		p3check = player3.GetNode<Area2D>("Area2D");
		player4 = GetNode("TileMap4").GetNode<Node2D>("1");
		p4check = player4.GetNode<Area2D>("Area2D");
		map1();
		GetParent().GetNode<MultiplayerSpawner>("MultiplayerSpawner").SpawnPath = this.GetPath();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		switch(activeMap){
			case 1:
				player2.Position = player1.Position;
				player3.Position = player1.Position;
				player4.Position = player1.Position;
				break;
			case 2:
				player1.Position = player2.Position;
				player3.Position = player2.Position;
				player4.Position = player2.Position;
				break;
			case 3:
				player2.Position = player3.Position;
				player1.Position = player3.Position;
				player4.Position = player3.Position;
				break;
			case 4:
				player2.Position = player4.Position;
				player3.Position = player4.Position;
				player1.Position = player4.Position;
				break;
		}
		
	}
	public void map1(){
		if(!(bool)p1check.Call("Overlapps")){
			player1.Call("activate");
			player2.Call("deactivate");
			player3.Call("deactivate");
			player4.Call("deactivate");
			activeMap = 1;
			GD.Print("Map1");	
		}
	}
	public void map2(){
		if(!(bool)p2check.Call("Overlapps")){
			player2.Call("activate");
			player1.Call("deactivate");
			player3.Call("deactivate");
			player4.Call("deactivate");
			activeMap = 2;
			GD.Print("Map2");	
		}
	}
	public void map3(){
		if(!(bool)p3check.Call("Overlapps")){
			player3.Call("activate");
			player2.Call("deactivate");
			player1.Call("deactivate");
			player4.Call("deactivate");
			activeMap = 3;
			GD.Print("Map3");	
		}
	}
	public void map4(){
		if(!(bool)p4check.Call("Overlapps")){
			player4.Call("activate");
			player2.Call("deactivate");
			player3.Call("deactivate");
			player1.Call("deactivate");
			activeMap = 4;
			GD.Print("Map4");	
		}
	}
	public void refill(bool has){
		player1.GetNode("CharacterBody2D").Call("refill", has);
		player2.GetNode("CharacterBody2D").Call("refill", has);
		player3.GetNode("CharacterBody2D").Call("refill", has);
		player4.GetNode("CharacterBody2D").Call("refill", has);
	}
	public void die(){
				switch(activeMap){
			case 1:
				player1.GetNode("CharacterBody2D").Call("die");
				break;
			case 2:
				player2.GetNode("CharacterBody2D").Call("die");
				break;
			case 3:
				player3.GetNode("CharacterBody2D").Call("die");
				break;
			case 4:
				player4.GetNode("CharacterBody2D").Call("die");
				break;
		}
	}
}
