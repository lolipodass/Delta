using UnityEngine;

public class UpToFallState : PlayerBaseState
{
    public UpToFallState(PlayerSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    protected override string AnimationName => "uptofall";
    private float timer = 0f;
    public override void Enter()
    {
        PlayAnimation();
        timer = 0.20f;
    }
    public override void LogicUpdate()
    {
        if (player.IsGrounded)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }
        if (YVelocity < 0.1f)
            PlayAnimation();
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            return;
        }
        stateMachine.ChangeState(player.fallState);
    }
}