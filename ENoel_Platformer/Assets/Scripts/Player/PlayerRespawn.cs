using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private Checkpoint respawn;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hazard"))
        {
            transform.position = new Vector3(respawn.transform.position.x, respawn.transform.position.y, transform.position.z);
        }

        else if (other.CompareTag("Checkpoint"))
        {
            Checkpoint checkpoint = other.GetComponent<Checkpoint>();
            if (checkpoint.CheckpointNumber > respawn.CheckpointNumber)
            {
                respawn.DisableCheckpoint();
                respawn = checkpoint;
                checkpoint.EnableCheckpoint();
            }
        }
    }
}
