using Godot;
using System;

public partial class body_double : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}
	public bool Overlapps(){
		if(HasOverlappingBodies()){
			return true;
		}
		return false;
	}
}
