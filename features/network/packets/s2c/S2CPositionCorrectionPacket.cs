using System.Linq;
using Godot;
using Godot.Collections;
using Research.Console;

namespace Research.Network.Packets.S2C;

public class S2CPositionCorrectionPacket : IS2CPacket
{
    public int Id { get; set; }
    public Vector2 CorrectedPosition { get; set; }
    public uint LastProcessedInput { get; set; }

    public S2CPositionCorrectionPacket() {}
    public S2CPositionCorrectionPacket(int id, Vector2 correctedPosition, uint lastProcessedInput)
    {
        Id = id;
        CorrectedPosition = correctedPosition;
        LastProcessedInput = lastProcessedInput;
    }
    
    public int GetId()
    {
        return Packets.PositionCorrection; // You'll need to add this to your Packets enum
    }
    
    public string GetFriendlyName()
    {
        return GetType().Name;
    }

    public void FromVariant(Variant variant)
    {
        var dictionary = variant.AsGodotDictionary<string, Variant>();
        Id = dictionary[nameof(Id)].AsInt32();
        CorrectedPosition = dictionary[nameof(CorrectedPosition)].AsVector2();
        LastProcessedInput = (uint)dictionary[nameof(LastProcessedInput)].AsInt64();
    }

    public Variant ToVariant()
    {
        return new Dictionary<string, Variant>
        {
            {nameof(Id), Id},
            {nameof(CorrectedPosition), CorrectedPosition},
            {nameof(LastProcessedInput), LastProcessedInput}
        };
    }

    public void Handle(Client client)
    {
        var player = client.Players.FirstOrDefault(i => i.Id == Id);
        if (player == null)
        {
            ResearchConsole.Instance.Warn($"Failed to process PositionCorrection packet for player {Id}");
            return;
        }
        
        if (player == client.LocalPlayer)
        {
            // This is a correction for the local player, perform reconciliation
            player.Movement.ReconcileState(CorrectedPosition, LastProcessedInput);
            ResearchConsole.Instance.Info($"Position corrected for local player. New position: {CorrectedPosition}");
        }
        else
        {
            // This shouldn't happen, as position corrections are only sent to the affected player
            ResearchConsole.Instance.Warn($"Received PositionCorrection packet for non-local player {Id}");
        }
    }
}