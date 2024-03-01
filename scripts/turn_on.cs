using Godot;
using System;

public partial class turn_on : Node2D
{	
	//8i9aqu76
	Camera2D camera;
	Node2D player;

	public override void _Ready()
	{
		camera = GetNode("PlayerBody").GetNode<Camera2D>("Camera2D");
		player = GetNode<Node2D>("PlayerBody");
	}

	public override void _PhysicsProcess(double delta)
	{
		this.GlobalPosition = new Vector2(player.GlobalPosition.X, player.GlobalPosition.Y);
	}
	
	public void activate()
	{
		SetProcess(false);
		player.Call("setActive", true);
		camera.Enabled = true;	
	}
	
	public void deactivate()
	{
		SetProcess(true);
		player.Call("setActive", false);
		camera.Enabled = false;	
	}
}
