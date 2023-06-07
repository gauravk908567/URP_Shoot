using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Unity.Netcode;

public class PlayerStateMachine : NetworkBehaviour
{
    #region ControlSettingProperties
    public PlayerBaseState CurrentState { get => _currentState; set => _currentState = value; }
    public Animator Animator { get => _animator; set => _animator = value; }
    public CharacterController CharacterController { get => _characterController; set => _characterController = value; }
    public Camera PlayerCamera { get => _playerCamera; set => _playerCamera = value; }

    //Gravity Properties
    public float Gravity { get => _gravity; set => _gravity = value; }

    //Hash Ints Properties
    public int IsWalkingHash { get => _isWalkingHash; set => _isWalkingHash = value; }
    public int IsRunningHash { get => _isRunningHash; set => _isRunningHash = value; }
    public int IsJumpingHash { get => _isJumpingHash; set => _isJumpingHash = value; }
    public int IsFallingHash { get => _isFallingHash; set => _isFallingHash = value; }
    public int IsCrouchingHash { get => _isCrouchingHash; set => _isCrouchingHash = value; }
    public int VelocityXHash { get => _velocityXHash; set => _velocityXHash = value; }
    public int VelocityZHash { get => _velocityZHash; set => _velocityZHash = value; }

    //State Check Properties
    public bool IsMovementPressed { get => _isMovementPressed; set => _isMovementPressed = value; }
    public bool IsRunPressed { get => _isRunPressed; set => _isRunPressed = value; }
    public bool IsJumpPressed { get { return _isJumpPressed; } }
    public bool IsJumping { get => _isJumping; set => _isJumping = value; }
    public bool IsFalling { get => _isFalling; set => _isFalling = value; }
    public bool IsCrouchPressed { get => _isCrouchPressed; set => _isCrouchPressed = value; }

    //Movement Input Properties
    public Vector2 CurrentMovementInput { get => _currentMovementInput; set => _currentMovementInput = value; }
    public Vector3 MoveDirection { get => _moveDirection; set => _moveDirection = value; }
    public Vector3 AppliedMovement { get => _appliedMovement; set => _appliedMovement = value; }
    public float MoveDirectionY { get => _moveDirection.y; set => _moveDirection.y = value; }
    public float MoveDirectionX { get => _moveDirection.x; set => _moveDirection.x = value; }
    public float MoveDirectionZ { get => _moveDirection.z; set => _moveDirection.z = value; }
    public float AppliedMovementY { get => _appliedMovement.y; set => _appliedMovement.y = value; }
    public float AppliedMovementX { get => _appliedMovement.x; set => _appliedMovement.x = value; }
    public float AppliedMovementZ { get => _appliedMovement.z; set => _appliedMovement.z = value; }

    //Jump Properties
    public bool RequiredJumpPress { get => _requiredJumpPress; set => _requiredJumpPress = value; }
    public float JumpGravity { get => _jumpGravity; set => _jumpGravity = value; }
    public float JumpVelocity { get => _jumpVelocity; set => _jumpVelocity = value; }

    //Movement Speed Input Variables Properties
    public float WalkSpeed { get => _walkSpeed; set => _walkSpeed = value; }
    public float SprintSpeed { get => _sprintSpeed; set => _sprintSpeed = value; }
    public float StrafeSpeed { get => _strafeSpeed; set => _strafeSpeed = value; }
    public float CrouchSpeed { get => _crouchSpeed; set => _crouchSpeed = value; }
    public float CrouchStrafeSpeed { get => _crouchStrafeSpeed; set => _crouchStrafeSpeed = value; }
    public float SlopeSlideSpeed { get => _slopeSlideSpeed; set => _slopeSlideSpeed = value; }

    //crouch Parameters
    public float CrouchHeight { get => _crouchHeight; set => _crouchHeight = value; }
    public float StandHeight { get => _standHeight; set => _standHeight = value; }
    public float TimeToCrouch { get => _timeToCrouch; set => _timeToCrouch = value; }
    public Vector3 CrouchCentre { get => _crouchCentre; set => _crouchCentre = value; }
    public Vector3 StandCentre { get => _standCentre; set => _standCentre = value; }
    public bool IsCrouching { get => _isCrouching; set => _isCrouching = value; }
    public bool IsCrouchingAnimation { get => _isCrouchingAnimation; set => _isCrouchingAnimation = value; }
    public bool shouldCrouch => _isCrouchPressed && !_isCrouchingAnimation && _characterController.isGrounded;
    public float SmoothTime { get => _smoothTime; set => _smoothTime = value; }

    //Sliding parameters
    public Vector3 HitPointNormal { get => _hitPointNormal; set => _hitPointNormal = value; }
    public bool isSliding
    {
        get
        {
            Debug.DrawRay(transform.position, Vector3.down, Color.red); //Was used to check lenght of player from current transform position to ground
            if (_characterController.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit SlopeHit, 1.5f))
            {
                _hitPointNormal = SlopeHit.normal;
                return Vector3.Angle(_hitPointNormal, Vector3.up) > _characterController.slopeLimit;
            }
            else
            {
                return false;
            }
        }
    }

    #endregion

    #region ControlSettings

    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    Pl_Movement _playerInput;
    Animator _animator;
    CharacterController _characterController;

    [Header("Movement")]
    [SerializeField] private float _walkSpeed = 4f;
    [SerializeField] private float _sprintSpeed = 6f;
    [SerializeField] private float _strafeSpeed = 4f;
    [SerializeField] private float _crouchStrafeSpeed = 4f;
    [SerializeField] private float _crouchSpeed = 2f;
    [SerializeField] private float _slopeSlideSpeed = 8f;

    [Header("Mouse Settings")]
    [SerializeField] private Camera _playerCamera;
    [SerializeField] float _mouseClampYMin = -70f;
    [SerializeField] float _mouseClampYMax = 70f;
    public float MouseXSensitivity = 10f;
    public float MouseYSensitivity = 10f;
    public bool invertMouseXSensitivity;
    public bool invertMouseYSensitivity;
    float _cameraRotation = 0f;

    [Header("Jump Settings")]
    float _jumpVelocity;
    float _jumpGravity;
    float _maxJumpHeight = 2f;
    float _maxJumpTime = 0.75f;

    bool _requiredJumpPress = false;

    [Header("Gravity Settings")]
    [SerializeField] float _gravity = -9.8f;

    [Header("Sliding Parameters")]
    private Vector3 _hitPointNormal;

    [Header("Crouch")]
    [SerializeField] private float _crouchHeight = 0.4f;
    [SerializeField] private float _standHeight = 2f;
    [SerializeField] private float _timeToCrouch = 0.25f;
    [SerializeField] private Vector3 _crouchCentre = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 _standCentre = new Vector3(0, 0, 0);
    [SerializeField] bool _isCrouching = false;
    [SerializeField] bool _isCrouchingAnimation = false;

    [Header("Hash Ints")]
    int _isWalkingHash;
    int _isRunningHash;
    int _isJumpingHash;
    int _isFallingHash;
    int _isCrouchingHash;
    int _velocityXHash;
    int _velocityZHash;

    [Header("State Check")]
    [SerializeField] bool _isMovementPressed;
    [SerializeField] bool _isRunPressed;
    [SerializeField] bool _isCrouchPressed = false;
    [SerializeField] bool _isFalling;
    [SerializeField] bool _isJumpPressed = false;
    [SerializeField] bool _isJumping = false;

    [SerializeField] Vector2 _currentMovementInput;

    Vector3 _moveDirection;
    Vector3 _appliedMovement;

    [SerializeField] private float _smoothTime = 0.1f;

    #endregion

    private void OnEnable()
    {
        _playerInput.PlayerControls.Enable();
    }
    private void OnDisable()
    {
        _playerInput.PlayerControls.Disable();
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Initialize();
        if (!IsOwner)
        {
            _playerCamera.enabled = false;
            _playerInput.Disable();
        }
        else
        {
            _playerCamera.enabled = true;
            _playerInput.Enable();
        }
    }

    void Initialize()
    {
        _playerCamera = GetComponentInChildren<Camera>();
    }
    void Awake()
    {
        _playerInput = new Pl_Movement();
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();

        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();

        _isWalkingHash = Animator.StringToHash("isWalking");
        _isRunningHash = Animator.StringToHash("isRunning");
        _isJumpingHash = Animator.StringToHash("isJumping");
        _isFallingHash = Animator.StringToHash("isFalling");
        _isCrouchingHash = Animator.StringToHash("isCrouching");
        _velocityXHash = Animator.StringToHash("velocityX");
        _velocityZHash = Animator.StringToHash("velocityZ");

        _playerInput.PlayerControls.Movement.started += onMovementInput;
        _playerInput.PlayerControls.Movement.performed += onMovementInput;
        _playerInput.PlayerControls.Movement.canceled += onMovementInput;
        _playerInput.PlayerControls.Jump.started += onJump;
        _playerInput.PlayerControls.Jump.canceled += onJump;
        _playerInput.PlayerControls.Run.started += onRun;
        _playerInput.PlayerControls.Run.canceled += onRun;
        _playerInput.PlayerControls.Crouch.started += onCrouch;
        _playerInput.PlayerControls.Crouch.canceled += onCrouch;

        SetupJumpVariables();
    }
    private void Start()
    {
        _characterController.Move(_appliedMovement * Time.deltaTime);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        if (!Application.isFocused) return;

        handleMouseLook();

        _characterController.Move(_appliedMovement * Time.deltaTime);
        IsMovementPressed = CurrentMovementInput.x != 0 || CurrentMovementInput.y != 0;

        CurrentState.UpdateStates();

        SlideCheck();
    }

    void onMovementInput(InputAction.CallbackContext context) => _currentMovementInput = context.ReadValue<Vector2>();
    void onRun(InputAction.CallbackContext context) => _isRunPressed = context.ReadValueAsButton();
    void onJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
        _requiredJumpPress = false;
    }
    void onCrouch(InputAction.CallbackContext context)
    {
        _isCrouchPressed = context.ReadValueAsButton();
    }
    void handleMouseLook()
    {
        float mouseX = _playerInput.PlayerControls.Mouse.ReadValue<Vector2>().x * (invertMouseXSensitivity ? -MouseXSensitivity : MouseXSensitivity) * Time.deltaTime;
        float mouseY = _playerInput.PlayerControls.Mouse.ReadValue<Vector2>().y * (invertMouseYSensitivity ? -MouseYSensitivity : MouseYSensitivity) * Time.deltaTime;

        _cameraRotation -= mouseY;
        _cameraRotation = Mathf.Clamp(_cameraRotation, _mouseClampYMin, _mouseClampYMax);

        _playerCamera.transform.localRotation = Quaternion.Euler(_cameraRotation, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }

    void SetupJumpVariables()
    {
        float timeToApex = _maxJumpTime / 2;
        _jumpGravity = (-2 * _maxJumpHeight) / (timeToApex * timeToApex);
        _jumpVelocity = (2 * _maxJumpHeight) / timeToApex;
    }
    void SlideCheck()
    {
        if (isSliding)
            MoveDirection += new Vector3(HitPointNormal.x, -HitPointNormal.y, HitPointNormal.z) * SlopeSlideSpeed;
    }
}
