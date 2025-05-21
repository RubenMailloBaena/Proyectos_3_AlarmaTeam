using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationType
{
    Idle, 
    Walk,
    Run,
    Attack,
    Dead,
    Charm
}

public class EnemyAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private float idleSpeed = 0.0f;
    private float walkSpeed = 0.5f;
    private float runSpeed = 1f;

    private void Awake()
    {
        SetAnimation(AnimationType.Idle, false);
    }

    public void SetAnimation(AnimationType animationType, bool setBool)
    {
        animator.SetBool("Dead",false);
        animator.SetBool("Attack", false);
        
        switch (animationType)
        {
            case AnimationType.Idle: 
                SetBlendTreeSpeed(idleSpeed);
                break;
            
            case AnimationType.Walk:
                SetBlendTreeSpeed(walkSpeed);
                break;
            
            case AnimationType.Run:
                SetBlendTreeSpeed(runSpeed);
                break;
            
            case AnimationType.Attack:
                animator.SetBool("Attack", setBool);
                break;
            
            case AnimationType.Dead:
                animator.SetBool("Dead", setBool);
                break;
            
            case AnimationType.Charm:
                animator.SetBool("Charmed", setBool);
                break;
        }
    }

    private void SetBlendTreeSpeed(float speed)
    {
        //QUIZAS SE PUEDE AÑADIR UN LERP RAPIDO PARA QUE SEA MAS SUAVE
        animator.SetFloat("Speed", speed);
    }
}
