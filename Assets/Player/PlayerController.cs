using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    public float moveSpeed = 5.0f;
    public float jumpForce = 5.0f;
    public float jumpFloatTime = 0.5f;
    public float gravityForce = 1.0f;
    
    public bool isGrounded = true;
    public bool isFloating = false;
    private Coroutine floatingCoroutine;

    private float airResistanceModifier = 1.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Get movement input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Move relative to player's rotation
        Vector3 moveDirection = (transform.right * horizontal + transform.forward * vertical).normalized;

        rb.velocity = new Vector3(moveDirection.x * moveSpeed * airResistanceModifier, 
                                  rb.velocity.y, 
                                  moveDirection.z * moveSpeed * airResistanceModifier);

        // Jump logic
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // Floating logic: Holding Space allows floating
        if (isFloating && !Input.GetKey(KeyCode.Space))
        {
            StopFloating();
        }

        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * 9.8f, ForceMode.Acceleration);
            airResistanceModifier = 0.5f;
        }
        else
        {
            airResistanceModifier = 1.0f;
        }
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // Reset vertical velocity
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
        floatingCoroutine = StartCoroutine(StartFloatTime(jumpFloatTime));
    }

    void StopFloating()
    {
        if (floatingCoroutine != null)
        {
            StopCoroutine(floatingCoroutine);
        }
        isFloating = false;
    }

    IEnumerator StartFloatTime(float duration)
    {
        isFloating = true;
        yield return new WaitForSeconds(duration);
        isFloating = false;
    }

    void FixedUpdate()
    {
        // Apply extra gravity when floating ends
        if (!isFloating && !isGrounded && !Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(Vector3.down * gravityForce, ForceMode.Impulse);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Floor"))
        {
            isGrounded = true;
        }
    }
}
