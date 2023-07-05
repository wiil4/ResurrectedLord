using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject _whatToSpawn;
    [SerializeField] Vector2 _timeLimiter;
    [SerializeField] private float _randomSpawnTime;
    private float _randomTimer;


    // Start is called before the first frame update
    void Start()
    {
        _randomSpawnTime = Random.Range(_timeLimiter.x, _timeLimiter.y);
    }

    // Update is called once per frame
    void Update()
    {
        if(_randomTimer < _randomSpawnTime)
        {
            _randomTimer += Time.deltaTime;
        }
        else
        {
            Spawn();
        }        
    }

    private void Spawn()
    {
        Instantiate(_whatToSpawn,transform.position,Quaternion.identity);
        _randomSpawnTime = Random.Range(_timeLimiter.x, _timeLimiter.y);
        _randomTimer = 0;
    }
}
