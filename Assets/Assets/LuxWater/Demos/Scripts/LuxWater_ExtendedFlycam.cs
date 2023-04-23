using UnityEngine;
using System.Collections;
//using UnityEngine.InputSystem;


namespace LuxWater.Demo {
 
	public class LuxWater_ExtendedFlycam : MonoBehaviour
	{


	// slightly changed....
	 
		/*
		EXTENDED FLYCAM
			Desi Quintans (CowfaceGames.com), 17 August 2012.
			Based on FlyThrough.js by Slin (http://wiki.unity3d.com/index.php/FlyThrough), 17 May 2011.
	 
		LICENSE
			Free as in speech, and free as in beer.
	 
		FEATURES
			WASD/Arrows:    Movement
			          Q:    Dropp
			          E:    Climb
	                      Shift:    Move faster
	                    Control:    Move slower
	                        End:    Toggle cursor locking to screen (you can also press Ctrl+P to toggle play mode on and off).
		*/
	 
		public float cameraSensitivity = 90;
		public float climbSpeed = 4;
		public float normalMoveSpeed = 10;
		public float slowMoveFactor = 0.25f;
		public float fastMoveFactor = 3;
	 
		private float rotationX = 0.0f;
		private float rotationY = 0.0f;
		private float min_height = -46;
		private float max_height = -1;
		private bool isOrtho = false;
		private Camera cam;
		private RaycastHit hit;
		private bool terraincolision = false;
		private float terraindistance = 3;
		public LayerMask terrainLayer;

		//public PlayerInput _playerInput;

		void Start() {
			if (min_height > max_height)
            {
				max_height = min_height;
            } 
			rotationX = transform.eulerAngles.y;
			cam = GetComponent<Camera>();
			//if (cam != null) {
				//isOrtho = cam.orthographic;
			//}
			//_playerInput = GetComponent<PlayerInput>();
		}

		void FixedUpdate()
		{
			terraincolision = Physics.Raycast(transform.position, new Vector3(0, -1, 0),out hit, terraindistance,terrainLayer);

		}

		void Update ()
		{
			/*
			// Cache deltaTime!
			var deltaTime = Time.deltaTime;	
			rotationX += _playerInput.actions["MoveCameraX"].ReadValue<float>() * cameraSensitivity * deltaTime;
			rotationY += _playerInput.actions["MoveCameraY"].ReadValue<float>() * cameraSensitivity * deltaTime;
			rotationY = Mathf.Clamp (rotationY, -90, 90);
	 
			var tempRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
			tempRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
			transform.localRotation = Quaternion.Slerp(transform.localRotation, tempRotation, deltaTime * 6.0f);
	 
		 	if (_playerInput.actions["Fast"].ReadValue<float>() > .1f)
		 	{
				transform.position += transform.forward * (normalMoveSpeed * fastMoveFactor) * _playerInput.actions["MoveY"].ReadValue<float>() * deltaTime;
				transform.position += transform.right * (normalMoveSpeed * fastMoveFactor) * _playerInput.actions["MoveX"].ReadValue<float>() * deltaTime;
		 	}
		 	else if (_playerInput.actions["Slow"].ReadValue<float>() > .1f)
		 	{
				transform.position += transform.forward * (normalMoveSpeed * slowMoveFactor) * _playerInput.actions["MoveY"].ReadValue<float>() * deltaTime;
				transform.position += transform.right * (normalMoveSpeed * slowMoveFactor) * _playerInput.actions["MoveX"].ReadValue<float>() * deltaTime;
		 	}
		 	else
		 	{
				if(isOrtho) {
					cam.orthographicSize *= (1.0f - _playerInput.actions["MoveY"].ReadValue<float>() * deltaTime);
				}
				else {
					transform.position += transform.forward * normalMoveSpeed * _playerInput.actions["MoveY"].ReadValue<float>() * deltaTime;
				}
				transform.position += transform.right * normalMoveSpeed * _playerInput.actions["MoveX"].ReadValue<float>() * deltaTime;
		 	}
	 
			if (_playerInput.actions["Dive"].ReadValue<float>() > .1f) {transform.position -= transform.up * climbSpeed * deltaTime;}
			if (_playerInput.actions["Rise"].ReadValue<float>() > .1f) {transform.position += transform.up * climbSpeed * deltaTime;}

			if (transform.position.y > max_height)
            {
				transform.position = new Vector3(transform.position.x, max_height, transform.position.z);
            }


			if (transform.position.y < min_height)
			{
				transform.position = new Vector3(transform.position.x, min_height, transform.position.z);
			}
			
			if (terraincolision)
            {
				transform.position = new Vector3(transform.position.x, hit.point.y+terraindistance, transform.position.z);
			}
			*/
		}
	}


}