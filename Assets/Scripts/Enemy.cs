using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Pathfinding;
using UnityEngine.Animations;
using System.Security.Cryptography;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;
    public int damage = 10;
    [Tooltip("Attacks per second")]
    public float attackRate = 1;
    int _currentHealth;
    Animator _animator;
    public GameObject damageCanvas;
    float _canAttack;
    public GameObject bloodSplat;
    public LayerMask playerLayers;
    BoxCollider2D _boxCollider;

    [Header("AI")]
    public int speed = 100;
    public Transform target;
    Rigidbody2D _rb;
    bool _playerInRange;
    public Transform scanCirclePosition;
    public float scanRadius = 0.2f;
    public float stopDistance = 1f;
    int moveDirection = 1;
    Player _player;
    public LayerMask hitScanLayers;
    public LayerMask terrainLayers;
    public LayerMask sightLayers;
    public Transform raycastPointDown;
    bool followPlayer;
    public int sigthDistance = 5;
    public int timeToLosePlayer = 5;
    float _canLose;
    Vector2 wanderDestination;
    bool wanderFinished = true;
    public int wanderWaitMax = 10;
    public int wanderWaitMin = 2;
    float _canWander;
    Vector2 moveDestination;

    void Start()
    {
        _currentHealth = maxHealth;
        _animator = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _canAttack = 1 / attackRate;
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    void FixedUpdate()
    {
        if (!_playerInRange)
        {
            Move(); 
        }
        else
        {
            Attack();
        }
        ScanPlayerToAttack();
        ScanPlayer();
    }

    private void Move()
    {
        if (followPlayer)
        {
            FollowPlayer();
        }
        else
        {
            Wander();
        }
    }

    void FollowPlayer()
    {
        Vector2 direction = ((Vector2)_player.transform.position - _rb.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(raycastPointDown.position, Vector2.down, .5f, terrainLayers);
        Vector2 destination = new Vector2(_player.transform.position.x - _player.GetComponent<BoxCollider2D>().size.x/2 * transform.localScale.x, _rb.position.y);
        if (!hit.transform)
        {
            destination = _rb.position;
        }
        if (Mathf.Abs(_rb.position.x - _player.transform.position.x) >= stopDistance)
        {
            _playerInRange = true;
            MoveEnemy(direction, destination); 
        }
        else
        {
            _playerInRange = false;
        }
    }
    void Wander()
    {
        if (Time.time < _canWander)
            return;
        if (wanderFinished)
        {
            int i = Random.Range(0, 2);
            int dir = 1;
            if (i == 0)
            {
                dir = -1;
            }
            wanderDestination = new Vector2(_rb.position.x + Random.Range(1, 5) * dir, _rb.position.y);
            RaycastHit2D hit = Physics2D.Raycast(_rb.position, new Vector2(transform.localScale.x, 0), Vector2.Distance(_rb.position, wanderDestination), terrainLayers);
            if(hit.transform)
            {
                wanderDestination = new Vector2(hit.point.x - (_boxCollider.size.x/2 * transform.localScale.x), hit.point.y);
            }
            wanderFinished = false;
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(raycastPointDown.position, Vector2.down, .5f, terrainLayers);
            if (!hit.transform)
            {
                if ((transform.localScale.x > 0 && wanderDestination.x > _rb.position.x) || (transform.localScale.x < 0 && wanderDestination.x < _rb.position.x))
                {
                    wanderFinished = true;
                    wanderDestination = _rb.position;
                    _canWander = Time.time + Random.Range(wanderWaitMin, wanderWaitMax + 1);
                }
            }
            Vector2 direction = (wanderDestination - _rb.position).normalized;
            wanderDestination = new Vector2(wanderDestination.x, _rb.position.y);
            MoveEnemy(direction, wanderDestination);
            if ((_rb.position.x >= wanderDestination.x - .1f && direction.x > 0) || (_rb.position.x <= wanderDestination.x + .1f && direction.x < 0))
            {
                wanderFinished = true;
                _canWander = Time.time + Random.Range(wanderWaitMin, wanderWaitMax + 1);
            }
        }
    }

    private void MoveEnemy(Vector2 direction, Vector2 destination)
    {
        moveDestination = destination;
        
        if (direction.x < 0)
        {
            transform.localScale = new Vector2(-1, 1);
            moveDirection = -1;
        }
        if (direction.x > 0)
        {
            transform.localScale = new Vector2(1, 1);
            moveDirection = 1;
        }
        if (_rb.velocity.y == 0)
        {
            transform.position = Vector2.MoveTowards(_rb.position, destination, Time.deltaTime * speed);
            _animator.SetFloat("Movement", Mathf.Abs(destination.x - _rb.position.x));
        }
    }


    void Attack()
    {
        _animator.SetFloat("Movement", 0);
        int attackIndex = Random.Range(0, 2);
        _animator.SetInteger("AttackType", attackIndex);
        if(Time.time >= _canAttack)
        {
            _animator.SetTrigger("Attack");
            _canAttack = Time.time + 1 / attackRate;
        }
    }

    public void AttackHitScan()
    {
        Collider2D[] players = Physics2D.OverlapCircleAll(scanCirclePosition.position, scanRadius, playerLayers);
        foreach (Collider2D player in players)
        {
            RaycastHit2D hit = Physics2D.Raycast(_rb.position, new Vector2(transform.localScale.x, 0), Mathf.Infinity, hitScanLayers);
            if(hit.transform == _player.transform) {
                _player.TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(int value, bool ifCrit)
    {
        followPlayer = true;
        GameObject g = Instantiate(damageCanvas, transform.position, Quaternion.identity);
        g.GetComponentInChildren<TMP_Text>().text = "-" + value.ToString();
        if (ifCrit)
            g.GetComponentInChildren<TMP_Text>().color = Color.yellow;

        _currentHealth -= value;
        GameObject b = Instantiate(bloodSplat, transform.position, Quaternion.identity);
        b.GetComponent<BloodSplat>().parent = transform;

        Destroy(b, .5f);
        if (_currentHealth <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            _animator.SetTrigger("GetHit");
        }
    }

    void ScanPlayerToAttack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(scanCirclePosition.position, scanRadius);
        bool playerFound = false;

        foreach (Collider2D collider in colliders)
        {
            if(collider.CompareTag("Player"))
            {
                playerFound = true;
                break;
            }
        }
        if(playerFound)
        {
            if(_playerInRange == false)
            {
                _canAttack = Time.time + 1 / attackRate;
                _playerInRange = true;
            }
        }
        else
        {
            if (_playerInRange == true)
            {
                _playerInRange = false;
            }
        }

    }

    void ScanPlayer()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(transform.localScale.x, 0), sigthDistance, sightLayers);
        if(hit.transform == _player.transform)
        {
            followPlayer = true;
            _canLose = Time.time + timeToLosePlayer;
        }
        else
        {
            if(Time.time >= _canLose)
            {
                followPlayer = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (scanCirclePosition == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(scanCirclePosition.position, scanRadius);
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + 5 * transform.localScale.x, transform.position.y));
        Gizmos.DrawWireSphere(moveDestination, .2f);
    }
}
