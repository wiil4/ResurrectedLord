using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaGroundEnemy : MonoBehaviour
{
    [Header("Player Damage")]
    [SerializeField] private float _playerDamage = -15f;
    [Header("MovementParameters")]
    [SerializeField] private float _speed = 8f;
    [SerializeField] private float _attackDistance = 1f;
    private bool _isGrounded;

    [Header("Attack Parameters")]
    [SerializeField] private float _timeBetweenAttack = 2f;
    [SerializeField] private LayerMask _playerLayer;            //Set Player Layer
    [SerializeField] private Transform _playerDetectorOrigin;
    [SerializeField] private Vector2 _boxSize = Vector2.one;
    [SerializeField]private float _attacktimer;
    private bool _isPlayerInsideDetector;
    public void SetChasePlayer(bool state) => _isPlayerInsideDetector = state;  //Bool Setted By ChaseAreaController.cs
    private Transform _playerTransform;

    Rigidbody2D _gEnemyRb;
    Animator _gEAnimator;

    // Start is called before the first frame update
    void Start()
    {
        _gEnemyRb = GetComponent<Rigidbody2D>();
        _gEAnimator = GetComponent<Animator>();
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
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
        if(_isPlayerInsideDetector)
        {
            float currentDistance = Vector2.Distance(transform.position,_playerTransform.position);
            if(currentDistance > _attackDistance)
            {
                PlayerChase();   
            }
            else
            {
                Attack();
            }
            
        }       
        else if(_isGrounded)
        {
            _gEnemyRb.velocity = Vector2.zero;
        }
        _gEAnimator.SetFloat("Walk", Mathf.Abs (_gEnemyRb.velocity.x));       
        
    }

    private void PlayerChase()
    { 
        float distance = _playerTransform.position.x - transform.position.x;
        _gEnemyRb.velocity = new Vector2(_speed * Mathf.Sign(distance),_gEnemyRb.velocity.y);
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
            _gEnemyRb.velocity = Vector2.zero;                       
        }
        else
        {            
            _attacktimer = 0;
            _gEAnimator.SetTrigger("Attack");
        }
    }

    private void SwordPointAnimation()
    {
        Collider2D hit = Physics2D.OverlapBox(_playerDetectorOrigin.position,_boxSize,0,_playerLayer);
        if(hit!=null)
        {                   
            GameObject hitted = hit.gameObject;
            if(hitted.tag == "Player")
            {
                hitted.gameObject.GetComponent<PlayerController>().ChangeCurrentLife(_playerDamage);        
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Ground")
        {
            _isGrounded = true;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(_playerDetectorOrigin.position, _boxSize);
    }
}
