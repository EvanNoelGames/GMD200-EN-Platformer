using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float xSpeed = 10f;
    public float XSpeed => xSpeed;
    [SerializeField] private float jumpForce = 800f;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D _rb;

    private float _xMoveInput;
    private float xDesiredMovement;
    private float xDampenMovement;

    private bool _shouldJump;
    private bool _isGrounded;
    private bool _canWallJumpRight;
    private bool _canWallJumpLeft;

    public bool IsGrounded => _isGrounded;

    private void Awake()
    {
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
    }

    private void FixedUpdate()
    {
        Collider2D col = Physics2D.OverlapCircle(transform.position, groundCheckRadius, groundLayer);
        Collider2D wallColLeft = Physics2D.OverlapBox(new Vector2(transform.position.x + 0.3f, transform.position.y + 0.75f), new Vector2(0.4f, 0.1f), 0f, groundLayer);
        Collider2D wallColRight = Physics2D.OverlapBox(new Vector2(transform.position.x - 0.3f, transform.position.y + 0.75f), new Vector2(0.4f, 0.1f), 0f, groundLayer);

        _isGrounded = col != null;
        _canWallJumpRight = wallColRight != null;
        _canWallJumpLeft = wallColLeft != null;

        _rb.velocity = new Vector2((_xMoveInput * xDampenMovement) + xDesiredMovement, _rb.velocity.y);

        if (_isGrounded)
        {
            xDesiredMovement = 0;
            xDampenMovement = 1;
        }
        else if (xDesiredMovement > 0)
        {
            xDesiredMovement -= 0.25f;
        }
        else if (xDesiredMovement < 0)
        {
            xDesiredMovement += 0.25f;
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
            else if (_canWallJumpRight)
            {
                _rb.velocity = Vector2.zero;
                xDesiredMovement = 10.0f;
                xDampenMovement = 0.1f;
                _rb.AddForce(Vector2.up * jumpForce);
            }
            else if (_canWallJumpLeft)
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
        Gizmos.color = _canWallJumpLeft ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, groundCheckRadius);
        Gizmos.DrawWireCube(new Vector3(transform.position.x - 0.3f, transform.position.y + 0.75f, transform.position.z), new Vector2(0.4f, 0.1f));
        Gizmos.DrawWireCube(new Vector3(transform.position.x + 0.3f, transform.position.y + 0.75f, transform.position.z), new Vector2(0.4f, 0.1f));
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
}
