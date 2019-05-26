using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 3;
    public float runSpeed = 6;
    public float crouchSpeed = 1.5f;
    public float gravity = -12;
    public float jumpHeight = 1;

    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;

    public float speedSmoothTime = 0.1f;
    float speedSmoothVelocity;
    float currentSpeed;
    float velocityY;

    //crouching
    [HideInInspector]
    public bool isCrouched;


    Animator animator;
    Transform cameraT;
    [HideInInspector]
    CharacterController controller;

    // new movement??
    //Rigidbody rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        cameraT = Camera.main.transform;
        controller = GetComponent<CharacterController>();
        //rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;
        bool running = Input.GetKey(KeyCode.LeftShift);

        Move(inputDir,running);

        if (isCrouched == true)
        {
            animator.SetBool("isCrouched", true);
            ChangeHeight(0);
        }
        else
        {
            animator.SetBool("isCrouched", false);
            ChangeHeight(1);
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            if(isCrouched == true)
            {
                isCrouched = false;
            }
            else
            {
                isCrouched = true;
            }
        }

        float animationSpeedPercent = ((running) ? currentSpeed / runSpeed : currentSpeed / walkSpeed * 0.5f);
        animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
    }
    void Move(Vector2 inputDir, bool running)
    {
        if (inputDir != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }

        if (running = Input.GetKey(KeyCode.LeftShift))
        {
            isCrouched = false;
        }
        float targetSpeed = ((running) ? runSpeed : (isCrouched) ? crouchSpeed : walkSpeed) * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        velocityY += Time.deltaTime * gravity;

        Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);
        currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

        if (controller.isGrounded)
        {
            velocityY = 0;
            animator.SetBool("isJumping", false);
        }
    }
    void Jump()
    {
        if (controller.isGrounded)
        {
            float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
            velocityY = jumpVelocity;
            animator.SetBool("isJumping", true);
            isCrouched = false;
        }
    }

    private void ChangeHeight(int state) // 0 = crouching 1= standup
    {
        if(state == 0)
        {
            controller.height = 1f;
            controller.center = new Vector3(0, 0.6f, 0);
            isCrouched = true;
        }
        else
        {
            controller.height = 1.75f;
            controller.center = new Vector3(0, 0.95f, 0);
            isCrouched = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Climbable"))
        {
            Debug.Log("hehe xd");
        }
    }

}
