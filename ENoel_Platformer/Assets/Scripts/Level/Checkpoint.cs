using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int checkpointNumber;
    public int CheckpointNumber => checkpointNumber;
    [SerializeField] private bool startingPoint;

    private void Awake()
    {
        if (startingPoint)
        {
            EnableCheckpoint();
        }
        else
        {
            DisableCheckpoint();
        }
    }

    public void EnableCheckpoint()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
    }

    public void DisableCheckpoint()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.cyan;
    }
}
