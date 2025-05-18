using UnityEngine;

public abstract class PlayerBaseState
{
    protected PlayerStateMachine stateMachine;
    protected PlayerStats Stats { get; private set; }
    protected PlayerSFM player;
    protected PlayerBaseState(PlayerSFM player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        Stats = player.PlayerStats;
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
        XVelocity = player.rb.linearVelocity.x;
        YVelocity = player.rb.linearVelocity.y;

        if (CanMoveHorizontal)
        {
            float targetXVelocity = player.ButtonMoveInput * Stats.Stats.MaxSpeed * HorizontalSpeedMultiplayer;
            XVelocity = Mathf.Lerp(XVelocity, targetXVelocity, 10f * Time.fixedDeltaTime);
        }
        if (CanMoveVertical)
            YVelocity = Mathf.Max(YVelocity, -Stats.Stats.MaxFallSpeed * VerticalSpeedMultiplayer);
        else
            YVelocity = 0;

        player.rb.linearVelocity = new Vector2(XVelocity, YVelocity);

    }

    protected void PlayAnimation()
    {
        player.animator.Play("Base." + AnimationName);
    }
    protected virtual string AnimationName => "idle";
    protected float YVelocity;
    protected float XVelocity;

    protected virtual bool CanMoveHorizontal => true;
    protected virtual bool CanMoveVertical => true;

    public virtual float HorizontalSpeedMultiplayer => 1f;
    public virtual float VerticalSpeedMultiplayer => 1f;
    public virtual bool CanRotate => true;
    public virtual bool CanHurt => true;
}
