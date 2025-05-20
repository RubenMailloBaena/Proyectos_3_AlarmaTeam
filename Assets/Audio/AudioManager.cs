using FMODUnity;
using FMOD.Studio;
using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    // Referencia al bus de música
    private Bus musicBus;

    void Start()
    {
        // Carga bloqueante del banco Maestro
        RuntimeManager.LoadBank("Master Bank", true);
        // (Opcional) carga el banco de strings si lo usas para nombres dinámicos
        RuntimeManager.LoadBank("Master Bank.strings", true);

        // Una vez cargados, obtenemos el bus por su ruta exacta
        musicBus = RuntimeManager.GetBus("bus:/Buses/Music");

        // Comprueba que existe
        if (musicBus.isValid())
        {
            Debug.Log("Bus de música cargado correctamente.");
        }
        else
        {
            Debug.LogError("No se encontró el bus de música. Revisa la ruta y que el banco esté compilado.");
        }

        // Por ejemplo, hacer un fade
        StartCoroutine(FadeBusVolume(0f, 1f, 2f));
    }

    IEnumerator FadeBusVolume(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float vol = Mathf.Lerp(from, to, elapsed / duration);
            musicBus.setVolume(vol);
            elapsed += Time.deltaTime;
            yield return null;
        }
        musicBus.setVolume(to);
    }
}

