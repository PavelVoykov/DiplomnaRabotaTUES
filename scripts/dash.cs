using Godot;
using System;

public partial class dash : Node2D
{
	Timer timer;
	public override void _Ready()
	{
		timer = GetNode<Timer>("Timer");
	}

	public void startDash(float duration){
		timer.WaitTime = duration;
		timer.Start();
	}
	public bool isDashing(){
		return !timer.IsStopped();
	}
}
