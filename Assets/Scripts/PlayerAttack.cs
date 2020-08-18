using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    Animator _animator;
    public float attackRate = 2;
    public float comboMaxWait = 1;
    public float comboMinWait = 0.2f;
    public float comboDamageMultiplier = 1.2f;
    public float hitRadius = 0.5f;
    public float maxDamage = 17;
    public float minDamage = 13;
    public float criticalChance = 25;
    public float criticalMultiplier = 2;
    public Transform hitLocation;
    float _canComboMax;
    float _canComboMin;
    public int maxCombo = 2;
    int currentCombo;
    float _canAttack = 0;
    public LayerMask enemyLayers;
    public LayerMask hitScanLayers;
    Player _player;

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnAttack()
    {
        Attack();
    }

    void Attack()
    {
        if(currentCombo != 0)
        {
            if (currentCombo < maxCombo)
            {
                if (Time.time >= _canComboMin && Time.time <= _canComboMax)
                {
                    currentCombo++;
                    _canComboMax = Time.time + comboMaxWait;
                    _canComboMin = Time.time + comboMinWait;
                    _canAttack = Time.time + 1 / attackRate;
                    _animator.SetBool("Played", false);
                }
                else if(Time.time >= _canComboMin)
                {
                    currentCombo = 0;
                } 
            }
            else
            {
                currentCombo = 0;
            }
        }
        if (Time.time >= _canAttack && currentCombo == 0)
        {
            _animator.SetTrigger("Attack");
            _canAttack = Time.time + 1 / attackRate;
            _canComboMax = Time.time + comboMaxWait;
            _canComboMin = Time.time + comboMinWait;
            currentCombo++;
        }
        _animator.SetInteger("Combo", currentCombo);
    }

    public void SwordHitScan()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(hitLocation.position, hitRadius, enemyLayers);
        foreach (Collider2D enemy in enemies)
        {
            int damage = (int)Random.Range(minDamage, maxDamage);
            bool ifCrit = Random.Range(0, 100) < criticalChance;
            float critM = 1;
            if (ifCrit)
                critM = criticalMultiplier;
            damage = Mathf.RoundToInt(damage * critM * Mathf.Pow(comboDamageMultiplier, currentCombo));

            RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(transform.localScale.x, 0), Mathf.Infinity, hitScanLayers);
            if (hit.transform == enemy.transform)
            {
                enemy.GetComponent<Enemy>().TakeDamage(damage, ifCrit);
            }
        }
    }

    public void SetPlayed()
    {
        _animator.SetBool("Played", true);
    }

    private void OnDrawGizmosSelected()
    {
        if(hitLocation == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(hitLocation.position, hitRadius);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            _player.TakeDamage(other.GetComponent<Enemy>().damage);
        }
    }
}
