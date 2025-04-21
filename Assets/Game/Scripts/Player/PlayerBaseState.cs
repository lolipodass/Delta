using UnityEngine;

public abstract class PlayerBaseState
{
    protected PlayerStateMachine stateMachine;
    protected PlayerMovementSFM player;
    protected PlayerBaseState(PlayerMovementSFM player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }
    virtual public void Enter() { }
    virtual public void LogicUpdate() { }
    virtual public void PhysicsUpdate() { }
    virtual public void Exit() { }

    public virtual bool CanMoveHorizontal => true;
    public virtual bool CanMoveVertical => true;

    public virtual float HorizontalSpeedMultiplayer => 1f;
    public virtual float VerticalSpeedMultiplayer => 1f;
}
