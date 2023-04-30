using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
	public float cameraSensitivity = 90;
	public float climbSpeed = 4;
	public float normalMoveSpeed = 10;
	public float slowMoveFactor = 0.25f;
	public float fastMoveFactor = 3;

	private float rotationX = 0.0f;
	private float rotationY = 0.0f;

	public PlayerInput _playerInput;

	public bool oculus = true;
	Rigidbody rb;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		rotationX = transform.eulerAngles.y;
		_playerInput = GetComponent<PlayerInput>();
	}

	void Update()
	{
		/*
		if (_playerInput.actions["oculus"].WasPressedThisFrame())
		{
			oculus = !oculus;
		}

		if (!oculus)
		{*/
		// Cache deltaTime!
		var deltaTime = Time.deltaTime;
		rotationX += _playerInput.actions["MoveCameraX"].ReadValue<float>() * cameraSensitivity * deltaTime;
		rotationY += _playerInput.actions["MoveCameraY"].ReadValue<float>() * cameraSensitivity * deltaTime;
		rotationY = Mathf.Clamp(rotationY, -90, 90);

		var tempRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
		tempRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
		transform.localRotation = Quaternion.Slerp(transform.localRotation, tempRotation, deltaTime * 6.0f);
		//}

		Vector3 movement;

		if (_playerInput.actions["Fast"].ReadValue<float>() > .1f)
		{
			movement = transform.forward * fastMoveFactor * _playerInput.actions["MoveY"].ReadValue<float>() +
				transform.right * fastMoveFactor * _playerInput.actions["MoveX"].ReadValue<float>();
		}
		else if (_playerInput.actions["Slow"].ReadValue<float>() > .1f)
		{
			movement = transform.forward * slowMoveFactor * _playerInput.actions["MoveY"].ReadValue<float>() +
				transform.right * slowMoveFactor * _playerInput.actions["MoveX"].ReadValue<float>();
		}
		else
		{
			movement = transform.forward * _playerInput.actions["MoveY"].ReadValue<float>() +
				transform.right * _playerInput.actions["MoveX"].ReadValue<float>();
		}
		if (_playerInput.actions["Dive"].ReadValue<float>() > .1f)
		{
			movement -= transform.up * climbSpeed;
		}
		if (_playerInput.actions["Rise"].ReadValue<float>() > .1f)
		{
			movement += transform.up * climbSpeed;
		}
		rb.velocity = movement * normalMoveSpeed;
	}
}
