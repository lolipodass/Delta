using UnityEngine;
using UnityEngine.InputSystem;

public class SaveState : PlayerBaseState
{
    public SaveState(PlayerSFM player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }
    private float saveTime = 0f;
    public override void Enter()
    {
        saveTime = 1f;
        player.animator.Play("Base.Save");
        Debug.Log("SaveState Enter");
    }

    public override void PhysicsUpdate()
    {
        saveTime -= Time.deltaTime;
        if (saveTime <= 0f)
        {
            player.StateMachine.ChangeState(player.idleState);
            return;
        }
    }

}