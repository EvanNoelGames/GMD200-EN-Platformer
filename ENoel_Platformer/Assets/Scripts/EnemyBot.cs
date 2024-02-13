using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBot : MonoBehaviour
{

    [SerializeField] private LayerMask groundLayer;

    private bool touchingRightWall;
    private bool touchingLeftWall;
    private bool touchingGroundLeft;
    private bool touchingGroundRight;

    private Rigidbody2D _rb;

    public float speed = 2;

    // Start is called before the first frame update
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rb.velocity = new Vector2(transform.localScale.x * speed, 0);

        Collider2D wallColLeft = Physics2D.OverlapBox(new Vector2(transform.position.x + 0.5f, transform.position.y), new Vector2(0.4f, 0.75f), 0f, groundLayer);
        Collider2D wallColRight = Physics2D.OverlapBox(new Vector2(transform.position.x - 0.5f, transform.position.y), new Vector2(0.4f, 0.75f), 0f, groundLayer);
        Collider2D groundColLeft = Physics2D.OverlapBox(new Vector2(transform.position.x + 0.5f, transform.position.y - 0.5f), new Vector2(0.4f, 0.3f), 0f, groundLayer);
        Collider2D groundColRight = Physics2D.OverlapBox(new Vector2(transform.position.x - 0.5f, transform.position.y - 0.5f), new Vector2(0.4f, 0.3f), 0f, groundLayer);

        touchingRightWall = wallColRight != null;
        touchingLeftWall = wallColLeft != null;

        if (groundColLeft != null)
        {
            touchingGroundLeft = false;
        }
        else
        {
            touchingGroundLeft = true;
        }

        if (groundColRight != null)
        {
            touchingGroundRight = false;
        }
        else
        {
            touchingGroundRight = true;
        }

        if (touchingRightWall || touchingLeftWall || touchingGroundLeft || touchingGroundRight)
        {
            Debug.Log("flip");
            Flip();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector3(transform.position.x + 0.5f, transform.position.y, transform.position.z), new Vector2(0.4f, 0.75f));
        Gizmos.DrawWireCube(new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z), new Vector2(0.4f, 0.75f));
        Gizmos.DrawWireCube(new Vector3(transform.position.x + 0.5f, transform.position.y - 0.5f, transform.position.z), new Vector2(0.4f, 0.3f));
        Gizmos.DrawWireCube(new Vector3(transform.position.x - 0.5f, transform.position.y - 0.5f, transform.position.z), new Vector2(0.4f, 0.3f));
    }

    private void Flip()
    {
        if (touchingRightWall || !touchingGroundRight)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
        }
        else if (touchingLeftWall || !touchingGroundLeft)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
        }
    }
}
