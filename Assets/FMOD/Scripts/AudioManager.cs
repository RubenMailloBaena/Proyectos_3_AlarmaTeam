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

    void HandlePlaySound2D(string path)
    {
        PlayOneShot(path, Vector3.zero);
    }

    void HandlePlaySound3D(string path, Vector3 position)
    {
        PlayOneShot(path, position);
    }

    private void OnEnable()
    {
        AudioEvents.onPlaySound2D += HandlePlaySound2D;
        AudioEvents.onPlaySound3D += HandlePlaySound3D;
    }

    private void OnDisable()
    {
        AudioEvents.onPlaySound2D -= HandlePlaySound2D;
        AudioEvents.onPlaySound3D -= HandlePlaySound3D;
    }
}

public static class AudioEvents
{
    public static Action<string, Vector3> onPlaySound3D;
    public static Action<string> onPlaySound2D;
}
