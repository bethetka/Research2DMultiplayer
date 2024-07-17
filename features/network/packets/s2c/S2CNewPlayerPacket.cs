using Godot;
using Godot.Collections;
using Research.Console;
using Research.Player;

namespace Research.Network.Packets.S2C;

public class S2CNewPlayerPacket : IS2CPacket
{
    public PlayerInfo Info;

    public S2CNewPlayerPacket() {}
    public S2CNewPlayerPacket(PlayerInfo info)
    {
        Info = info;
    }
    
    public int GetId()
    {
        return Packets.NewPlayer;
    }

    public string GetFriendlyName()
    {
        return GetType().Name;
    }

    public void FromVariant(Variant variant)
    {
        var dictionary = variant.AsGodotDictionary<string, Variant>();
        Info = PlayerInfo.FromVariant(dictionary[nameof(Info)]);
    }

    public Variant ToVariant()
    {
        return new Dictionary<string, Variant>
        {
            {nameof(Info), Info.ToVariant()}
        };
    }

    public void Handle(Client client)
    {
        var player = client.PlayerManager.SpawnNewPlayer();
        Info.ApplyToPlayer(player);
        client.Players.Add(player);
    }
}