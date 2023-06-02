public abstract class PlayerBaseState
{
    public PlayerStateMachine Ctx { get => _ctx; set => _ctx = value; }
    public PlayerStateFactory Factory { get => _factory; set => _factory = value; }
    public bool IsRootState { get => isRootState; set => isRootState = value; }

    private bool isRootState = false;

    private PlayerStateMachine _ctx;
    private PlayerStateFactory _factory;

    protected PlayerBaseState _currentSubState;
    protected PlayerBaseState _currentSuperState;

    protected PlayerBaseState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    {
        Ctx = currentContext;
        Factory = playerStateFactory;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void CheckSwitchState();
    public abstract void InitializeSubState();
    public abstract void ExitState();

    public void UpdateStates()
    {
        UpdateState();

        if (_currentSubState != null)
        {
            _currentSubState.UpdateStates();
        }
    }

    protected void SwitchState(PlayerBaseState newState)
    {
        ExitState();

        newState.EnterState();

        if (IsRootState)
        {
            Ctx.CurrentState = newState;
        }
        else if (_currentSuperState != null)
        {
            //set the current super states sub state to new state
            _currentSuperState.SetSubState(newState);
        }
    }

    protected void SetSuperState(PlayerBaseState newSuperState)
    {
        _currentSuperState = newSuperState;
    }

    protected void SetSubState(PlayerBaseState newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
}