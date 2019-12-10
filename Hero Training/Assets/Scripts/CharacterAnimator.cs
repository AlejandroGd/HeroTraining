using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void ChangeToIdle()
    {
        if (anim != null) anim.Play("Idle");
    }

    public void PlayAttack()
    {
        if (anim != null) anim.Play("Attack");
    }

    public void PlayHurt()
    {
        if (anim != null) anim.Play("Hurt");
    }

    public void PlayBuff()
    {
        if (anim != null) anim.Play("Buff");
    }

    public void PlayDead()
    {
        if (anim != null) anim.Play("Die");
    }
}