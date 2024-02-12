using Godot;
using System;

public partial class body_double : Area2D
{
	public bool Overlapps()
	{
		if(HasOverlappingBodies())
		{
			return true;
		}
		return false;
	}
}
