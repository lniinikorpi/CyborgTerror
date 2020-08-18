using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationReceiver : MonoBehaviour
{
    Enemy _enemy;
    // Start is called before the first frame update
    void Start()
    {
        _enemy = GetComponentInParent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AttackHitScan()
    {
        _enemy.AttackHitScan();
    }
}
