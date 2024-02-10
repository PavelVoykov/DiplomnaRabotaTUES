using Godot;
using System;

public partial class enemy : CharacterBody2D
{
	public const float Speed = 300.0f;

	[Export]
	public int gravity {get; set;} = 10000;
	Vector2 UP_DIRECTION = new Vector2(0, -1);
	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;
		Velocity = velocity;
		MoveAndSlide();
	}
}
