using System.Linq;
using Godot;
using Godot.Collections;
using Research.Console;

namespace Research.Network.Packets.C2S;

public class S2CMovePacket : IS2CPacket
{
    public int Id { get; set; }
    public Vector2 WishDir { get; set; }
    public Vector2 Position { get; set; }
    public S2CMovePacket() {}
    public S2CMovePacket(int id, Vector2 wishDir, Vector2 position)
    {
        Id = id;
        WishDir = wishDir;
        Position = position;
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
        Id = dictionary[nameof(Id)].AsInt32();
        WishDir = dictionary[nameof(WishDir)].AsVector2();
        Position = dictionary[nameof(Position)].AsVector2();
    }

    public Variant ToVariant()
    {
        return new Dictionary<string, Variant>
        {
            {nameof(Id), Id},
            {nameof(WishDir), WishDir},
            {nameof(Position), Position}
        };
    }

    public void Handle(Client client)
    {
        var player = client.Players.FirstOrDefault(i => i.Id == Id);
        if (player == null)
        {
            ResearchConsole.Instance.Warn($"Failed to process Move packet for player {Id}");
            return;
        }
        
        player.Movement.SetWishDir(WishDir);
        player.Position = Position;
    }
}