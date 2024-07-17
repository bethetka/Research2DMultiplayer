using System.Linq;
using Godot;
using Research.Game;
using ServerPlayerList = System.Collections.Generic.List<Research.Network.ServerPlayer>;

namespace Research.Network;

using Console;

public partial class Server : Node
{
    public enum Status
    {
        None,
        Listening
    }

    public static Server Instance;
    [Export] public PlayerManager PlayerManager;

    private TcpServer _listener;
    private Status _status;
    private ServerPlayerList _players;

    public override void _EnterTree()
    {
        base._EnterTree();
        Instance = this;
        _listener = new TcpServer();
        _status = Status.None;
        _players = new ServerPlayerList();

        if (GameArguments.IsServer())
        {
            Host();
        }
    }

    public void Host(ushort port = 6969)
    {
        Error err = _listener.Listen(port);
        if (err != Error.Ok)
        {
            ResearchConsole.Instance.Err($"Failed to host on port {port}: {err.ToString()}");
        }
        else
        {
            _status = Status.Listening;
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (_status == Status.Listening)
        {
            ProcessNewConnections();
            ProcessPlayers();
        }
    }

    public ServerPlayerList Players => _players;

    private void ProcessNewConnections()
    {
        if (_listener.IsConnectionAvailable())
        {
            var connection = _listener.TakeConnection();
            ResearchConsole.Instance.Info($"new player connected from {connection.GetConnectedHost()}.");
            connection.SetNoDelay(true);
            var player = new ServerPlayer()
            {
                Status = ServerPlayer.ServerPlayerStatus.Authenticating,
                Player = null,
                Peer = connection
            };
            _players.Add(player);
        }
    }

    private void ProcessPlayers()
    {
        foreach (var player in _players.ToList())
        {
            Error e = player.Peer.Poll();
            if (e != Error.Ok)
            {
                player.Kick(this, "disconnected");
                continue;
            }

            if (player.Peer.GetStatus() != StreamPeerTcp.Status.Connected)
            {
                player.Kick(this, "disconnected");
                continue; 
            }
            
            ProcessPlayer(player);
        }
    }

    public int IdCounter = 0;

    private void ProcessPlayer(ServerPlayer player)
    {
        if (player.Peer.GetAvailableBytes() >= 4)
        {
            int packetId = player.Peer.Get32();

            foreach (var packet in Packets.Packets.C2SPackets)
            {
                if (packet.GetId() == packetId)
                {
                    ResearchConsole.Instance.Info($"[player {(player.Player == null ? player.Peer.GetConnectedHost() : player.Player.Id)}] Received packet {packet.GetFriendlyName()}");
                    var variant = player.Peer.GetVar();
                    packet.FromVariant(variant);
                    packet.Handle(this, player);
                }
            }
        }
    }
}