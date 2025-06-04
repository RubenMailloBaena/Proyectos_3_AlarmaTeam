using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private Dictionary<string, EventInstance> activeEvents = new Dictionary<string, EventInstance>();
    private Dictionary<string, float> groupCooldowns = new Dictionary<string, float>();
    private const float groupCooldown = 0.1f;


    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public void HandlePlay3DOneShot(string groupKey, string eventPath, Vector3 pos)
    {
        if (groupCooldowns.TryGetValue(groupKey, out float lastTime))
        {
            if (Time.time - lastTime < groupCooldown)
                return;
        }

        RuntimeManager.PlayOneShot(eventPath, pos);
        groupCooldowns[groupKey] = Time.time;
    }

    public void HandlePlay2DOneShot(string path)
    {
        RuntimeManager.PlayOneShot(path);
    }
    public void HandlePlaySound3D(string path, Vector3 pos)
    {
        if (activeEvents.TryGetValue(path, out var existingInstance))
        {
            if (existingInstance.isValid())
            {
                existingInstance.getPlaybackState(out PLAYBACK_STATE state);
                if (state == PLAYBACK_STATE.PLAYING)
                {
                    existingInstance.set3DAttributes(RuntimeUtils.To3DAttributes(pos));
                    return;
                }
            }

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
            if (existingInstance.isValid())
            {
                existingInstance.getPlaybackState(out PLAYBACK_STATE state);
                if (state == PLAYBACK_STATE.PLAYING)
                {
                    return;
                }
            }

            existingInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            existingInstance.release();
            activeEvents.Remove(path);
        }

        EventInstance instance = RuntimeManager.CreateInstance(path);
        instance.start();

        activeEvents[path] = instance;
    }
        public void HandleStopSound(string path, bool immediate)
        {
            if (!activeEvents.TryGetValue(path, out var instance))
                return;

            if (instance.isValid())
            {
                instance.getPlaybackState(out PLAYBACK_STATE state);
                if (state != PLAYBACK_STATE.PLAYING)
                {
                    instance.release();
                    activeEvents.Remove(path);
                    return;
                }
            }

            var mode = immediate
                ? FMOD.Studio.STOP_MODE.IMMEDIATE
                : FMOD.Studio.STOP_MODE.ALLOWFADEOUT;

            instance.stop(mode);
            instance.release();
            activeEvents.Remove(path);
        }

    }
