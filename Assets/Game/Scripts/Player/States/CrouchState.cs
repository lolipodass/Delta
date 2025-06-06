using UnityEngine;

public class CrouchState : PlayerBaseState
{
    public CrouchState(PlayerSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        player.ToggleCrouch(true);
        player.AnimationCrouch = true;
    }
    public override void LogicUpdate()
    {
        if (!player.ButtonCrouch && player.CanStandUp())
        {
            player.ToggleCrouch(false);
            stateMachine.ChangeState(player.idleState);
            return;
        }
    }
    public override void Exit()
    {
        player.AnimationCrouch = false;
    }

    public override float HorizontalSpeedMultiplayer => 0.5f;
}