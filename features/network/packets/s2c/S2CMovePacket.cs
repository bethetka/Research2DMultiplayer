using System.Linq;
using Godot;
using Godot.Collections;
using Research.Console;

namespace Research.Network.Packets.S2C;

public class S2CMovePacket : IS2CPacket
{
    public int Id { get; set; }
    public Vector2 WishDir { get; set; }
    public Vector2 Position { get; set; }
    public uint Sequence { get; set; }

    public S2CMovePacket() {}
    public S2CMovePacket(int id, Vector2 wishDir, Vector2 position, uint sequence)
    {
        Id = id;
        WishDir = wishDir;
        Position = position;
        Sequence = sequence;
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
        Sequence = (uint)dictionary[nameof(Sequence)].AsInt64();
    }

    public Variant ToVariant()
    {
        return new Dictionary<string, Variant>
        {
            {nameof(Id), Id},
            {nameof(WishDir), WishDir},
            {nameof(Position), Position},
            {nameof(Sequence), Sequence}
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
        
        if (player == client.LocalPlayer)
        {
            // This is an update for the local player, perform reconciliation
            player.Movement.ReconcileState(Position, Sequence);
        }
        else
        {
            // This is an update for a remote player, update their position and movement
            player.Position = Position;
            player.Movement.SetWishDir(WishDir);
        }
    }
}