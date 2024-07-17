using System.Linq;
using Godot;
using Godot.Collections;
using Research.Console;
using Research.Network.Packets.S2C;
using Research.Player;

namespace Research.Network.Packets.C2S;

public class C2SAuthenticatePacket : IC2SPacket
{
    [Export] public string Username;

    public C2SAuthenticatePacket()
    {
    }

    public C2SAuthenticatePacket(string username)
    {
        Username = username;
    }

    public int GetId()
    {
        return Packets.Authenticate;
    }

    public string GetFriendlyName()
    {
        return GetType().Name;
    }

    public void FromVariant(Variant variant)
    {
        var dictionary = variant.AsGodotDictionary<string, Variant>();
        Username = dictionary[nameof(Username)].AsString();
    }

    public Variant ToVariant()
    {
        return new Dictionary<string, Variant>
        {
            {nameof(Username), Username}
        };
    }

    public void Handle(Server server, ServerPlayer sender)
    {
        server.IdCounter++;
        var player = server.PlayerManager.SpawnNewPlayer();
        player.Id = server.IdCounter;
        player.Username = Username;
        sender.Player = player;
        sender.Status = ServerPlayer.ServerPlayerStatus.Playing;

        var others = server.Players
            .Where(i => i.Status == ServerPlayer.ServerPlayerStatus.Playing)
            .Where(i => i.Player.Id != player.Id)
            .ToList();

        var fromPlayer = PlayerInfo.FromPlayer(player);
        var helloPacket = new S2CHelloPacket(
            fromPlayer,
            new Array<PlayerInfo>(others.Select(i => PlayerInfo.FromPlayer(i.Player)))
        );
        Packets.SendPacket(helloPacket, sender);

        var newPlayerPacket = new S2CNewPlayerPacket(fromPlayer);
        others.ForEach(i => Packets.SendPacket(newPlayerPacket, i));
    }
}