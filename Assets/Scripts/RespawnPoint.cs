using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("newRespawnPoint");
            other.gameObject.GetComponent<PlayerController>().newRespawnPoint = transform.position;
        }
    }
}
