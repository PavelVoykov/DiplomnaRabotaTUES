using Godot;
using System;

public partial class player: CharacterBody2D{

		[Export]
		public float CoyoteDuration {get; set;} = .1F;
		[Export]
		public int MovementSpeed {get; set;} = 2000;
		[Export]
		public int JumpStrength {get; set;} = 4250;
		[Export]
		public float DashLength {get; set;} = .3F;
		[Export]
		public int DashSpeed {get; set;} = 4000;
		[Export]
		public int Gravity {get; set;} = 10000;
		public bool IsCoyoteTriggered = false;
		public bool IsJumpPressed = false;
		public bool IsDashPressed = false;
		public bool Active = false;
		public bool Bullets = true;
		public bool Jumped = false;
		public bool Shoot = false;
		public bool Dead = false;
		public AnimatedSprite2D SpriteAnimation;
		public RayCast2D CeilingCheck;
		public Node2D CoyoteTime;
		public Node2D Dash;
		private PackedScene _bulletScene;
		private int _speed = 0;
		private bullet _bullet;
		private Camera2D _camera;
		Vector2 UP_DIRECTION = new Vector2(0, -1);
		CollisionShape2D collisionShape;
		Vector2 velocity = Vector2.Zero;
		Timer takeDamageTimer;
		Node2D root;
		
		public override void _Ready()
		{			
			_bulletScene = ResourceLoader.Load<PackedScene>("res://assets/objects/bullet.tscn");
			SpriteAnimation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
			root = (Node2D)GetParent().GetParent().GetParent().GetParent();
			collisionShape = GetNode<CollisionShape2D>("PlayerCollision");
			CeilingCheck = GetNode<RayCast2D>("CeilingCheck");
			CoyoteTime = GetNode<Node2D>("CoyoteTime");
			takeDamageTimer = GetNode<Timer>("Timer");
			_camera = GetNode<Camera2D>("Camera2D");
			Dash = GetNode<Node2D>("Dash");
			_camera.Enabled = false;
		}
		
		public override void _PhysicsProcess(double delta)
		{	
			if (IsMultiplayerAuthority() && Active && !Dead)
			{
				bool jump = Input.IsActionPressed("jump") && !IsJumpPressed && (IsOnFloor() || (bool)CoyoteTime.Call("isCoyote") && !Jumped);
				bool falling = !IsOnFloor() && Velocity.Y > 0.0;
				bool jumpCanclled = Input.IsActionJustReleased("jump") && Velocity.Y < 0.0;
				bool idle = IsOnFloor() && Mathf.IsZeroApprox(velocity.X);
				
				if((bool)Dash.Call("isDashing"))
				{
					_speed = DashSpeed;
				}else{
					_speed = MovementSpeed;
				}
				
				if(Input.IsActionPressed("dash") && !IsDashPressed)
				{
					Dash.Call("startDash", DashLength);
					IsDashPressed = true;
				}
								
				if(Input.IsActionJustReleased("jump"))
				{
					IsJumpPressed = false;
					IsCoyoteTriggered = true;
				}
				
				if(Input.IsActionJustReleased("dash"))
				{
					IsDashPressed = false;
				}
				
				if(idle)
				{
					SpriteAnimation.Play("Idle");
					IsCoyoteTriggered = false;
					Jumped = false;
				}
				
				if(jump)
				{
					velocity.Y = -JumpStrength;
					IsJumpPressed = true;
				}else if(jumpCanclled)
				{
					velocity.Y = 0;
				}
				
				if(CeilingCheck.IsColliding())
				{
					velocity.Y = (float)(Gravity*delta);
				}

				if(jump && !Jumped)
				{
					SpriteAnimation.Play("Jump");
					Jumped = true;
				}else if(falling && !IsCoyoteTriggered)
				{
					CoyoteTime.Call("startTime", CoyoteDuration);
					SpriteAnimation.Play("Falling");
					IsCoyoteTriggered = true;
				}else if(Mathf.Abs(velocity.X) > 0 && IsOnFloor())
				{
					SpriteAnimation.Play("Run");
					IsCoyoteTriggered = false;
					Jumped = false;
				}
				
				velocity.X = (Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left")) * _speed;
				velocity.Y += (float)(Gravity*delta);
				
				if((IsOnFloor() && !jump) || !_camera.Enabled)
				{
					velocity.Y = 0;
				}
				if(velocity.X > 0)
				{
					SpriteAnimation.FlipH = false;
				}else if(velocity.X < 0)
				{
					SpriteAnimation.FlipH = true;
				}
				
				Velocity = velocity;
				this.Position = new Vector2(0, 0);
				MoveAndSlide();
				damage();
				shoot();
			}
		}
		
		public	void damage(){
			for(int i = 0; i < GetSlideCollisionCount(); i++){
				KinematicCollision2D collision = GetSlideCollision(i);
				if((collision.GetCollider() as Node).GetParent().Name == "Enemy" && takeDamageTimer.IsStopped()){
					takeDamageTimer.WaitTime = 1F;
					takeDamageTimer.Start();
					Godot.Collections.Array arguments = new Godot.Collections.Array { 10 };
					root.Call("callRcp", 2, arguments);
				}
			}
		}
		
		public Vector2 getVelocity(){
			return velocity;
		}
		
		public void setActive(bool active){
			this.Active = active;
			collisionShape.SetDeferred("disabled", !active);
		}
		
		public void refill(bool has){
			Bullets = has;
		}
		
		public void die(){
			Dead = true;
			SetProcess(false);
			SpriteAnimation.AnimationFinished += finish;
			SpriteAnimation.Play("Death");
		}
		
		public void shoot()
		{
			if(Input.IsActionPressed("shoot") && !Shoot && Bullets)
			{
				Shoot = true;
				_bullet = (bullet)_bulletScene.Instantiate();
				AddChild(_bullet);
				Vector2 mousePosition = GetGlobalMousePosition();
				float angle = Mathf.Atan2(mousePosition.Y - this.GlobalPosition.Y, mousePosition.X - this.GlobalPosition.X);
				_bullet.Rotation = Mathf.DegToRad(Mathf.RadToDeg(angle)-90);
				Godot.Collections.Array arguments = new Godot.Collections.Array { Bullets };
				root.Call("callRcp", 3, arguments);
				
			}else if(Input.IsActionJustReleased("shoot")){
					Shoot = false;
			}			
		}
		
		public void finish(){
			GetTree().Paused = true;
		}
}
