using Godot;
using Godot.Collections;

namespace Research.Game;

using Player;

public partial class PlayerManager : Node
{
    [Export] public Map CurrentMap;
    [Export] public PackedScene PlayerPrefab;
    [Export] public Node Players;
    private Array<Player> _spawnedPlayers = new Array<Player>();
    private int _spCounter = 0;
    public Player SpawnNewPlayer()
    {
        Player player = (Player)PlayerPrefab.Instantiate();
        var spawnPoint = CurrentMap.Spawnpoints[_spCounter];
        _spCounter++;
        if (_spCounter == CurrentMap.Spawnpoints.Count)
        {
            _spCounter = 0;
        }

        player.Color.Color = new Color(GD.Randf(), GD.Randf(), GD.Randf(), 1f);

        player.Position = spawnPoint.Position;
        Players.AddChild(player);
        
        return player;
    }
}