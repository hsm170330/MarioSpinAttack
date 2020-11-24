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
    public float SpinDelay = 1f;
    [SerializeField] ParticleSystem spinP = null;

    //Audio
    [SerializeField] AudioClip SpinAudio = null;
    [SerializeField] AudioClip JumpAudio = null;

    //Animations
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        isAlive = true;
        isSpin = false;
        animator = GetComponent<Animator>();
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

                //animation
                animator.SetBool("IsWalking", true);
            }
            else
            {
                animator.SetBool("IsWalking", false);
            }
            //Debug.Log(horizontal);

            
            
            //when we jump
            if (Input.GetButtonDown("Jump") && isGrounded && !isSpin)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

                //audio
                AudioManager.PlayClip2D(JumpAudio, 1);

                //animation
                animator.SetBool("IsJumping", true);
                Invoke("StopJumpAnim", 1.2f);
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

        //play audio
        AudioManager.PlayClip2D(SpinAudio, 1);

        //animation
        animator.SetBool("IsSpining", true);
        Invoke("StopSpinAnim", 1f);
        spinP.Play();

        //we need two delays, one to disable the spin animation, and one to delay when we can spin again
        Invoke("ResetSpinAttack", SpinDelay);
        Invoke("SpinAgain", 2f);
    }


    // reset methods
    void ResetSpinAttack()
    {
        Debug.Log("Done Spinning");
        spin.SetActive(false);
        
    }

    void StopJumpAnim()
    {
        animator.SetBool("IsJumping", false);
    }

    void StopSpinAnim()
    {
        animator.SetBool("IsSpining", false);
    }

    void SpinAgain()
    {
        Debug.Log("We can spin again now");
        isSpin = false;
    }
}
