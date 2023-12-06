using Godot;
using System;

public partial class player: CharacterBody2D{

		[Export]
		public int movementSpeed {get; set;} = 2000;
		[Export]
		public int jump_str {get; set;} = 2250;
		[Export]
		public int gravity {get; set;} = 10000;
		[Export]
		public int dashSpeed {get; set;} = 4000;
		[Export]
		public float dashLength {get; set;} = .3F;
		[Export]
		public float coyoteDuration {get; set;} = .01F;
		
		
		int speed = 0;
		public bool isJumpPressed = false;
		public bool isDashPressed = false;
		public bool isCoyoteTriggered = false;
		public bool jumped = false;
		public AnimatedSprite2D sprite;
		public Node2D dash;
		public Node2D coyote_time;
		public RayCast2D ceiling_ray;
		Vector2 UP_DIRECTION = new Vector2(0, -1);
		Vector2 velocity = Vector2.Zero;
		
		public override void _Ready(){
			
			sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
			dash = GetNode<Node2D>("Dash");
			coyote_time = GetNode<Node2D>("CoyoteTime");
			ceiling_ray = GetNode<RayCast2D>("CeilingCheck");
		}
		
		public override void _PhysicsProcess(double delta){	
			
			bool jump = Input.IsActionPressed("jump") && !isJumpPressed && (IsOnFloor() || (bool)coyote_time.Call("isCoyote") && !jumped);
			bool falling = !IsOnFloor() && Velocity.Y > 0.0;
			bool jump_canclled = Input.IsActionJustReleased("jump") && Velocity.Y < 0.0;
			bool idle = IsOnFloor() && Mathf.IsZeroApprox(velocity.X);
			if((bool)dash.Call("isDashing"))	{
				speed = dashSpeed;
			}else{
				speed = movementSpeed;
			}
			
			if(Input.IsActionPressed("dash") && !isDashPressed){
				dash.Call("startDash", dashLength);
				isDashPressed = true;
			}
			if(jump){
				velocity.Y = -jump_str;
				isJumpPressed = true;
			}else if(jump){
				velocity.Y = -jump_str;
				isJumpPressed = true;
			}else if(jump_canclled){
				velocity.Y = 0;
			}
			if(ceiling_ray.IsColliding()){
				velocity.Y = (float)(gravity*delta);
			}
			if(Input.IsActionJustReleased("jump")){
				isJumpPressed = false;
			}
			if(Input.IsActionJustReleased("dash")){
				isDashPressed = false;
			}
			
			
			if(jump && !jumped){
				sprite.Play("Jump");
				jumped = true;
			}else if(idle){
				sprite.Play("Idle");
				isCoyoteTriggered = false;
				jumped = false;
			}else  if(falling && !isCoyoteTriggered){
				coyote_time.Call("startTime", coyoteDuration);
				sprite.Play("Falling");
				isCoyoteTriggered = true;
			}else if(Mathf.Abs(velocity.X) > 0 && IsOnFloor()){
				sprite.Play("Run");
				isCoyoteTriggered = false;
				jumped = false;
			}
			
			velocity.X = (Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left")) * speed;
			velocity.Y += (float)(gravity*delta);
			
			if(IsOnFloor() && !jump){
				velocity.Y = 0;
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
