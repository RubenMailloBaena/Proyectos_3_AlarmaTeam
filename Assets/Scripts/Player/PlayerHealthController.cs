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
    [SerializeField] private RawImage playerHurt;
    [SerializeField] private float maxHurtAlpha = 0.5f;
    private float currentHealth;
    private float timeTillLastDamage = 0.0f;
    
    
    private void Awake()
    {
        pController = GetComponent<PlayerController>();
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
    }
    
    public void TakeDamage()
    {
        currentHealth -= Time.deltaTime;
        timeTillLastDamage = cooldownToHealPlayer;

        if(currentHealth <= 0.0f)
            pController.SetIsPlayerDead(true);
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

        Color newColor = playerHurt.color;
        newColor.a = alpha;
        playerHurt.color = newColor;
    }

    private void SetAlphaToMax()
    {
        print("HERE");
        Color newColor = playerHurt.color;
        newColor.a = maxHurtAlpha;
        playerHurt.color = newColor;
    }

    private void OnEnable()
    {
        pController.OnTakeDamage += TakeDamage;
    }

    private void OnDisable()
    {
        pController.OnTakeDamage -= TakeDamage;
    }
}
