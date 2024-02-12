using Godot;
using System;

public partial class manager : Node2D
{
	

	private int _healthPoints = 100;
	private int _bulletCount = 100;
	private Node2D _root;
	Camera2D camera;
	Label bullets;
	Label health;
	
	public override void _Ready()
	{
		_root = (Node2D)GetParent();	
		health = GetNode<Label>("Health");
		bullets = GetNode<Label>("Bullets");
		health.Text = _healthPoints.ToString();
		bullets.Text = _bulletCount.ToString();
	}
	public override void _EnterTree(){
		camera = GetNode<Camera2D>("Camera2D");
		camera.Enabled = true;
	}
	
	private void _on_map_1_pressed()
	{
		Godot.Collections.Array arguments = new Godot.Collections.Array { 1 };
		_root.Call("callRcp", 1, arguments);
	}
	
	private void _on_map_2_pressed()
	{
		Godot.Collections.Array arguments = new Godot.Collections.Array { 2 };
		_root.Call("callRcp", 1, arguments);
	}

	private void _on_map_3_pressed()
	{
		Godot.Collections.Array arguments = new Godot.Collections.Array { 3 };
		_root.Call("callRcp", 1, arguments);
	}

	private void _on_map_4_pressed()
	{
		Godot.Collections.Array arguments = new Godot.Collections.Array { 4 };
		_root.Call("callRcp", 1, arguments);
	}
	
	private void _on_health_drop_pressed()
	{
		_healthPoints+=50;
		health.Text = _healthPoints.ToString();
	}
	
	private void _on_ammo_drop_pressed()
	{
		_bulletCount+=100;
		bullets.Text = _bulletCount.ToString();
		Godot.Collections.Array arguments = new Godot.Collections.Array { true };
		_root.Call("callRcp", 5, arguments);
	}
	
	public void damage(int num)
	{
		_healthPoints -= num;
		health.Text = _healthPoints.ToString();
		if(_healthPoints <= 0){
			Godot.Collections.Array arguments = new Godot.Collections.Array { };
			_root.Call("callRcp", 4, arguments);
			GetTree().Paused = true;
		}
	}
	public void shoot(int num)
	{
		_bulletCount -= num;
		bullets.Text = _bulletCount.ToString();
		if(_bulletCount <= 0){
			Godot.Collections.Array arguments = new Godot.Collections.Array { false };
			_root.Call("callRcp", 5, arguments);
		}
	}
}






