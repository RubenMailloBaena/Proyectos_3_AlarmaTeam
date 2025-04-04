using System.Collections;
using UnityEngine;

public class BridgeController : MonoBehaviour
{
    [SerializeField] private Transform finalpoint;
    [SerializeField] private float duration = 1f;

    private bool isMoving = false;
    private float elapsedTime = 0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && !isMoving)
        {
            StartCoroutine(MoveBridge());
        }
    }

    IEnumerator MoveBridge()
    {
        isMoving = true;
        elapsedTime = 0f;

        Vector3 start = transform.position;
        Vector3 end = new Vector3(finalpoint.position.x, transform.position.y, finalpoint.position.z);

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(start, end, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        isMoving = false;
    }
}