using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBullet : MonoBehaviour
{
    [Header("Player Damage")]
    [SerializeField] private float _playerDamage = -12f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other) {
        GameObject collided = other.gameObject;
        if(collided.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().ChangeCurrentLife(_playerDamage);           
        }
        if(collided.tag != "Enemy")
        {
            Destroy(gameObject);
        }
    }
}
