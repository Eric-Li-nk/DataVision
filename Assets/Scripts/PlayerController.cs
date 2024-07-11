using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float _movementSpeed =  50.0f;
    
    private Vector3 _direction = Vector3.zero;

    private void Update()
    {
        transform.position += (transform.forward * _direction.z + Vector3.up * _direction.y + transform.right * _direction.x).normalized * _movementSpeed * Time.deltaTime;
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
    
}
