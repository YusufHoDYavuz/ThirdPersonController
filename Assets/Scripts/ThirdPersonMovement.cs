using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController cc;
    public Transform cam;
    Animator anim;

    public float speed;
    public float turnSmoothTime;
    float turnVelocityTime;

    [Space]
    [Header("Jump")]
    Vector3 velocity;
    bool isGrounded;
    public Transform ground;
    public float distance = 0.3f;
    public float jumpHeight;
    public float gravity;
    public LayerMask mask;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;

        anim = GetComponent<Animator>();
    }

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontalInput, 0, verticalInput).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocityTime, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            cc.Move(moveDir.normalized * speed * Time.deltaTime);
        }


       
        Jump();
        AnimInput();
    }

    void Jump()
    {

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !Input.GetKey(KeyCode.W))
        {
            anim.SetTrigger("isJump");
            StartCoroutine(jumpFast());
        }

        isGrounded = Physics.CheckSphere(ground.position, distance, mask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0;
        }

        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);

    }

    IEnumerator jumpFast()
    {
        yield return new WaitForSeconds(0.40f);
        velocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
    }

    void AnimInput()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S))
            anim.SetBool("isWalk", true);
        else
            anim.SetBool("isWalk", false);

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
        {
            anim.SetBool("isRun", true);
            speed = 4;
        }
        else
        {
            anim.SetBool("isRun", false);
            speed = 1;
        }

        if (Input.GetKey(KeyCode.W) && isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger("isJumpForward");
            jumpHeight = 0.5f;
            velocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
        else
        {
            jumpHeight = 0.75f;
        }

    }
}
