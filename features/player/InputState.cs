using Godot;

namespace Research.Player;

public class InputState
{
    public uint Sequence { get; }
    public Vector2 Input { get; }
    public float DeltaTime { get; }

    public InputState(uint sequence, Vector2 input, float deltaTime)
    {
        Sequence = sequence;
        Input = input;
        DeltaTime = deltaTime;
    }
}
