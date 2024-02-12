using Godot;
using System;

public partial class enemy : CharacterBody2D
{
	[Export]
	public int Gravity {get; set;} = 10000;
	public const float Speed = 300.0f;
	Vector2 UP_DIRECTION = new Vector2(0, -1);
	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		if (!IsOnFloor())
		{
			velocity.Y += Gravity * (float)delta;
		}
		Velocity = velocity;
		MoveAndSlide();
	}
}
