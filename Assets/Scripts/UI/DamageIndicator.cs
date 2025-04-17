using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    /*[Header("Sprites")]
    [SerializeField] private Sprite arrowSprite; // Debe apuntar hacia ARRIBA en su orientaci�n natural
    [SerializeField] private Sprite crossSprite;

    [Header("Target")]
    public Vector3 targetPosition;

    [Header("UI Components")]
    private RectTransform pointerRectTransform;
    private Image pointerImage;

    [Header("Settings")]
    [SerializeField] private float screenEdgeBuffer = 40f;
    [SerializeField] private float smoothRotationSpeed = 5f;
    [SerializeField] private float smoothPositionSpeed = 15f;

    private float previousAngle;

    private void Awake()
    {
        pointerRectTransform = transform.Find("Pointer").GetComponent<RectTransform>();
        pointerImage = transform.Find("Pointer").GetComponent<Image>();
        previousAngle = 0;
    }

    private void Update()
    {
        Vector3 targetScreenPoint = Camera.main.WorldToScreenPoint(targetPosition);
        bool isOffscreen = targetScreenPoint.z <= 0 ||
                         targetScreenPoint.x <= 0 || targetScreenPoint.x >= Screen.width ||
                         targetScreenPoint.y <= 0 || targetScreenPoint.y >= Screen.height;

        if (!isOffscreen)
        {
            pointerImage.sprite = crossSprite;
            pointerRectTransform.position = Vector3.Lerp(pointerRectTransform.position, targetScreenPoint, smoothPositionSpeed * Time.deltaTime);
            pointerRectTransform.localEulerAngles = Vector3.zero;
            previousAngle = 0;
        }
        else
        {
            pointerImage.sprite = arrowSprite;

            // Posici�n suavizada
            Vector2 clampedPosition = GetClampedScreenPosition(targetScreenPoint);
            pointerRectTransform.position = Vector2.Lerp(pointerRectTransform.position, clampedPosition, smoothPositionSpeed * Time.deltaTime);

            // C�lculo de direcci�n (relativo al centro de pantalla)
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Vector2 screenTarget = GetCorrectedScreenPoint(targetScreenPoint);
            Vector2 direction = (screenTarget - screenCenter).normalized;

            // Calcular �ngulo (0� = arriba, 90� = derecha) - ajustado para sprite que apunta arriba
            float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

            // Suavizado de rotaci�n
            float deltaAngle = Mathf.DeltaAngle(previousAngle, targetAngle);
            float smoothDelta = Mathf.Lerp(0, deltaAngle, smoothRotationSpeed * Time.deltaTime);
            float newAngle = previousAngle + smoothDelta;

            pointerRectTransform.localEulerAngles = new Vector3(0, 0, -newAngle); // Invertido por sistema de coordenadas UI
            previousAngle = newAngle;
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
        if (screenPoint.z < 0)
        {
            // Proyectar al borde opuesto si est� detr�s de la c�mara
            screenPoint.x = Screen.width - screenPoint.x;
            screenPoint.y = Screen.height - screenPoint.y;
        }
        return new Vector2(screenPoint.x, screenPoint.y);
    }

    public void Show(Vector3 newTargetPosition)
    {
        gameObject.SetActive(true);
        targetPosition = newTargetPosition;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }*/
}