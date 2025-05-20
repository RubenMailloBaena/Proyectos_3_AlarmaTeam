using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlayOneShot(string path, Vector3 position)
    {
        RuntimeManager.PlayOneShot(path, position);
    }

    public EventInstance PlayEventInstance(string path)
    {
        EventInstance instance = RuntimeManager.CreateInstance(path);
        instance.start();
        return instance;
    }

    public void StopEvent(EventInstance instance)
    {
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        instance.release();
    }
    private void OnEnable()
    {
        AudioEvents.OnPlaySound3D += HandlePlaySound3D;
        AudioEvents.OnPlaySound2D += HandlePlaySound2D;
    }

    private void OnDisable()
    {
        AudioEvents.OnPlaySound3D -= HandlePlaySound3D;
        AudioEvents.OnPlaySound2D -= HandlePlaySound2D;
    }

    void HandlePlaySound3D(string path, Vector3 pos)
    {
        PlayOneShot(path, pos);
    }

    void HandlePlaySound2D(string path)
    {
        PlayOneShot(path, Vector3.zero); // o algún sistema de 2D
    }

}

public static class AudioEvents
{
    public static Action<string, Vector3> OnPlaySound3D;
    public static Action<string> OnPlaySound2D;
}

