using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float xSpeed = 10f;
    private float xStartingSpeed;
    public float XSpeed => xSpeed;
    [SerializeField] private float jumpForce = 800f;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D _rb;

    private float _xMoveInput;
    private float xDesiredMovement;
    private float xDampenMovement;

    private bool _shouldJump;
    private bool _shouldCrouch;
    private bool _isGrounded;

    private bool canWallJumpRight;
    private bool canWallJumpLeft;

    private bool crouching;
    private bool canUncrouch;

    public bool IsGrounded => _isGrounded;

    private void Awake()
    {
        xStartingSpeed = xSpeed;
        _rb = GetComponent<Rigidbody2D>();
        xDampenMovement = 1;
    }

    // Update is called once per frame
    void Update()
    {
        _xMoveInput = Input.GetAxis("Horizontal") * xSpeed;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _shouldJump = true;
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (_isGrounded && !crouching)
            {
                _shouldCrouch = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            _shouldCrouch = false;
        }
    }

    private void FixedUpdate()
    {
        Debug.Log(canUncrouch);
        Collider2D col = Physics2D.OverlapCircle(transform.position, groundCheckRadius, groundLayer);
        Collider2D wallColLeft = Physics2D.OverlapBox(new Vector2(transform.position.x + 0.2f, transform.position.y + 0.75f), new Vector2(0.4f, 0.3f), 0f, groundLayer);
        Collider2D wallColRight = Physics2D.OverlapBox(new Vector2(transform.position.x - 0.2f, transform.position.y + 0.75f), new Vector2(0.4f, 0.3f), 0f, groundLayer);
        Collider2D crouchCol = Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y + 0.75f), new Vector2(0.5f, 0.5f), 0f, groundLayer);

        _isGrounded = col != null;
        canWallJumpRight = wallColRight != null;
        canWallJumpLeft = wallColLeft != null;

        if (crouchCol != null)
        {
            canUncrouch = false;
        }
        else
        {
            canUncrouch = true;
        }

        _rb.velocity = new Vector2((_xMoveInput * xDampenMovement) + xDesiredMovement, _rb.velocity.y);

        Crouching();

        if (_isGrounded)
        {
            xDesiredMovement = 0;
            xDampenMovement = 1;
        }
        else
        {
            if (xDesiredMovement > 0)
            {
                xDesiredMovement -= 0.25f;
            }
            else if (xDesiredMovement < 0)
            {
                xDesiredMovement += 0.25f;
            }
        }

        // give the player less control after a wall jump
        if (xDampenMovement < 1)
        {
            xDampenMovement += 0.025f;
        }

        if (_shouldJump)
        {
            if (_isGrounded)
            {
                _rb.AddForce(Vector2.up * jumpForce);
            }
            else if (canWallJumpRight)
            {
                _rb.velocity = Vector2.zero;
                xDesiredMovement = 10.0f;
                xDampenMovement = 0.1f;
                _rb.AddForce(Vector2.up * jumpForce);
            }
            else if (canWallJumpLeft)
            {
                _rb.velocity = Vector2.zero;
                xDesiredMovement = -10.0f;
                xDampenMovement = 0.1f;
                _rb.AddForce(Vector2.up * jumpForce);
            }
            _shouldJump = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, groundCheckRadius);
        Gizmos.DrawWireCube(new Vector3(transform.position.x - 0.2f, transform.position.y + 0.75f, transform.position.z), new Vector2(0.4f, 0.3f));
        Gizmos.DrawWireCube(new Vector3(transform.position.x + 0.2f, transform.position.y + 0.75f, transform.position.z), new Vector2(0.4f, 0.3f));
        Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z), new Vector2(0.5f, 0.5f));
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Moving Platform"))
        {
            transform.SetParent(other.transform, true);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Moving Platform"))
        {
            transform.SetParent(null, true);
        }
    }

    // get parent scale and multiply it maybe
    private void Crouching()
    {
        if (_shouldCrouch)
        {
            transform.localScale = new Vector3(transform.localScale.x, 0.5f, transform.localScale.z);
            crouching = true;
            xSpeed = xStartingSpeed / 4;
        }
        else if (canUncrouch)
        {
            transform.localScale = new Vector3(transform.localScale.x, 1.0f, transform.localScale.z);
            crouching = false;
            xSpeed = xStartingSpeed;
        }

        // airbourne and crouching
        if (!_isGrounded && crouching)
        {
            transform.localScale = new Vector3(transform.localScale.x, 1.0f, transform.localScale.z);
            crouching = false;
            xSpeed = xStartingSpeed;
        }
    }
}
