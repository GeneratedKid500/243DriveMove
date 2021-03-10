using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    CapsuleCollider pCollider;

    [Header("Movement")]
    ////walking
    [SerializeField] float walkSpeed = 8;
    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;
    ////crouching
    float standHeight;
    float crouchHeight;
    float crouchTimer;
    bool crouching;

    [Header("Jumping")]
    [SerializeField] float jumpForce = 220;
    [SerializeField] float fastFallMultiplier = 2.5f;
    [SerializeField] float lowJumpMultiplier = 2.5f;
    bool grounded;
    public LayerMask groundedMask;

    [Header("Camera Rotation")]
    Transform cameraT;
    float verticalLookRotation;
    [SerializeField] float mouseSensitivityX = 250f;
    [SerializeField] float mouseSensitivityY = 250f;
    [SerializeField] float MinRotationY = -60f;
    [SerializeField] float MaxRotationY = 60f;

    [Header("Stairs")]
    public bool enableStairs;
    public GameObject stepRayUpper;
    public GameObject stepRayLower;
    [SerializeField] float stepSmooth = 2f;
    [SerializeField] float stepDistance = 0.1f;
    public LayerMask stairMask; //layer mask to determine what a stair is

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; //locks inside area and makes mouse cursor invisible
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cameraT = Camera.main.transform; //gets camera's transform

        //StepCheck
        if (enableStairs)
        {
            stepRayLower.transform.position = new Vector3(stepRayUpper.transform.position.x,
            stepRayLower.transform.position.y, stepRayUpper.transform.position.z);
        }

        pCollider = GetComponent<CapsuleCollider>();
        standHeight = pCollider.height;
        crouchHeight = pCollider.height / 2;
    }


    void Update()
    {
        ////CAMERA MOVEMENT
        CAMERA();

        //// CALC MOVEMENT
        float inputX = Input.GetAxisRaw("Horizontal"); //gets x input
        float inputY = Input.GetAxisRaw("Vertical"); //gets z input

        Vector3 moveDir = new Vector3(inputX, 0, inputY).normalized; //adds them as vector to get wanted direction 
        //normalised to stop from acually moving
        Vector3 targetMoveAmount = moveDir * walkSpeed; //calculates movement amount via dierction and walk speed
        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, .15f);
        //calculates the player's move from current to wanted position via SmoothDamp to ensure smooth movement
        //passes current velocity as a ref to ensure it gets updated

        //CROUCHING
        Crouch();

        ////Jump
        JUMP();
    }

    void FixedUpdate()
    {
        grounded = isGrounded(); //updates is grounded

        ////STAIRCLIMB
        if (enableStairs)
        {
            STAIRS(new Vector3(1.5f, 0, 1f)); //45' X
            STAIRS(Vector3.forward);
            STAIRS(new Vector3(-1.5f, 0, 1f)); //-45' X
        }
        //calculates all of these to ensure the player can get up some stairs
        //as just being straight on can lead to some issues such as not getting stairs

        //APPLY MOVEMENT
        Vector3 localMove = transform.TransformDirection(moveAmount) * Time.fixedDeltaTime; //move relatively
        //calculates move one final time based on the players relative position  
        rb.MovePosition(rb.position + localMove); //finally moves rigidbody
    }

    void CAMERA() //camera movement (including player on x val)
    {
        //rotates player based on Mouse X 
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivityX);

        //calcuates camerra movement based on Mouse Y as well the sensitivity
        verticalLookRotation += Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivityY;
        //clamping stops from going under or over certain value
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, MinRotationY, MaxRotationY);
        cameraT.localEulerAngles = Vector3.left * verticalLookRotation; //adjusts camera travsform using realtive euler angles
    }

    void Crouch() //gets crouch input
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl))
        {
            crouching = !crouching; //simpler than additional KeyUp
            ApplyCrouch();
        }
        else if (crouchTimer > 3 && Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.C) || Input.GetKeyUp(KeyCode.LeftControl))
        {
            crouching = !crouching;
            crouchTimer = 0;
            ApplyCrouch();
        }

        // if player holds button, will auto stand up once key is released.
        //this should be toggleable on the main menu
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.LeftControl))
        {
            crouchTimer += Time.deltaTime;
        }
    }

    void ApplyCrouch() //applies crouch movement
    {
        if (crouching)
            pCollider.height = crouchHeight; //lower height
        else
            pCollider.height = standHeight; //upper height

        //halves the height of the collider
        //lazy but effective solution as its not like the player will be seeing through the floor
    }

    void STAIRS(Vector3 rayDir) //uses raycasts to check if the player is hitting stairs and if the should go up
    {
        RaycastHit hitlower;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(rayDir), out hitlower, stepDistance)) //check lower bound
        {
            RaycastHit hitUpper;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(rayDir), out hitUpper, stepDistance + stepDistance)) //check upper bound
            {
                ///rb.MovePosition(rb.position - new Vector3(0f, -stepSmooth * Time.deltaTime, 0f)); //move to normal, world up
                rb.MovePosition(rb.position - (transform.up * -stepSmooth) * Time.deltaTime); //Move towards relative up
            }
        }

        //debug draws raycasts 
        Debug.DrawRay(stepRayLower.transform.position, transform.TransformDirection(rayDir));
        Debug.DrawRay(stepRayUpper.transform.position, transform.TransformDirection(rayDir));
    }

    void JUMP() //gets the jump input and applies it
    {
        if (Input.GetButtonDown("Jump") && grounded)
            rb.AddForce(transform.up * jumpForce); //Jump to the relative up

        if (rb.velocity.y < 0) //Fast Fall if no longer going up
            rb.velocity += transform.up * Physics.gravity.y * (fastFallMultiplier - 1) * Time.deltaTime;
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump")) //Low Jump if no longer pressing jump button
            rb.velocity += transform.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
    }

    bool isGrounded() //checks if grounded (returns bool)
    {
        Ray ray = new Ray(transform.position, -transform.up); //ray sent down down
        if (Physics.Raycast(ray, out RaycastHit hit, 1 + .1f, groundedMask)) //true if grounded
            return true;
        else
            return false;
    }
}
