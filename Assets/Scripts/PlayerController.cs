using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region //GENERAL_COMPONENTS
    private Rigidbody2D _plRb;
    private Animator _plAnimator;
    private AudioSource _audioSource;

    [SerializeField] private List<AudioClip> _audioClips;

    #endregion  
    // Start is called before the first frame update
    void Start()
    {
        _plRb = GetComponent<Rigidbody2D>();
        _plAnimator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _currentLife = _playerLife;
        _playerDefaultLayer = gameObject.layer;
        _inmuneLayerValue = LayerMask.NameToLayer("InmuneLayer");
        _newRespawnPoint = transform.position;
    }   

    #region //INPUTS_MANAGER
    private void InputsHandler()
    {
        _playerInputs = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        _jumpPressed = Input.GetButtonDown("Jump");        
        _dashPressed = Input.GetKeyDown(KeyCode.C);          
        _attackPressed = Input.GetKeyDown(KeyCode.X);   
        _plAnimator.SetFloat("Run", Mathf.Abs(_playerInputs.x));  
    } 
    #endregion 
    #region //LIFE_MANAGER
    [Header("Player Life")]
    [SerializeField] private float _playerLife = 100f;
    [Header("Inmune Params")]
    [SerializeField] private float _damageInmuneTime = 3f;
    [SerializeField] private float _respawnInmuneTime = 5f;
    private int _playerDefaultLayer;
    private int _inmuneLayerValue;
    private Vector2 _newRespawnPoint;
    private float _currentLife;
    [SerializeField]private float _freezeDamageTime = .3f;

    public Vector2 newRespawnPoint{
        set{
            _newRespawnPoint = value;
        }
    }
    public void ChangeCurrentLife(float value)
    {
        _currentLife+=value;
        Debug.Log(_currentLife);
        if(_currentLife<=0)
        {
            Respawn();
            return;
        }
        if(value <0)
        {
            _plAnimator.SetTrigger("Damage");
            _audioSource.PlayOneShot(_audioClips[0]);
            StartCoroutine(BecomeInmune(_damageInmuneTime,_inmuneLayerValue));
            StartCoroutine(FreezePosition(_freezeDamageTime));
        }            
        if(_currentLife>_playerLife)
        {
            _currentLife = _playerLife;
        }
    }
    private void Respawn()
    {
        if(GameManager.gm.currentPlayerLives >=0)
        {
            _plAnimator.SetTrigger("Damage");
            _audioSource.PlayOneShot(_audioClips[1]);
            transform.position = _newRespawnPoint;
            StartCoroutine(BecomeInmune(_respawnInmuneTime,_inmuneLayerValue));
            _currentLife = _playerLife;
        }        
        GameManager.gm.SetLivesState();
    }
    private IEnumerator BecomeInmune( float inmnuneTimer, int inmuneLayer)
    {
        gameObject.layer = inmuneLayer;
        yield return new WaitForSeconds(inmnuneTimer);
        gameObject.layer = _playerDefaultLayer;
        StopCoroutine(BecomeInmune(inmnuneTimer,inmuneLayer));
    }
    #endregion
    #region //GROUND_DETECTION
    [Header("Ground Detection")]    
    [SerializeField] private Transform _groundDetector;
    [SerializeField] private float _groundDetectorRadius = .3f;
    [SerializeField] private LayerMask _groundLayer;    //Set GroundLayer
    private bool _isGrounded;

    private void GroundDetection()
    {
        _isGrounded = Physics2D.OverlapCircle(_groundDetector.position,_groundDetectorRadius,_groundLayer);
    }
    #endregion
    #region //ONE_WAY_EFFECTOR
    [Header("OneWayPlatformDetector")]  
    [SerializeField] private LayerMask _oneWayPlatform;
    private void EffectorInterruption()
    {
        Collider2D hit = Physics2D.OverlapCircle(_groundDetector.position,_groundDetectorRadius,_oneWayPlatform);
        if(hit != null && _playerInputs.y <0)
        {
            hit.gameObject.GetComponent<OneWayPlat>().UndetectPlayerTemporary();
        }
    }
    
    #endregion
    #region //PLAYER_RUN
    [Header("Player Movement")]
    [SerializeField] private float _moveSpeed = 14f;         //Velocidad de movimiento    
    private Vector2 _playerInputs;     
    #endregion
    #region //PLAYER_FLIP

    private int _lookDirection = 1;
    private void FlipController()
    {
        if(_playerInputs.x <0){            //Si la velocidad del jugador es menor a 0 en x
            transform.eulerAngles = new Vector3(0, 180, 0);         //rotamos a la posición de 180 grados en el eje y
            _lookDirection = -1;
        }
        else if(_playerInputs.x>0){         //Si la velocidad del jugador es mayor a 0 en x
            transform.eulerAngles = Vector3.zero; 
            _lookDirection = 1;
        }                  //rotamos de nuevo a 0 grados en el eje y
    }
    #endregion
    #region //JUMP_SYSTEM
    [Header("Player Jump System")]
    [SerializeField] private float _jumpSpeed = 14f;        //Impulso de salto
    [SerializeField] private float _jumpLimitSpeed = 12f;    //Limitador de velocidad del salto
    [SerializeField] private float _fallMultiplier = 10f;    //Multiplicador de caída
    private bool _jumpPressed;
    private bool _firstJump;
    [Header("Multiple Jump")]
    [SerializeField] private float _jumpLimit = 2;      //Limite de Saltos
    [SerializeField]private float _jumpCounter = 0;     //Contador se Saltos

    private void JumpController()
    {
        if(_jumpPressed && _isGrounded)
        {
            _firstJump = true;
             ExecuteJump();                              
        }
        else if(_jumpPressed && _jumpCounter < _jumpLimit && _firstJump)       //N-Jump Detection
        {
            _firstJump = false;
            ExecuteJump();            
        } 
    }
    private void ExecuteJump()
    {
        _plRb.velocity = new Vector2(_plRb.velocity.x, _jumpSpeed);
        _jumpCounter++;
        _plAnimator.SetTrigger("Jump");
    }
    private void FallController()
    {
        if((_plRb.velocity.y < _jumpLimitSpeed || !Input.GetButton("Jump") && _plRb.velocity.y > 0)  &&!_isGrounded)
            _plRb.velocity += _fallMultiplier * Physics2D.gravity.y * Vector2.up * Time.deltaTime;
    }
    #endregion
    #region //DASH

    [Header("Dashing")]
    [SerializeField] private float _dashImpulse = 6f;
    private bool _isDashing;
    private bool _dashPressed;
    [SerializeField] private float _dashTimeLimiter = .3f;
    [SerializeField] private float _dashCooldown = .3f;

    void DashController()
    {
        if(_dashPressed && _isGrounded)
        {            
            StartCoroutine(Dash());            
        }
    }

    private IEnumerator Dash()
    {
        _isDashing = true;
        _plAnimator.SetBool("Dash",_isDashing);
        /*float defaultGravity = _plRb.gravityScale;
        _plRb.gravityScale = 0f;*/
        //_plRb.velocity = Vector2.zero;
        _plRb.velocity = new Vector2(_lookDirection * _dashImpulse * 4,_plRb.velocity.y);
        yield return new WaitForSeconds(_dashTimeLimiter);
        //_plRb.velocity = Vector2.zero;        
        //_plRb.gravityScale = defaultGravity;
        _isDashing = false;
        _plAnimator.SetBool("Dash",_isDashing);
        yield return new WaitForSeconds(_dashCooldown);
        StopCoroutine(Dash());
    }
    #endregion
    #region //MELEE_ATTACK

    [Header("Melee Attack Params")]
    [SerializeField] Transform _attackDetector;
    [SerializeField] private Vector2 _boxSize = Vector2.one;
    [SerializeField] private LayerMask _whatDamage; //Set Enemy Layer
    [SerializeField] private float _attackDuration = 0.3f;
    private bool _attackPressed;
    private bool _isAttacking;
    Collider2D[] hit;
    
    private void MeleeAttack()
    {
        if(_attackPressed)
        {
            _plAnimator.SetTrigger("Attack");              
            _audioSource.PlayOneShot(_audioClips[2]);
            StartCoroutine(FreezePosition(_attackDuration));
        }        
    }

    private void SwordAttack()
    {        
        hit = Physics2D.OverlapBoxAll(_attackDetector.position, _boxSize, 0, _whatDamage);
            if(hit!=null)
            {
                foreach(Collider2D enemy in hit)
                {
                    if(enemy.GetComponent<FlyingShooter>()!=null)
                    {
                        enemy.GetComponent<FlyingShooter>().ReceiveDamage(500);
                    }
                }                
                GameManager.gm.SetScore(hit.Length);
            }
    }

    private IEnumerator FreezePosition(float freezingTime)
    {
        _isAttacking = true;
        if(_isGrounded)
            _plRb.velocity = new Vector2(0,_plRb.velocity.y);
        yield return new WaitForSeconds(freezingTime);
        _isAttacking = false;
        StopCoroutine(FreezePosition(0));
    }
    #endregion
    // Update is called once per frame
    void Update()
    {
        if(!GameManager.gm.canPlay)
            return;
        FallController();
        if(_isDashing || _isAttacking)
            return;
        InputsHandler();     
        GroundDetection();
        JumpController(); 
        FlipController();
        DashController();    
        MeleeAttack();
        EffectorInterruption();
        if(Input.GetKeyDown(KeyCode.N))
        {
            ChangeCurrentLife(-45);
        }
    }
    private void FixedUpdate() 
    {       
        if(_isDashing || _isAttacking)
        {            
            return;
        }            
        _plRb.velocity = new Vector2(_playerInputs.x * _moveSpeed, _plRb.velocity.y);   //Movimiento en x      
    }

    private void OnDrawGizmos() {       //Dibujamos nuestro detector de suelo
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_groundDetector.position,_groundDetectorRadius); 
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(_attackDetector.position, _boxSize);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        
        if(other.gameObject.tag == "Ground")
        {
            gameObject.transform.SetParent(other.collider.gameObject.transform);
            _jumpCounter = 0;
        }            
    }
    private void OnCollisionExit2D(Collision2D other) {
        gameObject.transform.SetParent(null);        
    }
}
