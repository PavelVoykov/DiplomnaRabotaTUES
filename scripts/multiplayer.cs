using Godot;
using System;

public partial class multiplayer : Node2D
{
	public ENetMultiplayerPeer peer;
	private PackedScene player;
	private turn_on pinst;
	[Signal]
	public delegate void PlayerConnectedDelegateEventHandler(int id);
	public event PlayerConnectedDelegateEventHandler PlayerConnected;
	public Node2D node;
	public override void _Ready()
	{
		peer = new ENetMultiplayerPeer();
		player = ResourceLoader.Load<PackedScene>("res://assets/characters/player/player.tscn");
		node = GetNode<Node2D>("Node2D");
	}
	
	private void _on_host_pressed(){
		GD.Print("Hiiii");
		peer.CreateServer(123);
		Multiplayer.MultiplayerPeer = peer;
		PlayerConnected += AddPlayer;
		AddPlayer();
		GetNode<Control>("Host").ReleaseFocus();
		node.ProcessMode = Node.ProcessModeEnum.Inherit;
	}
	private void AddPlayer(int id = 1){
		pinst = (turn_on)player.Instantiate();
		pinst.Name = id.ToString();
		AddChild(pinst);
	}
	private void _on_join_pressed(){
	peer.CreateClient("127.0.0.1", 135);
	Multiplayer.MultiplayerPeer = peer;
	GetNode<Control>("Join").ReleaseFocus();
	}
}












