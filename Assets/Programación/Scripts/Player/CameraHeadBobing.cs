using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HeadBobSpeed
{
    Walk,
    Leaning,
    Running
}

public class CameraHeadBobing : MonoBehaviour
{
    [Header("Lean")] 
    [Range(0.001f, 0.01f)] 
    [SerializeField] private float leanAmount = 0.002f;
        
    [Range(1f, 30f)]
    [SerializeField] private float  leanFrequency= 10f;
    
    [Range(10f, 100f)]
    [SerializeField] private float leanSmooth = 10f;
    
    [Header("Walk")] 
    [Range(0.001f, 0.01f)] 
    [SerializeField] private float walkAmount = 0.002f;
        
    [Range(1f, 30f)]
    [SerializeField] private float  walkFrequency= 10f;
    
    [Range(10f, 100f)]
    [SerializeField] private float walkSmooth = 10f;
        
    [Header("Running")] 
    [Range(0.001f, 0.01f)]
    [SerializeField] private float runAmount = 0.002f;
    
    [Range(1f, 30f)]
    [SerializeField] private float  runFrequency= 10f;
    
    [Range(10f, 100f)]
    [SerializeField] private float runSmooth = 10f;

    public float returnSpeed = 2f;
    
    private float amount = 0.002f;
    private float  frequency= 10f;
    private float smooth = 10f;
    
    private Vector3 startPosition;
    private bool headBob = false;

    private PlayerController pController;
    
    
    private void Start()
    {
        pController = GameManager.GetInstance().GetPlayerController();
        startPosition = transform.localPosition;
    }

    public void Stop() => headBob = false;

    public void BobHead (HeadBobSpeed type)
    {
        headBob = true;
        SetAttributes(type);
    }

    private void Update()
    {
        if (headBob && !pController.IsIdle  && !pController.IsTeleporting && !pController.IsPlayerDead && !pController.IsVaulting)
        {
            StartHeadBobing();
        }
        else
            StopHeadBobing();
    }

    private void StartHeadBobing()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Lerp(pos.y, Mathf.Sin(Time.time * frequency) * amount * 1.4f, smooth * Time.deltaTime);
        pos.x += Mathf.Lerp(pos.x, Mathf.Cos(Time.time * frequency / 2f) * amount * 1.6f, smooth * Time.deltaTime);
        transform.localPosition += pos;
    }

    private void StopHeadBobing()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, startPosition, returnSpeed * Time.deltaTime);
    }
    
    private void SetAttributes(HeadBobSpeed type)
    {

        switch (type)
        {
            case HeadBobSpeed.Walk:
                amount = walkAmount;
                frequency = walkFrequency;
                smooth = walkSmooth;
                break;
            
            case HeadBobSpeed.Running:
                amount = runAmount;
                frequency = runFrequency;
                smooth = runSmooth;
                break;
            
            default:
                amount = leanAmount;
                frequency = leanFrequency;
                smooth = leanSmooth;
                break;
        }
        
    }
}
