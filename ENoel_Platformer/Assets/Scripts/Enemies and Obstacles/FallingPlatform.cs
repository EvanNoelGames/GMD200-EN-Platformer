using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    private Vector3 startingPosition;
    public float timeBeforeRespawning = 6f;
    public float timeBeforeFalling = 1f;
    public float fallSpeed = 1f;

    private bool falling = false;

    private Rigidbody2D _rb;

    void Start()
    {
        Physics2D.IgnoreLayerCollision(0, 0);
        _rb = GetComponent<Rigidbody2D>();
        startingPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (falling)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, -fallSpeed);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.transform.position.y > transform.position.y)
            {
                StartCoroutine(Co_Fall());
            }
        }
    }

    IEnumerator Co_Respawn()
    {
        yield return new WaitForSeconds(timeBeforeRespawning);
        falling = false;
        transform.position = startingPosition;
        _rb.gravityScale = 0f;
        _rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    IEnumerator Co_Fall()
    {
        yield return new WaitForSeconds(timeBeforeFalling);
        falling = true;
        _rb.gravityScale = 0.1f;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        StartCoroutine(Co_Respawn());
    }
}
