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
	long new_peer_id;
	public override void _Ready()
	{
		peer = new ENetMultiplayerPeer();
		maps_node = ResourceLoader.Load<PackedScene>("res://scenes/node_2d.tscn");
		menager_node = ResourceLoader.Load<PackedScene>("res://assets/characters/menager/menager.tscn");
		camera = GetNode<Camera2D>("Camera2D");
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
		Multiplayer.PeerConnected += setNewPeerId;
		AddPlayer();
		GetNode<Control>("Host").ReleaseFocus();
		camera.Enabled = false;
	}
	private void AddPlayer(int id=1){
		maps = (map_switch)maps_node.Instantiate();
		maps.Name = "Node2D";//id.ToString();
		AddChild(maps);
	}
	private void setNewPeerId(long id){
		new_peer_id = id;
	}
	private void _on_join_pressed(){
	Multiplayer.ConnectedToServer += Connected;
	AddMenager();
	Multiplayer.ConnectionFailed += Fail;
	Error err = peer.CreateClient("95.87.219.69", 123);
	if (err != Error.Ok) {
		GD.Print("Failed to connect to server: " + err);
		return;
	}
	
	Multiplayer.MultiplayerPeer = peer;
	GetNode<Control>("Join").ReleaseFocus();
	camera.Enabled = false;
	}
	private void AddMenager(long id = 2){
		menager = (menager)menager_node.Instantiate();
		menager.Name = id.ToString();
		AddChild(menager);
	}
	public void callRcp(int type, Godot.Collections.Array arguments){
		if(type == 1){
			Multiplayer.Rpc(1, this, "mapSwitch", arguments);	
		}else if(type == 2){
			Multiplayer.Rpc((int)new_peer_id, this, "damage", arguments);
		}else if(type == 3){
			Multiplayer.Rpc((int)new_peer_id, this, "shoot", arguments);
		}else if(type == 4){
			Multiplayer.Rpc(1, this, "death", arguments);
		}else if(type == 5){
			Multiplayer.Rpc(1, this, "hasBullets", arguments);
		}
		
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void mapSwitch(int num){
		Node2D node = GetNode<Node2D>("Node2D");
		node.Call("map"+num.ToString());
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void death(){
		Node2D node = GetNode<Node2D>("Node2D");
		node.Call("die");
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void hasBullets(bool has){
		Node2D node = GetNode<Node2D>("Node2D");
		node.Call("refill", has);
	}
	
	[Rpc(MultiplayerApi.RpcMode.Authority)]
	public void damage(int num){
		Node2D node = GetNode<Node2D>("2");
		node.Call("damage", num);
	}
	
	[Rpc(MultiplayerApi.RpcMode.Authority)]
	public void shoot(int num){
		Node2D node = GetNode<Node2D>("2");
		node.Call("shoot", num);
	}
	private void Connected(){
		GD.Print("Connected");
	}
	private void Fail(){
		GD.Print("Connection failed");
	}
}



