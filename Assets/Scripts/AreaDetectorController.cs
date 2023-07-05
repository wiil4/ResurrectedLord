using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDetectorController : MonoBehaviour
{
    private bool _playerChase = false;
    [SerializeField] private List<AreaGroundEnemy> _thisAreaEnemies;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player")
        {
            foreach(AreaGroundEnemy enemy in _thisAreaEnemies)
            {
                _playerChase = true;
                enemy.SetChasePlayer(_playerChase);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.tag == "Player")
        {
            foreach(AreaGroundEnemy enemy in _thisAreaEnemies)
            {
                _playerChase = false;
                enemy.SetChasePlayer(_playerChase);
            }
        }
    }
}
