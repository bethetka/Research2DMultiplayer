using System;
using Godot;
using ImGuiNET;
using Research.Console;
using Research.Game;
using Research.Network.Packets.C2S;
using Research.Player;
using Vector2 = System.Numerics.Vector2;

namespace Research.Network;

using PlayerList = System.Collections.Generic.List<Player.Player>;


public partial class Client : Node
{
    public enum Status
    {
        None,
        Authenticating,
        Spawning,
        Playing
    }

    public static Client Instance;
    public long ServerPing = 0;
    [Export] public PlayerManager PlayerManager;
    private StreamPeerTcp _client;
    private PlayerList _players;
    public Player.Player LocalPlayer;
    public Status ClientStatus;
    
    public StreamPeerTcp Peer => _client;
    public PlayerList Players => _players;
    public string Username { get; set; } = "defaultuser0";

    public override void _EnterTree()
    {
        base._EnterTree();
        Instance = this;
        _client = new StreamPeerTcp();
        ClientStatus = Status.None;
        _players = new PlayerList();
    }

    [ConsoleCommand("connect")]
    public static void Connect(string host, ushort port)
    {
        Error e = Instance._client.ConnectToHost(host, port);
        if (e != Error.Ok)
        {
            ResearchConsole.Instance.Err($"Failed to connect: {e.ToString()}");
            return;
        }

        ResearchConsole.Instance.Info($"Connecting to {host}:{port}.");
        Instance.ClientStatus = Status.Authenticating;
    }

    [ConsoleCommand("disconnect")]
    public static void DisconnectFromHost()
    {
        Instance._client.DisconnectFromHost();
        Instance.ClientStatus = Status.None;
        Instance.Players.ForEach(i => i.QueueFree());
    }

    private double _pingPongTime = 0;
    
    public override void _Process(double delta)
    {
        base._Process(delta);
        if (ClientStatus != Status.None)
        {
            Error e = _client.Poll();
            if (e != Error.Ok)
            {
                ResearchConsole.Instance.Err("failed to poll info about client, disconnecting from server");
                _client.DisconnectFromHost();
                ClientStatus = Status.None;
                return;
            }

            if (_client.GetStatus() != StreamPeerTcp.Status.Connected)
            {
                return;
            }
            ImGui.SetNextWindowPos(new Vector2(16,16));
            ImGui.Begin("Client Info", ImGuiWindowFlags.AlwaysAutoResize);
                ImGui.Text($"Ping: {ServerPing}");
            ImGui.End();
            
            if (ClientStatus == Status.Authenticating)
            {
                Authenticate(Username);
            }

            if (_pingPongTime >= 1)
            {
                var packet = new C2SPingPacket(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
                Packets.Packets.SendPacket(packet, this);
                _pingPongTime = 0;
            }

            _pingPongTime += delta;

            ProcessPackets();
        }
    }

    private void ProcessPackets()
    {
        // >= because if just > we can skip packets without data in it, like on v1 packet C2SDisconnect, that
        // sends just id
        if (Peer.GetAvailableBytes() >= 4)
        {
            int packetId = Peer.Get32();

            foreach (var packet in Packets.Packets.S2CPackets)
            {
                if (packet.GetId() == packetId)
                {
                    if (!(packet is S2CMovePacket) && !(packet is S2CPongPacket))
                        ResearchConsole.Instance.Info($"Received packet {packet.GetFriendlyName()}");
                    var variant = Peer.GetVar();
                    packet.FromVariant(variant);
                    packet.Handle(this);
                }
            }
        }
    }


    private void Authenticate(string username)
    {
        var packet = new C2SAuthenticatePacket(username);
        Packets.Packets.SendPacket(packet, this);
        ResearchConsole.Instance.Info($"authenticating with username {username}");
        ClientStatus = Status.Spawning;
    }
}