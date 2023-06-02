using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : PlayerBaseState
{
    public WalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {

    }
    public override void CheckSwitchState()
    {
        if (!Ctx.IsMovementPressed)
        {
            SwitchState(Factory.Idle());
        }
        else if (Ctx.IsMovementPressed && Ctx.IsRunPressed)
        {
            SwitchState(Factory.Run());
        }
    }

    public override void EnterState()
    {
        Debug.Log("Entered State Walk");

        Ctx.Animator.SetBool(Ctx.IsWalkingHash, true);
        Ctx.Animator.SetBool(Ctx.IsRunningHash, false);
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
        ;
    }

    public override void UpdateState()
    {
        HandleWalk();
        CheckSwitchState();
    }

    void HandleWalk()
    {
        float speed = Ctx.IsCrouching ? Ctx.CrouchSpeed : Ctx.WalkSpeed;
        float speedstrafe = Ctx.IsCrouching ? Ctx.CrouchStrafeSpeed : Ctx.StrafeSpeed;

        var currentinputVertical = Ctx.CurrentMovementInput.y * speed;
        var currentinputHorizontal = Ctx.CurrentMovementInput.x * speedstrafe;

        float moveDirectionY = Ctx.MoveDirectionY;
        Ctx.MoveDirection = Ctx.transform.TransformDirection(Vector3.right) * currentinputHorizontal + Ctx.transform.TransformDirection(Vector3.forward) * currentinputVertical;
        Ctx.MoveDirectionY = moveDirectionY;

        Ctx.AppliedMovementX = Ctx.MoveDirectionX;
        Ctx.AppliedMovementZ = Ctx.MoveDirectionZ;

        // Calculate velocityX and velocityZ
        float velocityX = currentinputHorizontal / speedstrafe;
        float velocityZ = currentinputVertical / speed; ;

        float currentVelocityX = Ctx.Animator.GetFloat(Ctx.VelocityXHash);
        float currentVelocityZ = Ctx.Animator.GetFloat(Ctx.VelocityZHash);

        float smoothedVelocityX;
        float smoothedVelocityZ;

        if (currentinputVertical == 0 && currentinputHorizontal == 0)
        {
            float minSmoothedVelocityX = 0f;
            float maxSmoothedVelocityX = 0f;
            float minSmoothedVelocityZ = 0f;
            float maxSmoothedVelocityZ = 0f;

            smoothedVelocityX = Mathf.SmoothDamp(currentVelocityX, 0f, ref currentVelocityX, Ctx.SmoothTime);
            smoothedVelocityZ = Mathf.SmoothDamp(currentVelocityZ, 0f, ref currentVelocityZ, Ctx.SmoothTime);

            smoothedVelocityX = Mathf.Clamp(smoothedVelocityX, minSmoothedVelocityX, maxSmoothedVelocityX);
            smoothedVelocityZ = Mathf.Clamp(smoothedVelocityZ, minSmoothedVelocityZ, maxSmoothedVelocityZ);
        }
        else
        {
            smoothedVelocityX = Mathf.SmoothDamp(currentVelocityX, velocityX, ref currentVelocityX, Ctx.SmoothTime);
            smoothedVelocityZ = Mathf.SmoothDamp(currentVelocityZ, velocityZ, ref currentVelocityZ, Ctx.SmoothTime);
        }

        // Set the animator parameters

        smoothedVelocityX = Mathf.Clamp(smoothedVelocityX, -0.5f, 0.5f);
        smoothedVelocityZ = Mathf.Clamp(smoothedVelocityZ, -0.5f, 0.5f);

        // Set the animator parameters
        Ctx.Animator.SetFloat(Ctx.VelocityXHash, smoothedVelocityX);
        Ctx.Animator.SetFloat(Ctx.VelocityZHash, smoothedVelocityZ);
    }
}
