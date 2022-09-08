using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // gain references to new input system

public class InputManager : MonoBehaviour
{
	[SerializeField] private PlayerInput playerInput;            // a script that connects action events to relevant code logic 
	[SerializeField] private PlayerInput.OnFootActions onFoot;   // reference to OnFoot action map
	[SerializeField] private PlayerMovement movement;            // reference to PlayerMovement.cs
	[SerializeField] private PlayerLook look;

    // Awake is called only once when the script instance is being loaded
    void Awake()
    {
       playerInput = new PlayerInput();            // instantiate PlayerInput component
		 onFoot = playerInput.OnFoot;                // instantiate OnFoot action map
		 movement = GetComponent<PlayerMovement>();  // reference to PlayerMovement script
		 look = GameObject.Find("CameraHolder").transform.GetChild(0).GetComponent<PlayerLook>();  // get PlayerLook.cs from camera
		 
		 // anytime jump is performed use callback context to trigger Jump()
		 onFoot.Jump.performed += ctx => movement.Jump();
    }

    // FixedUpdate is frame rate independent and used for physics calculations
    void FixedUpdate()
    {
		// send values read from Movement action to ProcessMove() to control character
		movement.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
    }

	 // Called last every frame, Useful for follow camera bc it tracks objects that may have moved in Update()
	 void LateUpdate()
	 {
		look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
	 }
	
	 // Enable action map
	 private void OnEnable()
	 {
		onFoot.Enable();
	 }

	 // Disable action map
	 private void OnDisable()
	 {
		onFoot.Disable();
	 }
}
