using Godot;

namespace Research.Network.Packets;

public interface IPacket
{
    public int GetId();
    public string GetFriendlyName();
    public void FromVariant(Variant variant);
    public Variant ToVariant();
}