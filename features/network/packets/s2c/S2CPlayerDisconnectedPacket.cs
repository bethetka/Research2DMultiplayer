using System.Linq;
using Godot;
using Godot.Collections;
using Research.Console;

namespace Research.Network.Packets.S2C;

public class S2CPlayerDisconnectedPacket : IS2CPacket 
{
    public int Id;
    public string Reason;
    public S2CPlayerDisconnectedPacket() {}

    public S2CPlayerDisconnectedPacket(int id, string reason)
    {
        Id = id;
        Reason = reason;
    }
    
    public int GetId()
    {
        return Packets.PlayerDisconnected;
    }

    public string GetFriendlyName()
    {
        return GetType().Name;
    }

    public void FromVariant(Variant variant)
    {
        var dictionary = variant.AsGodotDictionary<string, Variant>();
        Id = dictionary[nameof(Id)].AsInt32();
        Reason = dictionary[nameof(Reason)].AsString();
    }

    public Variant ToVariant()
    {
        return new Dictionary<string, Variant>()
        {
            {nameof(Id), Id},
            {nameof(Reason), Reason}
        };
    }

    public void Handle(Client client)
    {
        var player = client.Players.FirstOrDefault(i => i.Id == Id);
        if (player == null)
        {
            ResearchConsole.Instance.Warn($"Failed to dispose player by id {Id}, because it don't exist");
        }
        else
        {
            ResearchConsole.Instance.Info($"Player {Id} disconnected, reason: {Reason}");
            if (client.LocalPlayer.Id == Id)
            {
                client.Peer.DisconnectFromHost();
                client.ClientStatus = Client.Status.None;
            }
            player.QueueFree();
        }
    }
}