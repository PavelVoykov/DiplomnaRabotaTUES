using Godot;
using System;

public partial class dash : Node2D
{
	Timer dashTimer;
	Timer dashDelayTimer;
	public override void _Ready()
	{
		dashTimer = GetNode<Timer>("Dash");
		dashDelayTimer = GetNode<Timer>("DashDelay");
	}

	public void startDash(float duration){
		
		if(dashDelayTimer.IsStopped()){
			dashTimer.WaitTime = duration;
			dashTimer.Start();
			dashDelayTimer.WaitTime = 1F;
			dashDelayTimer.Start();
		}
	}
	
	public bool isDashing()
	{
		return !dashTimer.IsStopped();
	}
}
