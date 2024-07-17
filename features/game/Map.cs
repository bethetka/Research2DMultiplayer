using Godot;
using Godot.Collections;

namespace Research.Game;

public partial class Map : Node
{
    [Export] public Array<Marker2D> Spawnpoints = new Array<Marker2D>();
    
}