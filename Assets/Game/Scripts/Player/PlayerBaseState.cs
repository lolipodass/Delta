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
    virtual public void PhysicsUpdate()
    {
        Movement();
    }
    virtual public void Exit() { }

    virtual public void Movement()
    {

        //     if (!StateMachine.CurrentState.CanMoveHorizontal)
        //     {
        //         return;//?? if only vertical movement
        //     }

        XVelocity = player.rb.linearVelocity.x;
        YVelocity = player.rb.linearVelocity.y;
        float targetXVelocity = player.ButtonMoveInput * player.maxSpeed * HorizontalSpeedMultiplayer;
        XVelocity = Mathf.Lerp(XVelocity, targetXVelocity, 10f * Time.fixedDeltaTime);
        YVelocity = Mathf.Max(YVelocity, -player.MaxFallSpeed * VerticalSpeedMultiplayer);
        player.rb.linearVelocity = new Vector2(XVelocity, YVelocity);

    }

    protected float YVelocity;
    protected float XVelocity;

    protected virtual bool CanMoveHorizontal => true;
    protected virtual bool CanMoveVertical => true;

    public virtual float HorizontalSpeedMultiplayer => 1f;
    public virtual float VerticalSpeedMultiplayer => 1f;
}
