using Godot;
using System;

public partial class player: CharacterBody2D{

		[Export]
		public int speed {get; set;} = 200;
		[Export]
		public int jump_str {get; set;} = 450;
		[Export]
		public int gravity {get; set;} = 1000;

		public AnimatedSprite2D sprite;
		Vector2 UP_DIRECTION = new Vector2(0, -1);
		Vector2 velocity = Vector2.Zero;
		
		public override void _Ready(){
			
			sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
			
		}
		
		public override void _PhysicsProcess(double delta){	
			velocity.X = (Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left")) * speed;
			velocity.Y += (float)(gravity*delta);
			bool jump = Input.IsActionPressed("jump") && IsOnFloor();
			bool falling = !IsOnFloor() && Velocity.Y > 0.0;
			bool jump_canclled = Input.IsActionJustReleased("jump") && Velocity.Y < 0.0;
			bool idle = IsOnFloor() && Mathf.IsZeroApprox(velocity.X);
			if(jump){
				velocity.Y = -jump_str;
			}else if(jump_canclled){
				velocity.Y = 0;
			}
			if(jump){
				sprite.Play("Jump");
			}else if(idle){
				sprite.Play("Idle");
			}else  if(falling){
				sprite.Play("Falling");
			}else if(Mathf.Abs(velocity.X) > 0 && IsOnFloor()){
				sprite.Play("Run");
			}
			if(velocity.X > 0){
				sprite.FlipH = false;
			}else if(velocity.X < 0){
				sprite.FlipH = true;
			}
			
			Velocity = velocity;
			MoveAndSlide();
		}
}
