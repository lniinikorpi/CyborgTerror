using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationReceiver : MonoBehaviour
{
    PlayerAttack _pa;
    private void Awake()
    {
        _pa = GetComponentInParent<PlayerAttack>();
    }

    public void SwordHitScan()
    {
        _pa.SwordHitScan();
    }

    public void SetPlayed()
    {
        _pa.SetPlayed();
    }
}
