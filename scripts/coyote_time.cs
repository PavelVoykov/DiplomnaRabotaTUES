using Godot;
using System;

public partial class coyote_time : Node2D
{
	Timer timer;
	public override void _Ready()
	{
		timer = GetNode<Timer>("CoyoteTimer");
	}

	public void startTime(float duration)
	{
		timer.WaitTime = duration;
		timer.Start();
	}
	
	public bool isCoyote()
	{
		return !timer.IsStopped();
	}
}
