using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerManager player = other.GetComponent<PlayerManager>();
            player.AddStar();
            gameObject.SetActive(false);
            player.StarSound();
        }
    }
}
