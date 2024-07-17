using Godot;

namespace Research.Player;

public partial class Player : CharacterBody2D
{
    [Export] public Movement Movement;
    [Export] public ColorRect Color;
    public int Id;
    public string Username;
}