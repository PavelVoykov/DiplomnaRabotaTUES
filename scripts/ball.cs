using Godot;
using System;

public partial class ball : RigidBody2D
{
	
	[Export]
	public int speed {get; set;} = 100;
	[Export]
	public int jump {get; set;} = 100;
	[Export]
	public RayCast2D ray1;
	[Export]
	public RayCast2D ray2 ;
	[Export]
	public RayCast2D ray3;
	[Export]
	public RayCast2D ray4;
	[Export]
	public RayCast2D ray5;
	[Export]
	public RayCast2D ray6;
	[Export]
	public RayCast2D ray7;
	[Export]
	public RayCast2D ray8;
	public Vector2 screenSize;
	Vector2 vector = Vector2.Zero;
	public bool btn_up = true;
	
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

		if (Input.IsActionPressed("jump") && (ray1.IsColliding() || ray2.IsColliding() || ray3.IsColliding() || ray4.IsColliding() || ray5.IsColliding() || ray6.IsColliding() || ray7.IsColliding() || ray8.IsColliding()) && btn_up){
			btn_up = false;
			vector.Y = -1*jump;
		}
		if (!Input.IsActionPressed("jump") && (!ray1.IsColliding() || !ray2.IsColliding() || !ray3.IsColliding() || !ray4.IsColliding() || !ray5.IsColliding() || !ray6.IsColliding() || !ray7.IsColliding() || !ray8.IsColliding())){
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
