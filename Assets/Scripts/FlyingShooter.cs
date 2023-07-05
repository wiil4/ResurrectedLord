using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingShooter : MonoBehaviour
{
    [Header("Life Params")]
    [SerializeField] private float _startLife =100f;
    [SerializeField] private GameObject _particleED;

    [Header("Movement Params")]
    [SerializeField] private float _speed = 2f;
    [SerializeField] private float _detectionDistance = 8f;
    [SerializeField] private float _attackDistance = 5f;

    [Header("Attack Params")]
    [SerializeField] private float _timeBetweenAttack = 2f;
    [SerializeField] private GameObject _bulletReference;
    [SerializeField] private float _bulletSpeed = 8f;
    [SerializeField] private float _destroyBulletAfter = 5f;
    [SerializeField] private float _attacktimer;
    private Transform _playerTransform;
    private Animator _animator;
    private AudioSource _audioSource;
    private Collider2D _coll;
    [SerializeField] private List<AudioClip> _audioClips;

    // Start is called before the first frame update
    void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _coll = GetComponent<Collider2D>();
        _attacktimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.gm.canPlay)
            return;
        if(_playerTransform == null)
        {
            return;
        }
        if(_startLife<=0)
        {
            return;
        }
        float currentDistance = Vector2.Distance(transform.position,_playerTransform.position);
        if(currentDistance < _detectionDistance && currentDistance > _attackDistance)
        {
            PlayerChase();            
        }
        else if(currentDistance < _attackDistance)
        {
            Attack();
        }        
    }

    private void PlayerChase()
    {
        transform.position = Vector2.MoveTowards(transform.position,_playerTransform.position,_speed * Time.deltaTime);
        _attacktimer = 0;
        if(transform.position.x > _playerTransform.position.x)
        {
            transform.eulerAngles = new Vector3(0,180,0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0,0,0);
        }
    }

    private void Attack()
    {
        if(_attacktimer < _timeBetweenAttack)
        {
            _attacktimer += Time.deltaTime;            
        }
        else
        {
            _animator.SetTrigger("Shoot");            
            _attacktimer = 0;
        }
    }

    private void ShootBullet()
    {
        _audioSource.PlayOneShot(_audioClips[1]);
        GameObject bulletRef = Instantiate(_bulletReference, transform.position,Quaternion.identity);                        
        Vector2 direction = _playerTransform.position - transform.position;
        bulletRef.GetComponent<Rigidbody2D>().velocity = direction.normalized * _bulletSpeed;
        Destroy(bulletRef,_destroyBulletAfter);
    }

    public void ReceiveDamage(float damage)
    {
        _audioSource.PlayOneShot(_audioClips[0]);
        _animator.SetTrigger("Dead");
        _startLife -= damage;
        if(_startLife<=0)
        {
            _coll.enabled=false;
            GameObject p = Instantiate(_particleED,transform.position,Quaternion.identity);
            Destroy(p,2f);
            Destroy(gameObject,1f);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position,_detectionDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,_attackDistance);
    }
}
