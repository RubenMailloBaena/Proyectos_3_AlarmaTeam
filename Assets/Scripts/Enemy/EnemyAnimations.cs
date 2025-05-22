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
    Charm,
    CharmWalk
}

public class EnemyAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private float idleSpeed = 0.0f;
    private float walkSpeed = 0.5f;
    private float runSpeed = 1f;

    private float currentBlendSpeed = 0f;
    private float targetBlendSpeed = 0f;
    [SerializeField] private float speedLerpRate = 5f;

    private void Awake()
    {
        SetAnimation(AnimationType.Idle, false);
    }

    private void Update()
    {
        // Lerp para suavizar la transición entre velocidades
        currentBlendSpeed = Mathf.Lerp(currentBlendSpeed, targetBlendSpeed, Time.deltaTime * speedLerpRate);
        animator.SetFloat("Speed", currentBlendSpeed);
    }

    public void SetAnimation(AnimationType soundType, bool setBool)
    {
        animator.SetBool("Dead", false);
        animator.SetBool("Attack", false);

        switch (soundType)
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
            
            case AnimationType.CharmWalk:
                animator.SetTrigger("CharmWalk");
                break;
        }
    }

    private void SetBlendTreeSpeed(float speed)
    {
        targetBlendSpeed = speed;
    }
    
    public void StopAnimation(bool stop)
    {
        if (stop)
            animator.speed = 0.0f;
        else
            animator.speed = 1f;
    }
}
