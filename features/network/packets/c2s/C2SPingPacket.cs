using System;
using Godot;
using Godot.Collections;

namespace Research.Network.Packets.C2S;

public class C2SPingPacket : IC2SPacket
{
    public long Time;

    public C2SPingPacket()
    {
    }

    public C2SPingPacket(long time)
    {
        Time = time;
    }

    public int GetId()
    {
        return Packets.Ping;
    }

    public string GetFriendlyName()
    {
        return GetType().Name;
    }

    public void FromVariant(Variant variant)
    {
        var dictionary = variant.AsGodotDictionary<string, Variant>();
        Time = dictionary[nameof(Time)].AsInt64();
    }

    public Variant ToVariant()
    {
        return new Dictionary<string, Variant>
        {
            { nameof(Time), Time }
        };
    }

    public void Handle(Server server, ServerPlayer sender)
    {
        var packet = new S2CPongPacket(Time, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        Packets.SendPacket(packet, sender);
    }
}