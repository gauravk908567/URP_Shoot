using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : PlayerBaseState
{
    public JumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }
    public override void CheckSwitchState()
    {
        if (Ctx.CharacterController.isGrounded)
        {
            SwitchState(Factory.Grounded());
        }
    }

    public override void EnterState()
    {
        InitializeSubState();
        Debug.Log("Entered State Jump");
        HandleJump();
    }

    public override void ExitState()
    {
        Ctx.Animator.SetBool(Ctx.IsJumpingHash, false);
        if (Ctx.IsJumpPressed)
        {
            Ctx.RequiredJumpPress = true;
        }
        Ctx.IsJumping = false;
        Debug.Log("Exit Jump State");
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
        HandleGravity();
        CheckSwitchState();
    }

    void HandleJump()
    {
        Ctx.Animator.SetBool(Ctx.IsJumpingHash, true);
        Ctx.IsJumping = true;
        Ctx.MoveDirectionY = Ctx.JumpVelocity;
        Ctx.AppliedMovementY = Ctx.JumpVelocity;
    }
    public void HandleGravity()
    {
        bool isFalling = Ctx.MoveDirectionY <= 0.0f || !Ctx.IsJumpPressed;
        float fallMultiplier = 2.0f;

        if (isFalling)
        {
            float previousYVelocity = Ctx.MoveDirectionY;
            Ctx.MoveDirectionY = Ctx.MoveDirectionY + Ctx.JumpGravity * fallMultiplier * Time.deltaTime;
            Ctx.AppliedMovementY = Mathf.Max((previousYVelocity + Ctx.MoveDirectionY) * 0.5f, -20f);

        }
        else
        {
            float previousYVelocity = Ctx.MoveDirectionY;
            Ctx.MoveDirectionY = Ctx.MoveDirectionY + Ctx.JumpGravity * Time.deltaTime;
            Ctx.AppliedMovementY = (previousYVelocity + Ctx.MoveDirectionY) * 0.5f;

            Debug.Log("Gravity is being applied" + Ctx.Gravity);
        }
    }
}
