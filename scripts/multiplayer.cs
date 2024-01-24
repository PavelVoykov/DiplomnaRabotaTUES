using Godot;
using System;

public partial class multiplayer : Node2D
{
	public ENetMultiplayerPeer peer;
	private PackedScene maps_node;
	private map_switch maps;
	private Camera2D camera;
	[Signal]
	public delegate void PlayerConnectedDelegateEventHandler(int id);
	public event PlayerConnectedDelegateEventHandler PlayerConnected;
	public override void _Ready()
	{
		peer = new ENetMultiplayerPeer();
		maps_node = ResourceLoader.Load<PackedScene>("res://scenes/node_2d.tscn");
		camera = GetNode<Camera2D>("Camera2D");
	}
	
	private void _on_host_pressed(){
		GD.Print("Hiiii");
		peer.CreateServer(123);
		Multiplayer.MultiplayerPeer = peer;
		PlayerConnected += AddPlayer;
		AddPlayer();
		GetNode<Control>("Host").ReleaseFocus();
		camera.Enabled = false;
	}
	private void AddPlayer(int id = 11){
		maps = (map_switch)maps_node.Instantiate();
		maps.Name = id.ToString();
		AddChild(maps);
	}
	private void _on_join_pressed(){
	peer.CreateClient("127.0.0.1", 135);
	Multiplayer.MultiplayerPeer = peer;
	GetNode<Control>("Join").ReleaseFocus();
	}
}











