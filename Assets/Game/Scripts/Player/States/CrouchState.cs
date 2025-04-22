using UnityEngine;

public class CrouchState : PlayerBaseState
{
    public CrouchState(PlayerMovementSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        player.ToggleCrouch(true);
    }
    public override void LogicUpdate()
    {
        if (!player.IsHoldCrouchButton && player.CanStandUp())
        {
            player.ToggleCrouch(false);
            stateMachine.ChangeState(player.idleState);
            return;
        }
    }

    public override float HorizontalSpeedMultiplayer => 0.5f;
}