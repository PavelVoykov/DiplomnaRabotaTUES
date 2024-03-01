using Godot;
using System;

public partial class multiplayer : Node2D
{
	public ENetMultiplayerPeer Peer;
	private PackedScene _managerNode;
	private PackedScene _mapsNode;
	private Camera2D _camera;
	private map_switch _mapsSwitch;
	private manager _manager;
	private long _newPeerId;
	
	public override void _Ready()
	{
		Peer = new ENetMultiplayerPeer();
		_managerNode = ResourceLoader.Load<PackedScene>("res://assets/characters/manager/manager.tscn");
		_mapsNode = ResourceLoader.Load<PackedScene>("res://scenes/maps.tscn");
		_camera = GetNode<Camera2D>("Camera2D");
		GetNode<Control>("Host").GrabFocus();
	}
	
	private void _on_host_pressed()
	{
		Error err = Peer.CreateServer(123);
		
		if (err != Error.Ok)
		{
			return;
		}
		
		Multiplayer.MultiplayerPeer = Peer;
		Multiplayer.PeerConnected += SetNewPeerId;
		
		AddPlayer();
		
		GetNode<Control>("Host").ReleaseFocus();
		_camera.Enabled = false;
		GetNode<Control>("Host").Visible = false;
		GetNode<Control>("Join").Visible = false;
	}
		
	private void _on_join_pressed()
	{
		Multiplayer.ConnectedToServer += Connected;
		
		AddMenager();
		
		Multiplayer.ConnectionFailed += Fail;
		
		Error err = Peer.CreateClient("localhost", 123);
		
		if (err != Error.Ok)
		{
			GD.Print("Failed to connect to server: " + err);
			return;
		}
		
		Multiplayer.MultiplayerPeer = Peer;
		
		GetNode<Control>("Join").ReleaseFocus();
		_camera.Enabled = false;
		GetNode<Control>("Host").Visible = false;
		GetNode<Control>("Join").Visible = false;
	}

	private void AddPlayer(int id=1)
	{
		_mapsSwitch = (map_switch)_mapsNode.Instantiate();
		_mapsSwitch.Name = "Maps";
		
		AddChild(_mapsSwitch);
	}
	
	private void AddMenager()
	{
		_manager = (manager)_managerNode.Instantiate();
		_manager.Name = "Manager";
		AddChild(_manager);
	}
	
	public void callRcp(int type, Godot.Collections.Array arguments)
	{
		switch(type)
		{
			case 1:
				Multiplayer.Rpc(1, this, "switchMap", arguments);
				break;
			case 2:
				Multiplayer.Rpc((int)_newPeerId, this, "takeDamage", arguments);
				break;
			case 3:
				Multiplayer.Rpc((int)_newPeerId, this, "shoot", arguments);
				break;
			case 4:
				Multiplayer.Rpc(1, this, "die", arguments);
				break;
			case 5:
				Multiplayer.Rpc(1, this, "hasBulletsToggle", arguments);
				break;
		}
		
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void switchMap(int num)
	{
		Node2D node = GetNode<Node2D>("Maps");
		node.Call("map"+num.ToString());
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void die()
	{
		Node2D node = GetNode<Node2D>("Maps");
		node.Call("die");
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void hasBulletsToggle(bool hasBullets)
	{
		Node2D node = GetNode<Node2D>("Maps");
		node.Call("hasBullets", hasBullets);
	}
	
	[Rpc(MultiplayerApi.RpcMode.Authority)]
	public void takeDamage(int num)
	{
		Node2D node = GetNode<Node2D>("Manager");
		node.Call("damage", num);
	}
	
	[Rpc(MultiplayerApi.RpcMode.Authority)]
	public void shoot(int num)
	{
		Node2D node = GetNode<Node2D>("Manager");
		node.Call("shoot", num);
	}
	
	private void Connected()
	{
		GD.Print("Connected");
	}
	
	private void Fail()
	{
		GD.Print("Connection failed");
	}
	
	private void SetNewPeerId(long id)
	{
		_newPeerId = id;
	}
	
}



