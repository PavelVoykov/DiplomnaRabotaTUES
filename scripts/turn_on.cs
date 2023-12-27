using Godot;
using System;

public partial class turn_on : Node2D
{
	Camera2D camera;
	Node2D player;
	Vector2 velocity;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		camera = GetNode("CharacterBody2D").GetNode<Camera2D>("Camera2D");
		player = GetNode<Node2D>("CharacterBody2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		
		this.GlobalPosition = new Vector2(player.GlobalPosition.X, player.GlobalPosition.Y);
	}
	
	public void activate(){
		GetNode("Area2D").SetProcess(false);
		SetProcess(false);
		camera.Enabled = true;	
	}
	
	public void deactivate(){
		GetNode("Area2D").SetProcess(true);
		SetProcess(true);
		camera.Enabled = false;	
	}
}
