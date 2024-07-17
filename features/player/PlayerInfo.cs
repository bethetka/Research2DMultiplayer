using Godot;
using Godot.Collections;

namespace Research.Player;

public partial class PlayerInfo : GodotObject
{
    [Export] public int Id;
    [Export] public string Username;
    [Export] public Vector2 Position;
    [Export] public Color Color;

    private PlayerInfo(int id, string username, Vector2 position, Color color)
    {
        Id = id;
        Username = username;
        Position = position;
        Color = color;
    }

    public PlayerInfo()
    {
    }

    public static PlayerInfo FromPlayer(Player player)
    {
        return new PlayerInfo(player.Id, player.Username, player.Position, player.Color.Color);
    }

    public void ApplyToPlayer(Player player)
    {
        player.Id = Id;
        player.Username = Username;
        player.Position = Position;
        player.Color.Color = Color;
    }
    
    public static PlayerInfo FromVariant(Variant variant)
    {
        var dictionary = variant.AsGodotDictionary<string, Variant>();

        return new PlayerInfo(
            dictionary[nameof(Id)].AsInt32(),
            dictionary[nameof(Username)].AsString(),
            dictionary[nameof(Position)].AsVector2(),
            dictionary[nameof(Color)].AsColor()
        );
    }

    public Variant ToVariant()
    {
        return new Dictionary<string, Variant>
        {
            {nameof(Id), Id},
            {nameof(Username), Username},
            {nameof(Position), Position},
            {nameof(Color), Color}
        };
    }
}