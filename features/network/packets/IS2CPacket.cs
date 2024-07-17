namespace Research.Network.Packets;

public interface IS2CPacket : IPacket
{
    public void Handle(Client client);
}