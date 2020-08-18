using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 2;
    public float jumpForce = 10;
    public float dashDistance = 2;
    public float dashSpeed = 10;
    Rigidbody2D _rb;
    Vector2 _movement;
    Animator _animator;
    bool _dashing;
    BoxCollider2D _collider;
    public BoxCollider2D _dashCollider;
    bool _canDoubleJump = false;
    bool _canJump;
    bool _doubleJumped;
    Player _player;
    public LayerMask terrainLayers;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        _dashCollider.enabled = false;
        if(_rb == null)
        {
            Debug.LogWarning("Character controller is null");
        }
        _animator = GetComponentInChildren<Animator>();
        _player = GetComponent<Player>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_player.damageTaken)
        {
            MovePlayer(); 
        }
    }

    public void OnMove(InputValue value)
    {
        _movement = new Vector2(value.Get<Vector2>().x ,0 );
    }

    public void OnJump()
    {
        Jump();
    }

    public void OnDash()
    {
        if (!_dashing)
        {
            StartCoroutine(Dash()); 
        }
    }

    void MovePlayer()
    {
        if (_dashing)
            return;
        _rb.velocity = new Vector2(_movement.x * speed * Time.fixedDeltaTime, _rb.velocity.y);
        if(_rb.velocity.y == 0)
        {
            _canJump = true;
            _canDoubleJump = false;
            _doubleJumped = false;
        }
        else
        {
            _canJump = false;
            if (!_doubleJumped)
            {
                _canDoubleJump = true;  
            }
        }
        _canJump = _rb.velocity.y == 0;
        _animator.SetFloat("movement", Mathf.Abs(_movement.x));
        Flip(_movement.x);

    }

    void Jump()
    {
        if (_canJump && !_dashing)
        {
            _rb.velocity = Vector2.up * jumpForce;
            _doubleJumped = false;
        }
        else if(_canDoubleJump && !_dashing)
        {
            _rb.velocity = Vector2.up * jumpForce;
            _doubleJumped = true;
            _canDoubleJump = false;
        }
    }

    void Flip(float value)
    {
        if(value > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector2(1, 1);
        }
        else if(value < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector2(-1, 1);
        }
    }

    IEnumerator Dash()
    {
        Vector2 targetPosition = transform.position;
        if (transform.localScale.x > 0)
        {
            targetPosition += new Vector2(dashDistance, 0);
        }
        else
        {
            targetPosition -= new Vector2(dashDistance, 0);
        }
        RaycastHit2D hit = Physics2D.Raycast(_rb.position, new Vector2(transform.localScale.x, 0), dashDistance, terrainLayers);
        if (hit.transform)
        {
            targetPosition = new Vector2(hit.point.x - (_dashCollider.size.x/2 * transform.localScale.x), targetPosition.y);
        }
        _dashing = true;
        _animator.SetBool("Dashing", _dashing);
        _collider.enabled = false;
        _dashCollider.enabled = true;
        float gtemp = _rb.gravityScale;
        _rb.velocity = new Vector2(_rb.velocity.x, 0);
        _rb.gravityScale = 0;
        while(transform.position.x != targetPosition.x)
        {
            float step = dashSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, step);
            yield return new WaitForSeconds(0.002f);
        }
        _rb.gravityScale = gtemp;
        _collider.enabled = true;
        _dashCollider.enabled = false;
        _dashing = false;
        _animator.SetBool("Dashing", _dashing);
    }
}
