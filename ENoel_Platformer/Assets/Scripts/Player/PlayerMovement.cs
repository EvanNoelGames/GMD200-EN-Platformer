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
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D _rb;

    private PlayerManager _playerManager;

    private float _xMoveInput;
    private float xDesiredMovement;
    private float xDampenMovement;

    private float xInitialVelocity;
    private float initalIceVelocity;
    private float yModifier;

    private bool touchingIce;

    private bool _shouldJump;
    private bool _shouldCrouch;
    private bool _isGrounded;
    private bool hitCeiling;

    private bool canWallJumpRight;
    private bool canWallJumpLeft;

    private bool crouching;
    private bool canUncrouch;

    private bool canDive;

    public bool flipping;

    public bool respawn;

    public bool IsGrounded => _isGrounded;

    private void Awake()
    {
        _playerManager = GetComponent<PlayerManager>();
        xStartingSpeed = xSpeed;
        flipping = false;
        xDampenMovement = 1;
        _rb = GetComponent<Rigidbody2D>();
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
        Collider2D col = Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y), new Vector2(0.4f, 0.2f), 0f, groundLayer);
        Collider2D wallColLeft = Physics2D.OverlapBox(new Vector2(transform.position.x + 0.2f, transform.position.y + 0.75f), new Vector2(0.4f, 0.3f), 0f, groundLayer);
        Collider2D wallColRight = Physics2D.OverlapBox(new Vector2(transform.position.x - 0.2f, transform.position.y + 0.75f), new Vector2(0.4f, 0.3f), 0f, groundLayer);
        Collider2D crouchCol = Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y + 0.75f), new Vector2(0.4f, 0.5f), 0f, groundLayer);

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

        if (_playerManager.Playing)
        {
            HandlePlayerMovement();
        }
        else
        {
            _rb.velocity = Vector3.zero;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y, transform.position.z), new Vector2(0.4f, 0.2f));
        Gizmos.DrawWireCube(new Vector3(transform.position.x - 0.2f, transform.position.y + 0.75f, transform.position.z), new Vector2(0.4f, 0.3f));
        Gizmos.DrawWireCube(new Vector3(transform.position.x + 0.2f, transform.position.y + 0.75f, transform.position.z), new Vector2(0.4f, 0.3f));
        Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z), new Vector2(0.4f, 0.5f));
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // if the player hits a wall while a special jump is occuring then stop them
        if (!_isGrounded)
        {
            if (xDesiredMovement != 0)
            {
                xInitialVelocity = 0;
                hitCeiling = true;
                xDesiredMovement = 0;
                xDampenMovement = 0.5f;
                _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y / 2);
            }
        }

        if (!other.gameObject.CompareTag("Ice"))
        {
            if (_isGrounded)
            {
                initalIceVelocity = 0;
            }
        }

        if (other.gameObject.CompareTag("Moving Platform"))
        {
            if (transform.position.y > other.transform.position.y)
            {
                transform.SetParent(other.transform, true);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ice"))
        {
            touchingIce = true;
            if (!_isGrounded)
            {
                if (_rb.velocity.y < 0)
                {
                    yModifier -= 0.05f;
                }
            }
            else
            {
                if (initalIceVelocity < 0.5f && initalIceVelocity > -0.5f)
                {
                    initalIceVelocity = 0f;
                }
                if (_xMoveInput != 0)
                {
                    initalIceVelocity = Mathf.Clamp(_rb.velocity.x * 0.75f, -10f, 10f);
                }
                else if (initalIceVelocity > 0)
                {
                    initalIceVelocity -= 0.1f;
                }
                else if (initalIceVelocity < 0)
                {
                    initalIceVelocity += 0.1f;
                }
            }
        }
        else
        {
            initalIceVelocity = 0;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Moving Platform"))
        {
            transform.SetParent(null, true);
        }
        else if (other.gameObject.CompareTag("Ice"))
        {
            touchingIce = false;
        }
    }

    private void HandlePlayerMovement()
    {
        if (!respawn)
        {
            _rb.velocity = new Vector2((_xMoveInput * xDampenMovement) + xDesiredMovement + xInitialVelocity + initalIceVelocity, _rb.velocity.y + yModifier);
        }
        else
        {
            _rb.velocity = Vector2.zero;
            StartCoroutine(Co_DoneRespawning());
        }

        Crouching();

        // if the player is grounded reset their movement modifiers
        if (_isGrounded)
        {
            yModifier = 0;
            xInitialVelocity = 0;
            canDive = true;
            hitCeiling = false;
            flipping = false;
            xDesiredMovement = 0;
            xDampenMovement = 1;
        }
        else
        {
            // bring down the initial velocity quicker if the player is attempting to change directions
            if (transform.localScale.x > 0 && _xMoveInput < 0 && xInitialVelocity > 0)
            {
                xInitialVelocity -= 0.5f;
            }
            else if (transform.localScale.x < 0 && _xMoveInput > 0 && xInitialVelocity < 0)
            {
                xInitialVelocity += 0.5f;
            }

            // if player is not flipping then count the movement modifiers down
            if (!flipping)
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

            if (yModifier < 0 && !touchingIce)
            {
                yModifier += 0.1f;
            }

            initalIceVelocity = 0f;
        }

        // give the player less control after a non-flip jump
        if (xDampenMovement < 1 && !flipping)
        {
            xDampenMovement += 0.025f;

            // slowly bring initial velocity back to 0
            if (xInitialVelocity != 0)
            {
                if (xInitialVelocity < 0)
                {
                    xInitialVelocity += 0.05f;
                }
                else if (xInitialVelocity > 0)
                {
                    xInitialVelocity -= 0.05f;
                }
            }
        }
        else if (xDampenMovement < 1 && flipping)
        {
            xDampenMovement += 0.005f;
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
                        Backflip();
                    }
                }
                else
                {
                    canDive = true;
                    // set the initial velocity only if the player is not running into a wall
                    if (!canWallJumpLeft && _rb.velocity.x > 0 || !canWallJumpRight && _rb.velocity.x < 0)
                    {
                        xInitialVelocity = _rb.velocity.x;
                    }
                    xDampenMovement = 0.1f;
                    _rb.AddForce(Vector2.up * jumpForce);
                }
            }
            // if the player is not on the ground (but attempting to jump) check if they can walljump
            else if (canWallJumpLeft || canWallJumpRight)
            {
                WallJump();
            }
            else if (canDive)
            {
                Dive();
            }
            _shouldJump = false;
        }
    }

    IEnumerator Co_DoneRespawning()
    {
        yield return new WaitForSeconds(1f);
        respawn = false;
    }

    private void Dive()
    {
        canDive = false;
        if (_xMoveInput > 0)
        {
            // if the player is heading in one direction, yet they want to dive in another, then flip them
            if (transform.localScale.x < 0)
            {
                xInitialVelocity = -xInitialVelocity;
            }
            xDesiredMovement = 10.0f;
            xDampenMovement = 0f;
            if (_rb.velocity.y < 5)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, 0);
                _rb.AddForce(Vector2.up * (jumpForce * 0.5f));
            }
            xSpeed = xStartingSpeed;
        }
        else if (_xMoveInput < 0)
        {
            // if the player is heading in one direction, yet they want to dive in another, then flip them
            if (transform.localScale.x > 0)
            {
                xInitialVelocity = -xInitialVelocity;
            }
            xDesiredMovement = -10.0f;
            xDampenMovement = 0f;
            if (_rb.velocity.y < 5)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, 0);
                _rb.AddForce(Vector2.up * (jumpForce * 0.5f));
            }
            xSpeed = xStartingSpeed;
        }
        // if the player is not pressing a direction then send them in the direction they are already looking
        else
        {
            if (transform.localScale.x > 0)
            {
                xDesiredMovement = 10.0f;
                xDampenMovement = 0f;
                if (_rb.velocity.y < 5)
                {
                    _rb.velocity = new Vector2(_rb.velocity.x, 0);
                    _rb.AddForce(Vector2.up * (jumpForce * 0.5f));
                }
                xSpeed = xStartingSpeed;
            }
            else if (transform.localScale.x < 0)
            {
                xDesiredMovement = -10.0f;
                xDampenMovement = 0f;
                if (_rb.velocity.y < 5)
                {
                    _rb.velocity = new Vector2(_rb.velocity.x, 0);
                    _rb.AddForce(Vector2.up * (jumpForce * 0.5f));
                }
                xSpeed = xStartingSpeed;
            }
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

    private void Backflip()
    {
        flipping = true;
        if (transform.localScale.x < 0)
        {
            _rb.velocity = Vector2.zero;
            xDampenMovement = 0.0f;
            StartCoroutine(Co_BackflipDelayedMovement(true));
            _rb.AddForce(Vector2.up * (jumpForce * 1.25f));
        }
        else if (transform.localScale.x > 0)
        {
            _rb.velocity = Vector2.zero;
            xDampenMovement = 0.0f;
            StartCoroutine(Co_BackflipDelayedMovement(false));
            _rb.AddForce(Vector2.up * (jumpForce * 1.25f));
        }
    }

    IEnumerator Co_BackflipDelayedMovement(bool facingLeft)
    {
        for (int i = 1; i <= 10; i++)
        {
            yield return new WaitForSeconds(0.025f);
            if (!_isGrounded && !hitCeiling)
            {
                if (facingLeft)
                {
                    xDesiredMovement = +1.75f * ((float)i / 5);
                }
                else
                {
                    xDesiredMovement = -1.75f * ((float)i / 5);
                }
            }
            else
            {
                xDesiredMovement = 0;
                i = 10;
            }
        }
        yield return new WaitUntil(() => Vector2.Distance(new Vector2(_rb.velocity.y, 0), Vector2.zero) < 0.05f);
        yield return new WaitForSeconds(0.05f);
        flipping = false;
        canDive = true;
    }

    IEnumerator Co_EnableDiveDelayed()
    {
        yield return new WaitForSeconds(0.75f);
        canDive = true;
    }

    private void WallJump()
    {
        canDive = false;
        xInitialVelocity = 0;
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
        StartCoroutine(Co_EnableDiveDelayed());
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
        canDive = true;
    }
}
