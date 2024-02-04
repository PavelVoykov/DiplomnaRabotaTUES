using Godot;
using System;

public partial class multiplayer : Node2D
{
	public ENetMultiplayerPeer peer;
	private PackedScene maps_node;
	private PackedScene menager_node;
	private menager menager;
	private map_switch maps;
	private Camera2D camera;
	LineEdit message;
	TextEdit chat;
	public override void _Ready()
	{
		peer = new ENetMultiplayerPeer();
		maps_node = ResourceLoader.Load<PackedScene>("res://scenes/node_2d.tscn");
		menager_node = ResourceLoader.Load<PackedScene>("res://assets/characters/menager/menager.tscn");
		camera = GetNode<Camera2D>("Camera2D");
		message = GetNode<LineEdit>("Input");
		chat = GetNode<TextEdit>("TextEdit");
	}
	
	private void _on_host_pressed(){
		GD.Print("Hiiii");
		Error err = peer.CreateServer(123);
		if (err != Error.Ok) {
			GD.Print("Failed to create server: " + err);
			return;
		}else{
			GD.Print("Created");
		}
		Multiplayer.MultiplayerPeer = peer;
		Multiplayer.PeerConnected += AddMenager;
		
		AddPlayer();
		GetNode<Control>("Host").ReleaseFocus();
		camera.Enabled = false;
	}
	private void AddPlayer(int id=1){
		maps = (map_switch)maps_node.Instantiate();
		maps.Name = "Node2D";//id.ToString();
		AddChild(maps);
	}
	private void _on_join_pressed(){
	Multiplayer.ConnectedToServer += Connected;
	Multiplayer.ConnectionFailed += Fail;
	Error err = peer.CreateClient("localhost", 123);
	if (err != Error.Ok) {
		GD.Print("Failed to connect to server: " + err);
		return;
	}
	
	Multiplayer.MultiplayerPeer = peer;
	GetNode<Control>("Join").ReleaseFocus();
	camera.Enabled = false;
	}
	private void AddMenager(long id){
		menager = (menager)menager_node.Instantiate();
		menager.Name = id.ToString();
		AddChild(menager);
	}
	private void _on_button_pressed()
	{
		Godot.Collections.Array arguments = new Godot.Collections.Array { message.Text };
		GD.Print(arguments);
		Multiplayer.Rpc(1, this, "msg", arguments);
	}
	[Rpc]
	public void msg(String mess){
		chat.Text += mess + '\n';
	}
	private void Connected(){
		GD.Print("Connected");
	}
	private void Fail(){
		GD.Print("Connection failed");
	}
}



