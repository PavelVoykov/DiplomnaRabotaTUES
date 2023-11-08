using Godot;
using System;

public partial class player : RigidBody2D
{
	
	[Export]
	public int speed {get; set;} = 100;
	[Export]
	public int jump {get; set;} = 100;
	[Export]
	public RayCast2D ray1;
	[Export]
	public RayCast2D ray2 ;
	public Vector2 screenSize;
	Vector2 vector = Vector2.Zero;
	public bool btn_up = true;
	public bool hero_up = true;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		screenSize = GetViewportRect().Size;
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
	
	public override void _PhysicsProcess(double delta){
		
		vector = Vector2.Zero;
		if (Input.IsActionPressed("move_left")){
			vector.X = -1*speed;
		}else if (Input.IsActionPressed("move_right")){
			vector.X = 1*speed;
		}

		if (Input.IsActionPressed("jump") && (ray1.IsColliding() || ray2.IsColliding()) && btn_up){
			btn_up = false;
			hero_up = false;
			vector.Y = -1*jump;
		}
		if(!ray1.IsColliding() || !ray2.IsColliding()){
			hero_up = true;
		}
		if (!Input.IsActionPressed("jump") && hero_up){
			btn_up = true;
		}
	}
	
	 public override void _IntegrateForces(PhysicsDirectBodyState2D state)
	{
		state.ApplyForce(vector);
		var jumpVector = vector;
		jumpVector.X = 0;
		state.ApplyImpulse(jumpVector);
		
	}
}
