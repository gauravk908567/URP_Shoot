using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedState : PlayerBaseState, IRootGravity
{
    public GroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }
    public override void CheckSwitchState()
    {
        if (Ctx.IsJumpPressed && !Ctx.RequiredJumpPress)
        {
            SwitchState(Factory.Jump());
        }
        else if (!Ctx.CharacterController.isGrounded && !Ctx.IsJumpPressed)
        {
            SwitchState(Factory.Fall());
        }
        else if (Ctx.IsCrouchPressed && !Ctx.IsCrouchingAnimation)
        {
            SwitchState(Factory.Crouch());
        }
    }

    public override void EnterState()
    {
        InitializeSubState();
        HandleGravity();
        Debug.Log("Entered State Grounded");
    }

    public override void ExitState()
    {

    }
    public override void InitializeSubState()
    {
        if (!Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Idle());
        }
        else if (Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.walk());
        }
        else
        {
            SetSubState(Factory.Run());
        }
    }

    public override void UpdateState()
    {
        CheckSwitchState();
    }
    public void HandleGravity()
    {
        Ctx.MoveDirectionY = Ctx.Gravity;
        Ctx.AppliedMovementY = Ctx.Gravity;
    }
}
