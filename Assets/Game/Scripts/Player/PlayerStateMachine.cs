using UnityEngine;

public class PlayerStateMachine
{
    public PlayerStateMachine(PlayerMovementSFM playerMovementSFM) => this.playerMovementSFM = playerMovementSFM;
    public PlayerMovementSFM playerMovementSFM;
    public PlayerBaseState CurrentState { get; set; }
    public void InitializeState(PlayerBaseState state)
    {
        CurrentState = state;
        CurrentState.Enter();
    }

    public void ChangeState(PlayerBaseState newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}

