using Godot;
using Godot.Collections;

namespace Research.Network.Packets.C2S;

public class C2SMovePacket : IC2SPacket
{
    public Vector2 WishDir { get; set; }
    public C2SMovePacket() {}
    public C2SMovePacket(Vector2 wishDir)
    {
        WishDir = wishDir;
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
        WishDir = dictionary[nameof(WishDir)].AsVector2();
    }

    public Variant ToVariant()
    {
        return new Dictionary<string, Variant>
        {
            {nameof(WishDir), WishDir}
        };
    }

    public void Handle(Server server, ServerPlayer sender)
    {
        sender.Player.Movement.SetWishDir(WishDir);

        var packet = new S2CMovePacket(sender.Player.Id, WishDir, sender.Player.Position);
        foreach (var player in server.Players)
        {
            Packets.SendPacket(packet, player);
        }
    }
}