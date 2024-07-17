using System.Linq;
using Godot;
using Godot.Collections;
using Research.Console;
using Research.Player;

namespace Research.Network.Packets.S2C;

public class S2CHelloPacket : IS2CPacket
{
    public PlayerInfo Self;
    public Array<PlayerInfo> Other;

    public S2CHelloPacket()
    {
    }

    public S2CHelloPacket(PlayerInfo self, Array<PlayerInfo> other)
    {
        Self = self;
        Other = other;
    }

    public int GetId()
    {
        return Packets.Hello;
    }

    public string GetFriendlyName()
    {
        return GetType().Name;
    }

    public void FromVariant(Variant variant)
    {
        var dictionary = variant.AsGodotDictionary<string, Variant>();
        Self = PlayerInfo.FromVariant(dictionary[nameof(Self)]);
        Other = new Array<PlayerInfo>(dictionary[nameof(Other)].AsGodotArray<Variant>().Select(i => PlayerInfo.FromVariant(i)));
    }

    public Variant ToVariant()
    {
        return new Dictionary<string, Variant>
        {
            {nameof(Self), Self.ToVariant()},
            {nameof(Other), new Array<Variant>(Other.Select(i => i.ToVariant()))}
        };
    }

    public void Handle(Client client)
    {
        var p = client.PlayerManager.SpawnNewPlayer();
        Self.ApplyToPlayer(p);
        client.LocalPlayer = p;
        client.Players.Add(p);

        foreach (var info in Other)
        {
            var other = client.PlayerManager.SpawnNewPlayer();
            info.ApplyToPlayer(other);
            client.Players.Add(other);
        }

        client.ClientStatus = Client.Status.Playing;
        ResearchConsole.Instance.Info($"spawned on server, assigned id {Self.Id}");
    }
}