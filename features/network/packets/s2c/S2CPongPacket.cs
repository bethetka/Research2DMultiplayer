using System;
using Godot;
using Godot.Collections;

namespace Research.Network.Packets.C2S;

public class S2CPongPacket : IS2CPacket
{
    public long Time;
    public long ServerTime;

    public S2CPongPacket()
    {
    }

    public S2CPongPacket(long time, long serverTime)
    {
        Time = time;
        ServerTime = serverTime;
    }
    
    public int GetId()
    {
        return Packets.Pong;
    }

    public string GetFriendlyName()
    {
        return GetType().Name;
    }

    public void FromVariant(Variant variant)
    {
        var dictionary = variant.AsGodotDictionary<string, Variant>();
        Time = dictionary[nameof(Time)].AsInt64();
        ServerTime = dictionary[nameof(ServerTime)].AsInt64();
    }

    public Variant ToVariant()
    {
        return new Dictionary<string, Variant>
        {
            {nameof(Time), Time},
            {nameof(ServerTime), ServerTime}
        };
    }

    public void Handle(Client client)
    {
        client.ServerPing = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - Time;
    }
    
}