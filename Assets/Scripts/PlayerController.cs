using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float _movementSpeed =  150.0f;
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private Camera _overviewCamera;
    
    private Vector3 _direction = Vector3.zero;
    private float _sprintFactor = 1.0f;

    private void Update()
    {
        transform.position += (transform.forward * _direction.z + Vector3.up * _direction.y + transform.right * _direction.x).normalized * _movementSpeed * Time.deltaTime * _sprintFactor;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();
        _direction.x = dir.x;
        _direction.z = dir.y;
    }

    public void OnVerticalMove(InputAction.CallbackContext context)
    {
        float dir = context.ReadValue<float>();
        _direction.y = dir;
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if(context.performed)
            _sprintFactor = 2.0f;
        else if (context.canceled)
            _sprintFactor = 1.0f;
    }

    public void SwitchCamera(InputAction.CallbackContext context)
    {
        _playerCamera.enabled = !_playerCamera.enabled;
        _overviewCamera.enabled = !_overviewCamera.enabled;
    }
    
}
