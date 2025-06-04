using UnityEngine;

public class UpToFallState : PlayerBaseState
{
    public UpToFallState(PlayerSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    protected override string AnimationName => "uptofall";
    private bool isHoldOnEnterJumpButton = false;
    private float timer = 0f;
    public override void Enter()
    {
        PlayAnimation();
        timer = 0.20f;
        isHoldOnEnterJumpButton = player.ButtonJump;
    }
    public override void LogicUpdate()
    {
        if (player.IsGrounded)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }
        if (player.ButtonDash && Stats.Stats.HasDash && player.CanDash)
        {
            stateMachine.ChangeState(player.dashState);
            return;
        }
        if (player.ButtonJump && CanJump())
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }
        isHoldOnEnterJumpButton = player.ButtonJump;

        if (YVelocity < 0.1f)
            PlayAnimation();
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            return;
        }
        stateMachine.ChangeState(player.fallState);
    }
    private bool CanJump()
    {
        return !isHoldOnEnterJumpButton &&
        ((player.TimeLastWallTouch > 0 && Stats.Stats.HasWallJump) || player.ExtraJumpCountLeft > 0)
        || player.TimeLastGrounded > 0;
    }
    public override float HorizontalSpeedMultiplayer => Stats.Stats.AirControlFactor;
}