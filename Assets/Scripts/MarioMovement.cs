using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioMovement : MonoBehaviour
{
    public CharacterController mario;

    public float speed = 20f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;
    public LayerMask groundMask;
    Vector3 velocity;
    bool isGrounded;
    bool isAlive;
    public Transform cam;

    //spin variables
    bool isSpin;
    public GameObject spin = null;
    public float SpinDelay = 2f;

    // Start is called before the first frame update
    void Start()
    {
        isAlive = true;
        isSpin = false;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (isAlive)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                mario.Move(moveDir.normalized * speed * Time.deltaTime);
            }
            
            //when we jump
            if (Input.GetButtonDown("Jump") && isGrounded && !isSpin)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            //when we spin
            if (Input.GetButtonDown("Fire1") && isSpin == false)
            {
                SpinAttack();
            }
            velocity.y += gravity * Time.deltaTime;

            mario.Move(velocity * Time.deltaTime);
        }
    }

    void SpinAttack()
    {
        Debug.Log("Spin");
        isSpin = true;
        spin.SetActive(true);

        //check to see whether we are in the air or not
        //if we are in the air, we get a bonus jump from the spin attack
        if (isGrounded == false)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        //we need a delay so that we can't just keep spinning
        Invoke("ResetSpinAttack", SpinDelay);
    }

    void ResetSpinAttack()
    {
        Debug.Log("Done Spinning");
        spin.SetActive(false);
        isSpin = false;
    }
}
