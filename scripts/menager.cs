using Godot;
using System;

public partial class menager : Node2D
{
	
	Camera2D camera;
	private Node2D root;
	private int health_points = 100 ;
	Label Health;
	private int bullet_count = 100 ;
	Label Bullets;
	public override void _Ready()
	{
		root = (Node2D)GetParent();	
		Health = GetNode<Label>("Health");
		Health.Text = health_points.ToString();
		Bullets = GetNode<Label>("Bullets");
		Bullets.Text = bullet_count.ToString();
	}
	public override void _EnterTree(){
		camera = GetNode<Camera2D>("Camera2D");
		camera.Enabled = true;
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
	private void _on_map_1_pressed()
	{
		Godot.Collections.Array arguments = new Godot.Collections.Array { 1 };
		root.Call("callRcp", 1, arguments);
	}
	private void _on_map_2_pressed()
	{
		Godot.Collections.Array arguments = new Godot.Collections.Array { 2 };
		root.Call("callRcp", 1, arguments);
	}


	private void _on_map_3_pressed()
	{
		Godot.Collections.Array arguments = new Godot.Collections.Array { 3 };
		root.Call("callRcp", 1, arguments);
	}


	private void _on_map_4_pressed()
	{
		Godot.Collections.Array arguments = new Godot.Collections.Array { 4 };
		root.Call("callRcp", 1, arguments);
	}
	
	private void _on_health_drop_pressed()
	{
		health_points+=50;
		Health.Text = health_points.ToString();
	}
	
	private void _on_ammo_drop_pressed()
	{
		bullet_count+=100;
		Bullets.Text = bullet_count.ToString();
		Godot.Collections.Array arguments = new Godot.Collections.Array { true };
		root.Call("callRcp", 5, arguments);
	}
	
	public void damage(int num){
		health_points -= num;
		Health.Text = health_points.ToString();
		if(health_points <= 0){
			Godot.Collections.Array arguments = new Godot.Collections.Array { };
			root.Call("callRcp", 4, arguments);
			GetTree().Paused = true;
		}
	}
	public void shoot(int num){
		bullet_count -= num;
		Bullets.Text = bullet_count.ToString();
		if(bullet_count <= 0){
			Godot.Collections.Array arguments = new Godot.Collections.Array { false };
			root.Call("callRcp", 5, arguments);
		}
	}
}






