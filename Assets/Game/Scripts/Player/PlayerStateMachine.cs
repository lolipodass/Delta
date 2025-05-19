using UnityEngine;

public class PlayerStateMachine
{
    public PlayerStateMachine(PlayerSFM playerMovementSFM) => this.playerMovementSFM = playerMovementSFM;
    public PlayerSFM playerMovementSFM;
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
        CurrentState.Init();
        CurrentState.Enter();
    }
}

