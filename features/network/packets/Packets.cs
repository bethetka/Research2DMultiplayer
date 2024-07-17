using System.Collections.Generic;
using Godot;
using Research.Console;
using Research.Network.Packets.C2S;
using Research.Network.Packets.S2C;

namespace Research.Network.Packets;

public static class Packets
{
    public static readonly int Authenticate = 0;
    public static readonly int Hello = 1; 
    public static readonly int NewPlayer = 2;
    public static readonly int PlayerDisconnected = 3; 
    public static readonly int Disconnect = 4; 
    public static readonly int Move = 5; 
    public static readonly int Ping = 6; 
    public static readonly int Pong = 7; 

    public static List<IC2SPacket> C2SPackets = new List<IC2SPacket>();
    public static List<IS2CPacket> S2CPackets = new List<IS2CPacket>();

    static Packets()
    {
        C2SPackets.Add(new C2SAuthenticatePacket());
        C2SPackets.Add(new C2SPlayerDisconnect());
        S2CPackets.Add(new S2CHelloPacket());
        S2CPackets.Add(new S2CNewPlayerPacket());
        S2CPackets.Add(new S2CPlayerDisconnectedPacket());
        C2SPackets.Add(new C2SMovePacket());
        S2CPackets.Add(new S2CMovePacket());
        C2SPackets.Add(new C2SPingPacket());
        S2CPackets.Add(new S2CPongPacket());
    }

    public static void SendPacket(IC2SPacket packet, Client client)
    {
        if (!(packet is C2SMovePacket) && !(packet is C2SPingPacket))
            ResearchConsole.Instance.Info($"sending packet {packet.GetFriendlyName()}");
        client.Peer.Put32(packet.GetId());
        var variant = packet.ToVariant();
        client.Peer.PutVar(variant);
    }

    public static void SendPacket(IS2CPacket packet, ServerPlayer to)
    {
        to.Peer.Put32(packet.GetId());
        var variant = packet.ToVariant();
        to.Peer.PutVar(variant);
    }
    
    public static void Broadcast(Server server, IS2CPacket packet)
    {
        foreach (var serverPlayer in server.Players)
        {
            SendPacket(packet, serverPlayer);
        }
    }
}