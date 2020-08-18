using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int maxHealth = 200;
    public bool damageTaken;
    int _currentHealth;
    public int invulnerabilityTime = 2;
    bool invulnerable;
    float _canTakeDamage;
    public float knockbackX = 1;
    public float knockbackY = 10;
    Animator _anim;
    Rigidbody2D _rb;
    public BoxCollider2D dashCollider;
    public BoxCollider2D playerCollider;
    Color _original;
    SpriteRenderer _spriteRenderer;
    bool flashing;

    private void Awake()
    {
        _currentHealth = maxHealth;
        _anim = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _original = _spriteRenderer.color;
    }

    public void TakeDamage(int value)
    {
        if (invulnerable)
            return;
        _canTakeDamage = Time.time + invulnerabilityTime;
        _currentHealth -= value;
        damageTaken = true;
        invulnerable = true;
        if (!flashing)
        {
            StartCoroutine(WhiteFlash()); 
        }
        StartCoroutine(UIManager.instance.UpdateHealthHUD(_currentHealth, _currentHealth + value));
        _anim.SetTrigger("GetHit");
        _rb.velocity = new Vector2(knockbackX * -transform.localScale.x, knockbackY);
        if (_currentHealth <= 0)
            print("Is Die");
    }

    private void FixedUpdate()
    {
        if(_rb.velocity.y == 0 && damageTaken)
        {
            damageTaken = false;
        }
    }

    private void Update()
    {
        if (invulnerable)
        {
            CheckInvulnerable();
        }
    }

    void CheckInvulnerable()
    {
        if(Time.time > _canTakeDamage)
        {
            invulnerable = false;
        }
        else
        {
            invulnerable = true;
        }
    }

    IEnumerator WhiteFlash()
    {
        flashing = true;
        Color e = new Color(0, 0, 0, 0);
        while(invulnerable)
        {
            if(_spriteRenderer.color == e)
            {
                _spriteRenderer.color = _original;
            }
            else
            {
                _spriteRenderer.color = e;
            }
            yield return new WaitForSeconds(.15f);
        }
        flashing = false;
        _spriteRenderer.color = _original;
    }

    public int GetCurrentHealth()
    {
        return _currentHealth;
    }
}
