using Godot;
using System;

public partial class player: CharacterBody2D{

		[Export]
		public float CoyoteDuration {get; set;} = .1F;
		//The duration of the coyote time
		[Export]
		public int MovementSpeed {get; set;} = 2000;
		//The default walking speed of the player
		[Export]
		public int JumpStrength {get; set;} = 4250;
		//The strength of the player's jump
		[Export]
		public float DashLength {get; set;} = .3F;
		//The duration of the dash ability
		[Export]
		public int DashSpeed {get; set;} = 4000;
		//The movement speed of the player for the duration of the dash
		[Export]
		public int Gravity {get; set;} = 10000;
		//The strength of the gravity afecting the player
		public AnimatedSprite2D SpriteAnimation;
		//Reference to the players animations
		public RayCast2D CeilingCheck;
		//Reference to the ray checking for collisions from above
		private Camera2D _camera;
		//Reference to the player's camera
		public Node2D CoyoteTime;
		public Node2D Dash;
		private PackedScene _bulletScene;
		private int _speed = 0;
		private bullet _bullet;
		private Godot.Collections.Array _keyNMouseInputs =  new Godot.Collections.Array{"move_left", "move_right", "jump", "dash", "shoot"};
		private Godot.Collections.Array _controllerInputs =  new Godot.Collections.Array{"move_left_controller", "move_right_controller", "jump_controller", "dash_controller", "shoot_controller"};
		
		public bool IsCoyoteTriggered = false;
		public bool KeyboardToggle = true;
		public bool IsJumpPressed = false;
		public bool IsDashPressed = false;
		public bool Bullets = true;
		public bool Active = false;
		public bool Jumped = false;
		public bool Shoot = false;
		public bool Dead = false;
		
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
				bool jump = (Input.IsActionPressed("jump") || Input.IsActionPressed("jump_controller")) && !IsJumpPressed && (IsOnFloor() || (bool)CoyoteTime.Call("isCoyote") && !Jumped);
				bool falling = !IsOnFloor() && Velocity.Y > 0.0;
				bool jumpCanceled = (Input.IsActionJustReleased("jump") || Input.IsActionJustReleased("jump_controller")) && Velocity.Y < 0.0;
				bool idle = IsOnFloor() && Mathf.IsZeroApprox(velocity.X);
				
				if((bool)Dash.Call("isDashing"))
				{
					_speed = DashSpeed;
				}else{
					_speed = MovementSpeed;
				}
				
				if((Input.IsActionPressed("dash") || Input.IsActionPressed("dash_controller")) && !IsDashPressed)
				{
					Dash.Call("startDash", DashLength);
					IsDashPressed = true;
				}
								
				if((Input.IsActionJustReleased("jump") || Input.IsActionJustReleased("jump_controller")))
				{
					IsJumpPressed = false;
					IsCoyoteTriggered = true;
				}
				
				if((Input.IsActionJustReleased("dash") || Input.IsActionJustReleased("dash_controller")))
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
				}else if(jumpCanceled)
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
				
				velocity.X = ((Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left"))+(Input.GetActionStrength("move_right_controller") - Input.GetActionStrength("move_left_controller"))) * _speed;
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
				KeyboardOrController();
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
		
		public void hasBullets(bool hasBulletsToggle){
			Bullets = hasBulletsToggle;
		}
		
		public void die(){
			Dead = true;
			SetProcess(false);
			SpriteAnimation.AnimationFinished += finish;
			SpriteAnimation.Play("Death");
		}
		
		public void finish(){
			GetTree().Paused = true;
		}
		
		public void shoot()
		{
			if((Input.IsActionPressed("shoot") || Input.IsActionPressed("shoot_controller")) && !Shoot && Bullets)
			{
				RandomNumberGenerator random = new RandomNumberGenerator();
					random.Randomize();
				Shoot = true;
				_bullet = (bullet)_bulletScene.Instantiate();
				AddChild(_bullet);
				//Bullet is shot a instance of the bulllet class is created and is added as a child of the player
				if(KeyboardToggle){
					Vector2 mousePosition = GetGlobalMousePosition();
					float angle = Mathf.Atan2(mousePosition.Y - this.GlobalPosition.Y, mousePosition.X - this.GlobalPosition.X);
					_bullet.Rotation = Mathf.DegToRad(Mathf.RadToDeg(angle)-90+random.RandiRange(-7, 7));
				}else if(!KeyboardToggle){
					Vector2 controllerDirection = new Vector2(Input.GetAxis("shootDirectionXN", "shootDirectionXP"), Input.GetAxis("shootDirectionYN", "shootDirectionYP"));
					float angle = Mathf.Atan2(controllerDirection.Y, controllerDirection.X);
					_bullet.Rotation = Mathf.DegToRad(Mathf.RadToDeg(angle)-90+random.RandiRange(-7, 7));
					GD.Print(_bullet.Rotation);
				}
				
				//The bullets rotation is changed to point at the mouse cursor
				Godot.Collections.Array arguments = new Godot.Collections.Array { Bullets };
				root.Call("callRcp", 3, arguments);
				//RPC call is made 
				
			}else if((Input.IsActionJustReleased("shoot") || Input.IsActionJustReleased("shoot_controller"))){
					Shoot = false;
			}			
		}
		public void KeyboardOrController(){
			for (int i =  0; i < _keyNMouseInputs.Count; i++)
			{
				if(Input.IsActionPressed((String)_keyNMouseInputs[i]))
				{
					KeyboardToggle = true;
				}
			}
			
			for (int i =  0; i < _controllerInputs.Count; i++)
			{
				if(Input.IsActionPressed((String)_controllerInputs[i]))
				{
					KeyboardToggle = false;
				}
			}
		}
}
