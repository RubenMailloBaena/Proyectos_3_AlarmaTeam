using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public enum SoundType
{
    Idle,
    Walk,
    Run,
    Attack,
    Dead,
    Charm,
    CharmWalk,
    Seen,
    Detect,
    LookAt
}

public class EnemyAudioManager : MonoBehaviour
{
    private Dictionary<SoundType, EventInstance> activeInstances = new Dictionary<SoundType, EventInstance>();

    private EnemyController controller;

    private void Awake()
    {
        controller = GetComponent<EnemyController>();
    }

    private static readonly Dictionary<SoundType, string> clipMap = new Dictionary<SoundType, string>
    {
        { SoundType.Idle,      "event:/Enemigo/enemigo_voz_tranquila" },
        { SoundType.Walk,      "event:/Enemigo/enemigo_pasos_madera_andar" },
        { SoundType.Run,       "event:/Enemigo/enemigo_pasos_madera_correr" },
        { SoundType.Attack,    "event:/Enemigo/enemigo_rezo" },
        { SoundType.Dead,      "event:/Enemigo/enemigo_muerte" },
        { SoundType.Charm,     "event:/Jugador/jugador_habilidad_control_mental" },
        { SoundType.CharmWalk, "event:/Enemigo/enemigo_pasos_madera_andar" },
        { SoundType.Seen, "event:/Enemigo/enemigo_deteccion_completa" },
        { SoundType.Detect, "event:/Enemigo/enemigo_deteccion_completa" },
        { SoundType.LookAt, "event:/Enemigo/enemigo_voz_tranquila" }
    };

    

    private void OnEnable()
    {
        controller.OnPlaySound += SetSound;
        controller.OnStopSound += StopSound;
        controller.OnStopAllSounds += StopAllSounds;
    }

    private void OnDisable()
    {
        controller.OnPlaySound -= SetSound;
        controller.OnStopSound -= StopSound;
        controller.OnStopAllSounds -= StopAllSounds;
    }

    public void SetSound(SoundType soundType)
    {
        // Si ya existe una instancia para este sonido y está reproduciéndose, no hacer nada
        if (activeInstances.TryGetValue(soundType, out var existingInstance) && existingInstance.isValid())
        {
            existingInstance.getPlaybackState(out PLAYBACK_STATE state);
            if (state == PLAYBACK_STATE.PLAYING)
                return;
        }

        if (!clipMap.TryGetValue(soundType, out var clipName))
        {
            Debug.LogWarning($"[EnemyAudio] No hay clip mapeado para {soundType}");
            return;
        }

        // Crear nueva instancia
        var newInstance = RuntimeManager.CreateInstance(clipName);
        newInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        RuntimeManager.AttachInstanceToGameObject(newInstance, transform, GetComponent<Rigidbody>());
        newInstance.start();

        // Guardar o reemplazar la instancia
        if (activeInstances.ContainsKey(soundType))
        {
            activeInstances[soundType].stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            activeInstances[soundType].release();
            activeInstances[soundType] = newInstance;
        }
        else
        {
            activeInstances.Add(soundType, newInstance);
        }
    }

    public void StopSound(SoundType soundType)
    {
        if (activeInstances.TryGetValue(soundType, out var instance) && instance.isValid())
        {
            instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            instance.release();
            activeInstances.Remove(soundType);
        }
    }

    public void StopAllSounds()
    {
        foreach (var kvp in activeInstances)
        {
            if (kvp.Value.isValid())
            {
                kvp.Value.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                kvp.Value.release();
            }
        }
        activeInstances.Clear();
    }
}

