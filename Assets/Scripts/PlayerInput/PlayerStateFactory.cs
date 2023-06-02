using System.Collections.Generic;

public class PlayerStateFactory
{
    enum EPlayerStates { grounded, idle, walk, run, jump, fall, crouch };

    PlayerStateMachine _context;
    Dictionary<EPlayerStates, PlayerBaseState> _states = new Dictionary<EPlayerStates, PlayerBaseState>();

    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        _context = currentContext;
        _states[EPlayerStates.grounded] = new GroundedState(_context, this);
        _states[EPlayerStates.idle] = new IdleState(_context, this);
        _states[EPlayerStates.walk] = new WalkState(_context, this);
        _states[EPlayerStates.run] = new RunState(_context, this);
        _states[EPlayerStates.jump] = new JumpState(_context, this);
        _states[EPlayerStates.fall] = new FallState(_context, this);
        _states[EPlayerStates.crouch] = new CrouchState(_context, this);
    }

    public PlayerBaseState Grounded()
    {
        return _states[EPlayerStates.grounded];
    }
    public PlayerBaseState Idle()
    {
        return _states[EPlayerStates.idle];
    }
    public PlayerBaseState walk()
    {
        return _states[EPlayerStates.walk];
    }
    public PlayerBaseState Run()
    {
        return _states[EPlayerStates.run];
    }
    public PlayerBaseState Jump()
    {
        return _states[EPlayerStates.jump];
    }
    public PlayerBaseState Fall()
    {
        return _states[EPlayerStates.fall];
    }
    public PlayerBaseState Crouch()
    {
        return _states[EPlayerStates.crouch];
    }
}
