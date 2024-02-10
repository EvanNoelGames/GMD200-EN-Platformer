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

        HandlePlayerMovement();
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
        // if the player hits a wall while a special jump is occuring than stop them
        if (!_isGrounded && xDesiredMovement != 0)
        {
            xDesiredMovement = 0;
            xDampenMovement = 1;
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y / 2);
        }

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

    private void HandlePlayerMovement()
    {
        _rb.velocity = new Vector2((_xMoveInput * xDampenMovement) + xDesiredMovement, _rb.velocity.y);

        Crouching();

        // if the player is grounded reset their movement modifiers
        if (_isGrounded)
        {
            xDesiredMovement = 0;
            xDampenMovement = 1;
        }
        // otherwise count them down
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
                if (crouching)
                {
                    if (_rb.velocity.x != 0 && canUncrouch)
                    {
                        CrouchJump();
                    }
                    else if (canUncrouch)
                    {
                        Debug.Log("bleh");
                        _rb.AddForce(Vector2.up * jumpForce);
                    }
                }
                else
                {
                    _rb.AddForce(Vector2.up * jumpForce);
                }
            }
            // if the player is not on the ground (but attempting to jump) check if they can walljump
            else
            {
                WallJump();
            }
            _shouldJump = false;
        }
    }

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

        // airborne and crouched
        if (!_isGrounded && crouching)
        {
            transform.localScale = new Vector3(transform.localScale.x, 1.0f, transform.localScale.z);
            crouching = false;
            xSpeed = xStartingSpeed;
        }
    }

    private void WallJump()
    {
        if (canWallJumpRight)
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
    }

    private void CrouchJump()
    {
        if (_rb.velocity.x > 0)
        {
            _rb.velocity = Vector2.zero;
            xDesiredMovement = 20.0f;
            xDampenMovement = 0.1f;
            _rb.AddForce(Vector2.up * (jumpForce * 0.75f));

            transform.localScale = new Vector3(transform.localScale.x, 1.0f, transform.localScale.z);
            crouching = false;
            xSpeed = xStartingSpeed;
        }
        else if (_rb.velocity.x < 0)
        {
            _rb.velocity = Vector2.zero;
            xDesiredMovement = -20.0f;
            xDampenMovement = 0.1f;
            _rb.AddForce(Vector2.up * (jumpForce * 0.75f));

            transform.localScale = new Vector3(transform.localScale.x, 1.0f, transform.localScale.z);
            crouching = false;
            xSpeed = xStartingSpeed;
        }
    }
}
