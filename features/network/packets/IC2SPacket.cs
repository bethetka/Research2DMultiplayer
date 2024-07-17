namespace Research.Network.Packets;

using Research.Player;

public interface IC2SPacket : IPacket
{
    public void Handle(Server server, ServerPlayer sender);

}