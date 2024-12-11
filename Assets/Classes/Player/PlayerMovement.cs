using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector3 Velocity;
    private Vector3 PlayerMovementInput;
    private Vector2 PlayerMouseInput;
    private float xRot;

    [SerializeField] private Transform PlayerCamera;
    [SerializeField] private CharacterController Controller;

    [SerializeField] private float Speed;
    [SerializeField] private float Sensitivity;

    private void Update()
    {
        PlayerMovementInput= new Vector3(Input.GetAxisRaw("Horizontal"),0f,Input.GetAxisRaw("Vertical"));
        PlayerMouseInput= new Vector2(Input.GetAxisRaw("Mouse X"),Input.GetAxisRaw("Mouse Y"));
        
        MovePlayer();
        MovePlayerCamera();
    }

    private void MovePlayer()
    {
        Vector3 MoveVector = transform.TransformDirection(PlayerMovementInput);

        if (Input.GetKey(KeyCode.Space))
        {
            Velocity.y = -1f;
        }
        else if(Input.GetKey(KeyCode.LeftShift))
        {
            Velocity.y = 1f;
        }
        Controller.Move(MoveVector*Speed*Time.deltaTime);
        Controller.Move(Velocity*Speed*Time.deltaTime);

        Velocity.y = 0f;
    }

    private void MovePlayerCamera()
    {
        if (Input.GetMouseButton(1))
        {
            xRot -= PlayerMouseInput.y * Sensitivity;
            transform.Rotate(0f, PlayerMouseInput.x * Sensitivity, 0f);
            PlayerCamera.transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        }
    }
}
