using Godot;
using System;

public partial class dash : Node2D
{
	Timer timer;
	Timer timer2;
	public override void _Ready()
	{
		timer = GetNode<Timer>("Timer");
		timer2 = GetNode<Timer>("Timer2");
	}

	public void startDash(float duration){
		
		if(timer2.IsStopped()){
			timer.WaitTime = duration;
			timer.Start();
			timer2.WaitTime = 1F;
			timer2.Start();
		}
	}
	public bool isDashing(){
		return !timer.IsStopped();
	}
}
