using Godot;
using System;

public partial class charBody2dPlayer: CharacterBody2D{

		[Export]
		public int speed {get; set;} = 100;
		[Export]
		public int jump {get; set;} = 100;

		public override void _PhysicsProcess(double delta){
			Vector2 vector = Vector2.Zero;
			if(Input.IsActionPressed("move_left")){
				vector.X = -1*speed;
			}else if(Input.IsActionPressed("move_right")){
				vector.Y = 1*speed;
			}
			Velocity = vector;
			MoveAndSlide();
		}
}
