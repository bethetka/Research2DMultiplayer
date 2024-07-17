using Godot;
using Research.Network;
using Research.Network.Packets;
using Research.Network.Packets.C2S;
using System.Collections.Generic;

namespace Research.Player;

public partial class Movement : PlayerComponent
{
    [Export] public float Speed { get; set; } = 250.0f;
    private Vector2 _currentWishDir;
    private Vector2 _lastInput;
    private uint _lastProcessedInput = 0;
    private Queue<InputState> _pendingInputs = new Queue<InputState>();

    public Vector2 GetWishDir()
    {
        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
        return inputDir;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        
        if (IsControlled())
        {
            var wishDir = GetWishDir();
            if (_lastInput != wishDir)
            {
                _lastProcessedInput++;
                var inputState = new InputState(_lastProcessedInput, wishDir, (float)delta);
                _pendingInputs.Enqueue(inputState);
                
                var packet = new C2SMovePacket(_lastProcessedInput, wishDir);
                Packets.SendPacket(packet, Client.Instance);
                _lastInput = wishDir;
            }

            // Apply prediction
            ApplyInput(wishDir, (float)delta);
        }
        else
        {
            // For non-controlled players, just apply the current wish direction
            Parent.Position += _currentWishDir.Normalized() * Speed * (float)delta;
        }
    }

    private void ApplyInput(Vector2 input, float deltaTime)
    {
        Parent.Position += input.Normalized() * Speed * deltaTime;
    }

    public void SetWishDir(Vector2 wishDir)
    {
        _currentWishDir = wishDir;
    }

    public void ReconcileState(Vector2 serverPosition, uint lastProcessedInput)
    {
        Parent.Position = serverPosition;
        
        // Remove older inputs
        while (_pendingInputs.Count > 0 && _pendingInputs.Peek().Sequence <= lastProcessedInput)
        {
            _pendingInputs.Dequeue();
        }

        // Re-apply pending inputs
        foreach (var input in _pendingInputs)
        {
            ApplyInput(input.Input, input.DeltaTime);
        }
    }
}
