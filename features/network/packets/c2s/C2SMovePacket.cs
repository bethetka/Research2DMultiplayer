using Godot;
using Godot.Collections;
using Research.Network.Packets.S2C;

namespace Research.Network.Packets.C2S;

public class C2SMovePacket : IC2SPacket
{
    public C2SMovePacket() {}
    
    public uint Sequence { get; set; }
    public Vector2 Direction { get; set; }

    public C2SMovePacket(uint sequence, Vector2 direction)
    {
        Sequence = sequence;
        Direction = direction;
    }

    
    public int GetId()
    {
        return Packets.Move;
    }
    
    public string GetFriendlyName()
    {
        return GetType().Name;
    }

    public void FromVariant(Variant variant)
    {
        var dictionary = variant.AsGodotDictionary<string, Variant>();
        Direction = dictionary[nameof(Direction)].AsVector2();
        Sequence = dictionary[nameof(Sequence)].AsUInt32();
    }

    public Variant ToVariant()
    {
        return new Dictionary<string, Variant>
        {
            {nameof(Direction), Direction},
            {nameof(Sequence), Sequence}
        };
    }

    public void Handle(Server server, ServerPlayer sender)
    {
        sender.Player.Movement.SetWishDir(Direction);
        sender.LastProcessedInput = Sequence;
        sender.LastProcessedPosition = sender.Player.Position;

        // Broadcast the move to other players
        var packet = new S2CMovePacket(sender.Player.Id, Direction, sender.LastProcessedPosition, Sequence);
        foreach (var otherPlayer in server.Players)
        {
            if (otherPlayer != sender)
            {
                Packets.SendPacket(packet, otherPlayer);
            }
        }
    }
}