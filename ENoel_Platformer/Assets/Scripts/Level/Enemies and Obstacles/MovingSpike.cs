using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSpike : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private Transform[] movePoints;

    private int _pointIndex = 0;

    private Transform _currentPoint;

    // Start is called before the first frame update
    void Start()
    {
        _currentPoint = movePoints[_pointIndex];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(new Vector3(0, 0, -moveSpeed));
        transform.position = Vector2.MoveTowards(transform.position, _currentPoint.position, moveSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, _currentPoint.position) < 0.01f)
        {
            _pointIndex++;
            _pointIndex %= movePoints.Length;
            _currentPoint = movePoints[_pointIndex];
        }
    }
}
