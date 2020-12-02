using Godot;
using System;
using System.Collections.Generic;

public class Network : Node
{
    public static Network Instance { get; private set; }

    [Signal]
    public delegate void ServerCreated();

	[Signal]
    public delegate void JoinSuccess();

	[Signal]
    public delegate void JoinFailed();



    public int MaxPlayers = 4;
    public static int Port {get; } = 4546;

    public override void _Ready()
    {
        base._Ready();

        Instance = this;

        // Connect to key networking signals
        GetTree().Connect("network_peer_connected", this, nameof(_on_player_connected));
        GetTree().Connect("network_peer_disconnected", this, nameof(_on_player_disconnected));
        GetTree().Connect("connected_to_server", this, nameof(_on_connected_to_server));
        GetTree().Connect("server_disconnected", this, nameof(_on_disconnected_from_server));
        GetTree().Connect("connection_failed", this, nameof(_on_connection_failed));
    }

    public void CreateServer() {
        var net = new NetworkedMultiplayerENet();

        if (net.CreateServer(Port) != Error.Ok){
            GD.Print("Failed to create server");
            return;
        }

        GetTree().NetworkPeer = net;
        EmitSignal(nameof(ServerCreated));
    }

    public void JoinServer(string ip, int port) {
        var net = new NetworkedMultiplayerENet();

        if (net.CreateClient(ip, port) != Error.Ok) {
            GD.Print("Failed to connect to server");
            return;
        }

        GetTree().NetworkPeer = net;
        EmitSignal(nameof(JoinSuccess));
    }


    // Called on EVERYONE when a player connects
    private void _on_player_connected(int id) {

    }

    // Called on EVERYONE when a player is disconnected
    private void _on_player_disconnected(int id) {

    }

    // Called on client when successfully connected
    private void _on_connected_to_server() {

    }

    // Called on client when it disconnects
    private void _on_disconnected_from_server() {

    }

    // Called on client when connection attempt failed
    private void _on_connection_failed() {
        GetTree().NetworkPeer = null;
        EmitSignal(nameof(JoinFailed));
    }
}
