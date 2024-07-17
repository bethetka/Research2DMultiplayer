using Godot;
using Research.Network.Packets.S2C;

namespace Research.Network;

using Player;

public class ServerPlayer
{
    public enum ServerPlayerStatus
    {
        Authenticating,
        Playing
    }
    
    public StreamPeerTcp Peer;
    public ServerPlayerStatus Status;
    public Player Player;

    public void Kick(Server server, string reason)
    {
        server.Players.Remove(this);
        if (Status == ServerPlayerStatus.Playing)
        {
            Player.QueueFree();
            // TODO: trigger gamemode implementation like PlayerKicked, to process some cringe
            var packet = new S2CPlayerDisconnectedPacket(Player.Id, reason);
            foreach (var player in server.Players)
            {
                Packets.Packets.SendPacket(packet, player);
            }
        }
        Peer.DisconnectFromHost();

    }
}
