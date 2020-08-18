using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSplat : MonoBehaviour
{
    public Transform parent;
    Animator animator;
    bool playing = false;
    bool firstFrame = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(parent != null)
        {
            if (!playing)
            {
                animator.Play("BloodSplat");
                playing = true;
            }
            transform.position = parent.position;    
        }
        if(firstFrame)
        {
            firstFrame = false;
        }
        else
        {
            if (!playing)
            {
                animator.Play("BloodSplat");
                playing = true;
            }
        }
    }
}
