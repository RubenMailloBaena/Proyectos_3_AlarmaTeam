using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySeenIndicator : MonoBehaviour, ISeeArrow
{
    [Header("References")] 
    [SerializeField] private RectTransform pointArrow;
    [SerializeField] private RectTransform seenBarFollowPosition;
    [SerializeField] private RectTransform seenFillBarParent;
    [SerializeField] private Image pointerImage;
    [SerializeField] private Image fillImage;
    [SerializeField] private Image backFillImage;
    
    [Header("Sprites")]
    [SerializeField] private Sprite interrogantSprite; 
    [SerializeField] private Sprite exclamationSprite;

    [Header("Target")]
    public Transform targetPosition;

    [Header("Settings")]
    [SerializeField] private float waitTimeToDisspear = 3f;
    private float screenEdgeBuffer;
    [Tooltip("Porcentage del borde de la pantalla")]
    [SerializeField] private float screenEdgePercentage = 0.05f;
    [SerializeField] private float smoothPositionSpeed = 15f;

    public bool IsActive { get; set; }
    private Coroutine hideExclamationCorutine;

    public void SetTarget(Transform target)
    {
        if (hideExclamationCorutine != null)
        {
            StopCoroutine(hideExclamationCorutine);
            hideExclamationCorutine = null;
            SetActive(false);
        }
        
        targetPosition = target;
        fillImage.enabled = true;
        fillImage.fillAmount = 0.0f;
        backFillImage.color = Color.white;
        backFillImage.sprite = interrogantSprite;
    }

    private void Update()
    {
        UpdateArrowPosition();
    }

    private void UpdateArrowPosition()
    {
        screenEdgeBuffer = Mathf.Min(Screen.width, Screen.height) * screenEdgePercentage;
        
        Vector3 targetScreenPoint = Camera.main.WorldToScreenPoint(targetPosition.position);
        bool isOffscreen = targetScreenPoint.z <= 0 ||
                           targetScreenPoint.x <= screenEdgeBuffer || targetScreenPoint.x >= Screen.width - screenEdgeBuffer ||
                           targetScreenPoint.y <= screenEdgeBuffer || targetScreenPoint.y >= Screen.height - screenEdgeBuffer;

        if (!isOffscreen)
        {
            //pointArrow.position = Vector3.Lerp(pointArrow.position, targetScreenPoint, smoothPositionSpeed * Time.deltaTime);
            pointArrow.position = targetScreenPoint;
            pointArrow.localEulerAngles = Vector3.zero;
        }
        else
        {
            //mover la flecha
            Vector2 clampedPosition = GetClampedScreenPosition(targetScreenPoint);
            //pointArrow.position = Vector2.Lerp(pointArrow.position, clampedPosition, smoothPositionSpeed * Time.deltaTime);
            pointArrow.position = clampedPosition;

            // Rotar la flecha
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Vector2 screenTarget = GetCorrectedScreenPoint(targetScreenPoint);
            Vector2 direction = (screenTarget - screenCenter).normalized;

            float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            pointArrow.localEulerAngles = new Vector3(0, 0, -targetAngle);
        }
        
        //LA BARRA SIEMPRE SIGUE A LA FLECHA
        
        ScreenVisualSettings(isOffscreen);
    }

    private void ScreenVisualSettings(bool isOffScreen)
    {
        if (isOffScreen)//Fuera de la pantalla
        {
            pointerImage.enabled = true;
            seenFillBarParent.position = seenBarFollowPosition.position;
        }
        else //dentro de la pantalla
        {
            pointerImage.enabled = false;
            seenFillBarParent.position = pointArrow.position;
        }
    }

    private Vector2 GetClampedScreenPosition(Vector3 screenPoint)
    {
        Vector2 corrected = GetCorrectedScreenPoint(screenPoint);
        return new Vector2(
            Mathf.Clamp(corrected.x, screenEdgeBuffer, Screen.width - screenEdgeBuffer),
            Mathf.Clamp(corrected.y, screenEdgeBuffer, Screen.height - screenEdgeBuffer)
        );
    }

    private Vector2 GetCorrectedScreenPoint(Vector3 screenPoint)
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

        Vector3 dirToTarget = (targetPosition.position - Camera.main.transform.position).normalized;
        Vector3 camRight = Camera.main.transform.right;
        Vector3 camUp = Camera.main.transform.up;

        float x = Vector3.Dot(dirToTarget, camRight);
        float y = Vector3.Dot(dirToTarget, camUp);
        Vector2 screenDir = new Vector2(x, y).normalized;

        return screenCenter + screenDir * (Screen.width / 2f - screenEdgeBuffer);
        //return new Vector2(screenPoint.x, screenPoint.y);
    }
    
    public void UpdateArrow(float amount, float maxCapacity)
    {
        fillImage.fillAmount = Remap(amount, 0.0f, maxCapacity, 0.0f, 1.0f);
    }

    public void SetActive(bool show)
    {
        if(show)
            UpdateArrowPosition();
        
        gameObject.SetActive(show);
        IsActive = show;
    }
    
    public void PlayerSeen()
    {
        fillImage.enabled = false;
        backFillImage.color = Color.red;
        backFillImage.sprite = exclamationSprite;
        if(hideExclamationCorutine == null)
            hideExclamationCorutine = StartCoroutine(HideExclamation());
    }

    public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
    }

    private IEnumerator HideExclamation()
    {
        yield return new WaitForSeconds(waitTimeToDisspear);
        SetActive(false);
    }
}
