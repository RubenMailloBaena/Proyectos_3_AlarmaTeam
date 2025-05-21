using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private Dictionary<string, EventInstance> activeEvents = new Dictionary<string, EventInstance>();

    private void Awake()
    {
        if (Instance != null) 
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void HandlePlaySound3D(string path, Vector3 pos)
    {
        if(activeEvents.TryGetValue(path, out var existingInstance))
        {
            existingInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            existingInstance.release();
            activeEvents.Remove(path);
        }
        
        EventInstance instance = RuntimeManager.CreateInstance(path);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(pos));
        instance.start();

        activeEvents[path] = instance;
    }

    public void HandlePlaySound2D(string path)
    {
        if (activeEvents.TryGetValue(path, out var existingInstance))
        {
            existingInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            existingInstance.release();
            activeEvents.Remove(path);
        }

        EventInstance instance = RuntimeManager.CreateInstance(path);
        instance.start();

        activeEvents[path] = instance;
    }


    public void HandleStopSound(string path)
    {
        if (activeEvents.TryGetValue(path, out var instance))
        {
            instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            instance.release();
            activeEvents.Remove(path);
        }
    }

}



