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
		
		private PackedScene bulletScene;
		
		int speed = 0;
		public bool isJumpPressed = false;
		public bool isDashPressed = false;
		public bool isCoyoteTriggered = false;
		public bool jumped = false;
		public bool shoot = false;
		public AnimatedSprite2D sprite;
		public Node2D dash;
		public Node2D coyote_time;
		public RayCast2D ceiling_ray;
		Vector2 UP_DIRECTION = new Vector2(0, -1);
		Vector2 velocity = Vector2.Zero;
		private bullet bullet;
		private Camera2D camera;
		Node2D root;
		Timer timer;
		public bool active = false;
		public bool bullets = true;
		public bool dead = false;
		CollisionShape2D collisionShape;
		public override void _Ready(){
			
			sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
			dash = GetNode<Node2D>("Dash");
			coyote_time = GetNode<Node2D>("CoyoteTime");
			ceiling_ray = GetNode<RayCast2D>("CeilingCheck");
			bulletScene = ResourceLoader.Load<PackedScene>("res://assets/objects/bullet.tscn");
			camera = GetNode<Camera2D>("Camera2D");
			camera.Enabled = false;
			root = (Node2D)GetParent().GetParent().GetParent().GetParent();
			timer = GetNode<Timer>("Timer");
			collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		}
		
		public override void _PhysicsProcess(double delta){	
			if (IsMultiplayerAuthority() && active && !dead){
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
				}else if(jump_canclled){
					velocity.Y = 0;
				}
				if(ceiling_ray.IsColliding()){
					velocity.Y = (float)(gravity*delta);
				}
				if(Input.IsActionJustReleased("jump")){
					isJumpPressed = false;
					isCoyoteTriggered = true;
				}
				if(Input.IsActionJustReleased("dash")){
					isDashPressed = false;
				}
				
				if(Input.IsActionPressed("shoot") && !shoot && bullets){
					shoot = true;
					bullet = (bullet)bulletScene.Instantiate();
					AddChild(bullet);
					Vector2 mousePosition = GetGlobalMousePosition();
					float angle = Mathf.Atan2(mousePosition.Y - this.GlobalPosition.Y, mousePosition.X - this.GlobalPosition.X);
					bullet.Rotation = Mathf.DegToRad(Mathf.RadToDeg(angle)-90);
					Godot.Collections.Array arguments = new Godot.Collections.Array { bullets };
					root.Call("callRcp", 3, arguments);
					
				}else if(Input.IsActionJustReleased("shoot")){
					shoot = false;
				}
				if(idle){
					sprite.Play("Idle");
					isCoyoteTriggered = false;
					jumped = false;
				}
				if(jump && !jumped){
					sprite.Play("Jump");
					jumped = true;
				}else if(falling && !isCoyoteTriggered){
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
				
				if((IsOnFloor() && !jump) || !camera.Enabled){
					velocity.Y = 0;
				}
				if(velocity.X > 0){
					sprite.FlipH = false;
				}else if(velocity.X < 0){
					sprite.FlipH = true;
				}
				
				Velocity = velocity;
				this.Position = new Vector2(0, 0);
				MoveAndSlide();
				damage();
			}
		}
		public	void damage(){
			for(int i = 0; i < GetSlideCollisionCount(); i++){
				KinematicCollision2D collision = GetSlideCollision(i);
				if((collision.GetCollider() as Node).GetParent().Name == "Enemy" && timer.IsStopped()){
					timer.WaitTime = 1F;
					timer.Start();
					Godot.Collections.Array arguments = new Godot.Collections.Array { 10 };
					root.Call("callRcp", 2, arguments);
				}
			}
		}
		public Vector2 getVelocity(){
			return velocity;
		}
		public void setActive(bool active){
			this.active = active;
			collisionShape.SetDeferred("disabled", !active);
		}
		public void refill(bool has){
			bullets = has;
		}
		public void die(){
			dead = true;
			SetProcess(false);
			sprite.AnimationFinished += finish;
			sprite.Play("Death");
		}
		public void finish(){
			GetTree().Paused = true;
		}
}
