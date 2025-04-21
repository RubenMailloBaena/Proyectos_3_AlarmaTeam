using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerController))]
public class PlayerHealthController : MonoBehaviour
{
    private PlayerController pController;
    
    [SerializeField] private float secondsThatCanTakeDamage = 5f;
    [SerializeField] private float cooldownToHealPlayer = 2f;
    [SerializeField] private float maxHurtAlpha = 0.5f;
    private float currentHealth;
    private float timeTillLastDamage = 0.0f;
    private bool isGodMode;

    private void Awake()
    {
        pController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        currentHealth = secondsThatCanTakeDamage;
    }

    void Update()
    {
        if (pController.IsPlayerDead)
        {
            SetAlphaToMax();
            return;
        }
        HealPlayer();
        UpdateHUD();
        print(currentHealth);
    }
    
    public void TakeDamage(float damage)
    {
        if (isGodMode) return;
        
        currentHealth -= damage * Time.deltaTime;
        timeTillLastDamage = cooldownToHealPlayer;

        if (currentHealth <= 0.0f)
        {
            pController.SetIsPlayerDead(true);
            pController.ShowGameOverHUD(true);
            GameManager.GetInstance().PlayerDead();
        }
    }
    

    private void HealPlayer()
    {
        if (timeTillLastDamage <= 0.0f)
        {
            currentHealth += Time.deltaTime;
            currentHealth = Mathf.Clamp(currentHealth, 0, secondsThatCanTakeDamage);
        }
        else
            timeTillLastDamage -= Time.deltaTime;
    }

    private void UpdateHUD()
    {
        float normalizedHealth = Mathf.Clamp01(currentHealth / secondsThatCanTakeDamage);
        float alpha = (1f - normalizedHealth) * maxHurtAlpha;

        Color newColor = pController.GetHurtVisualColor();
        newColor.a = alpha;
        pController.SetHurtVisualColor(newColor);
    }

    private void SetAlphaToMax()
    {
        Color newColor = pController.GetHurtVisualColor();
        newColor.a = maxHurtAlpha;
        pController.SetHurtVisualColor(newColor);
    }

    public void Respawn()
    {
        pController.ShowGameOverHUD(false);
        pController.SetIsPlayerDead(false);
        currentHealth = secondsThatCanTakeDamage;
        UpdateHUD();
    }

    private void GodMode()
    {
        isGodMode = !isGodMode;
        Debug.Log("GodMode: " + isGodMode);
    }

    private void OnEnable()
    {
        pController.OnTakeDamage += TakeDamage;
        pController.onRestart += Respawn;
        pController.onGodMode += GodMode;
    }

    private void OnDisable()
    {
        pController.OnTakeDamage -= TakeDamage;
        pController.onRestart -= Respawn;
        pController.onGodMode -= GodMode;
    }
}
