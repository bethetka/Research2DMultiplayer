using System.Linq;
using Godot;
using Research.Console;
using Research.Network.Packets.S2C;

namespace Research.Network.Packets.C2S;

public class C2SPlayerDisconnect : IC2SPacket
{
    public int GetId()
    {
        return Packets.Disconnect;
    }

    public string GetFriendlyName()
    {
        return GetType().Name;
    }

    public void FromVariant(Variant variant)
    {
        
    }

    public Variant ToVariant()
    {
        return Variant.From("");
    }

    public void Handle(Server server, ServerPlayer sender)
    {
        var packet = new S2CPlayerDisconnectedPacket(sender.Player.Id, "Player disconnected.");
        Packets.Broadcast(server, packet);

        if (sender.Status == ServerPlayer.ServerPlayerStatus.Playing)
        {
            sender.Player.QueueFree();
        }
    }
}