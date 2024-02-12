using Godot;
using System;

public partial class bullet : RayCast2D
{
	public override void _PhysicsProcess(double delta)
	{
		if(this.IsColliding())
		{
			Vector2 coordinates = this.GetCollisionPoint();
			Object collider = this.GetCollider();
			Node shape = this.GetCollider() as Node;
			
			if(shape.GetNodeOrNull("CollisionShape2D") != null)
			{
				GD.Print(shape.GetNode("CollisionShape2D"));
			}
			this.QueueFree();
		}
		
		if(delta > 0)
		{
			this.QueueFree();
		}
	}
}
