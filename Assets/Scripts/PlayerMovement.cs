using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	private CharacterController controller;
	private Vector3 playerVelocity;
	private bool isGrounded;
	public float gravity = -9.8f;
	public float speed = 5f;
	public float jumpHeight = 1f;

    // Start is called before the first frame update
    void Start()
    {
		// get character controller from object
      controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
		isGrounded = controller.isGrounded;
    }
	
	 // Receive inputs from InputManager.cs and apply to character controller.
	 public void ProcessMove(Vector2 input)
	 {
		// empty Vector3 for char direction
		Vector3 moveDirection = Vector3.zero;

		// bind values from InputManager to character directions
		moveDirection.x = input.x;
		moveDirection.z = input.y;

		// controller.move moves game object in direction 
		controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);

		// apply gravity to player
		playerVelocity.y += gravity * Time.deltaTime;

		// if player is grounded only apply a small amount of gravity
		if (isGrounded && playerVelocity.y < 0)
		{
			playerVelocity.y = -2f;
		}

		// keep applying gravity over time
		controller.Move(playerVelocity * Time.deltaTime);
	 }

	 public void Jump()
	 {
		if (isGrounded)
		{
			playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
		}
	 }
}