using Godot;
using System;

public partial class bullet : RayCast2D
{
	public override void _Ready()
	{
	}
	public override void _PhysicsProcess(double delta){
		if(this.IsColliding()){
			Vector2 coordinates = this.GetCollisionPoint();
			Object collider = this.GetCollider();
			if (collider is RigidBody2D nodeCollider)
			{
				nodeCollider.QueueFree();
			}
			this.QueueFree();
			
		}
		if(delta > 0){
			this.QueueFree();
		}
	}
}
