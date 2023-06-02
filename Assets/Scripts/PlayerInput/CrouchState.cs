using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchState : PlayerBaseState
{
    public CrouchState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }
    public override void CheckSwitchState()
    {
        if (Ctx.IsCrouchPressed && Ctx.IsCrouchingAnimation && !Ctx.IsCrouching)
        {
            SwitchState(Factory.Grounded());
        }
        else if (!Ctx.CharacterController.isGrounded && !Ctx.IsJumpPressed)
        {
            SwitchState(Factory.Fall());
        }
        else if (Ctx.IsJumpPressed)
        {
            SwitchState(Factory.Grounded());
        }
    }

    public override void EnterState()
    {
        InitializeSubState();
        Debug.Log("Entered Crouch State");
        Ctx.Animator.SetBool(Ctx.IsCrouchingHash, true);
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
        else if (Ctx.IsMovementPressed && Ctx.IsRunPressed)
        {
            SwitchState(Factory.Run());
        }
        else if (Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SwitchState(Factory.walk());
        }
    }

    public override void UpdateState()
    {
        if (Ctx.shouldCrouch)
            HandleCrouch();

        CheckSwitchState();
    }

    private void HandleCrouch()
    {
        Ctx.StartCoroutine(CrouchStand());
    }
    private IEnumerator CrouchStand()
    {
        if (Ctx.IsCrouching && Physics.Raycast(Ctx.PlayerCamera.transform.position, Vector3.up, 1f))
            yield break;

        Ctx.IsCrouchingAnimation = true;

        float TimeElapsed = 0;
        float targetHeight = Ctx.IsCrouching ? Ctx.StandHeight : Ctx.CrouchHeight;
        float Currentheight = Ctx.CharacterController.height;
        Vector3 TargetCenter = Ctx.IsCrouching ? Ctx.StandCentre : Ctx.CrouchCentre;
        Vector3 CurrentCenter = Ctx.CharacterController.center;

        while (TimeElapsed < Ctx.TimeToCrouch)
        {
            Ctx.CharacterController.height = Mathf.Lerp(Currentheight, targetHeight, TimeElapsed / Ctx.TimeToCrouch);
            Ctx.CharacterController.center = Vector3.Lerp(CurrentCenter, TargetCenter, TimeElapsed / Ctx.TimeToCrouch);
            TimeElapsed += Time.deltaTime;
            yield return null;
        }

        Ctx.CharacterController.height = targetHeight;
        Ctx.CharacterController.center = TargetCenter;

        Ctx.IsCrouching = !Ctx.IsCrouching;

        Ctx.IsCrouchingAnimation = false;

        Ctx.Animator.SetBool(Ctx.IsCrouchingHash, false);
    }


}
