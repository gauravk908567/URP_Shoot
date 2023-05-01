using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class _PlayerStateMachine : MonoBehaviour
{
    #region ControlsSettingProperties

    #endregion

    #region ControlsSettings

    Pl_Movement _playerInput;
    Animator _animator;
    CharacterController _characterController;

    [Header("Movement")]
    [SerializeField] private float _walkSpeed = 4f;
    [SerializeField] private float _sprintSpeed = 6f;
    [SerializeField] private float _strafeSpeed = 6f;

    [Header("Crouch")]
    [SerializeField] private float _crouchHeight = 0.4f;

    #endregion

    private void OnEnable()
    {
        _playerInput.PlayerControls.Enable();
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
    }

    private void OnDisable()
    {
        _playerInput.PlayerControls.Disable();
    }
    void Awake()
    {
        _playerInput = new Pl_Movement();
    }


    void Update()
    {

    }
}
