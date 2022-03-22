using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    private CharacterController controller;

    //different speeds:
    public float walkSpeed = 5f;
    public float runSpeed = 12f;
    private float playerSpeed;

    //gravity application:
    private float verticalVelocity; //we only use its y-coord
    public float gravity = -9.81f; //default gravity in m/s (changing to -20f usually feels better)

    //to check if grounded:
    public Transform groundCheck; //for checking if grounded
    public float groundSphereRadius = 0.4f; //sphere drawn around 'groundCheck' obj
    public LayerMask groundMask; //layers sphere is checking for
    private bool isGrounded;

    //jumping varibles:
    public float jumpHeight = 3f; //height want player to be able to jump
    public float midAirSlopeLimit = 90;
    public float midAirStepOffset = 0.1f;

    //grounded resets for w/ jumping:
    public float groundedSlopeLimit = 45;
    public float groundedStepOffset = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //applies this downward velocity to make sure player on ground:
        float groundedDownwardVelocity = -2f; 

        //set if currently grounded:
        isGrounded = Physics.CheckSphere(groundCheck.position, groundSphereRadius, groundMask);

        //reset y velocity, slope limit, and step offset if player grounded and he wasnt before:
        if(isGrounded && verticalVelocity < 0)
        {
            //reset slope limit and step offset w/ land:
            controller.slopeLimit = groundedSlopeLimit;
            controller.stepOffset = groundedStepOffset;

            verticalVelocity = groundedDownwardVelocity; //not zero just incase player not totally on the grnd yet, this will force him to the grnd
        }

        //get input:
        float xInput = Input.GetAxisRaw("Horizontal"); //use 'Raw' instead so player stops instantly instead of sliding to a stop
        float zInput = Input.GetAxisRaw("Vertical");

        //set 'playerSpeed' to 'runSpeed' if grounded and left shift held down, otherwise set it to 'walkSpeed' if grounded:
        if(isGrounded)
        {
            if(Input.GetButton("Fire3")) //'Fire3' = axis for left shift, (mouse 2?), and controller something
            {
                playerSpeed = runSpeed;
            }
            else
            {
                playerSpeed = walkSpeed;
            }
        }

        //turn input into dir want to move relative to curr rot:
        //Vector3 direction = new Vector3(xInput, 0f, zInput).normalized; //dont want bc moves using global coords, so moves same way independent of player's already set rot
        Vector3 moveDirection = transform.right * xInput + transform.forward * zInput; //'.right' and '.forward' use obj's curr local coord syst (don't need to normalize?)

        //normalize move direction vector so moving diagonal not faster than in 1 dir:
        moveDirection = moveDirection.normalized;

        //apply player speed:
        //moveDirection *= playerSpeed;

        //move controller in desired direction:
        controller.Move(moveDirection * playerSpeed * Time.deltaTime);

        //jump and change slope limit, step offset if currently grounded and pressed the button:
        if(isGrounded && Input.GetButtonDown("Jump"))
        {
            //increase slope limit to avoid jitter w/ mid-air
            controller.slopeLimit = midAirSlopeLimit;

            //lower step offset so dont land on surfaces higher than feet:
            controller.stepOffset = midAirStepOffset;

            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity); // v = sqrt(h * -2 * g) is the velocity req'd to jump certain height
        }

        //apply gravity every second in freefall:
        verticalVelocity += gravity * Time.deltaTime;
        controller.Move( new Vector3(0, verticalVelocity * Time.deltaTime, 0));

        //combine move direction and y-velocity to move controller:
        //velocity = velocity + moveDirection;
        //controller.Move(velocity * Time.deltaTime);

        //reset y-coord velocity if controller collides w/ something above it (need to be below all 'Move()' calls bc they ret collision flags):
        if ((controller.collisionFlags & CollisionFlags.Above) != 0)
        {
            verticalVelocity = groundedDownwardVelocity;

            //check: Debug.Log("Vertical velocity reset bc of head collision");
        }
    }

    //draw black wire sphere around 'groundCheck' obj when player obj is selected:
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(groundCheck.position, groundSphereRadius);
    }
}
