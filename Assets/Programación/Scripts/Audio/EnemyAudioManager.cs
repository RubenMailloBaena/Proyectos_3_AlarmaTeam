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
    CharmWalk
}

public class EnemyAudioManager : MonoBehaviour
{
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
        { SoundType.CharmWalk, "event:/Enemigo/enemigo_pasos_madera_andar" }
    };

    private EventInstance currentInstance;
    private string currentClip;
    private EnemyController controller;

    private void OnEnable()
    {
        controller.OnPlaySound += SetSound;
        controller.OnStopSound += StopSound;
    }

    private void OnDisable()
    {
        controller.OnPlaySound -= SetSound;
        controller.OnStopSound -= StopSound;
    }

    private void SetSound(SoundType soundType)
    {
        if (!clipMap.TryGetValue(soundType, out var clipName))
        {
            Debug.LogWarning($"[EnemyAudio] No hay clip mapeado para {soundType}");
            return;
        }

        // Si ya está sonando ese clip y aún se está reproduciendo, no hacer nada
        if (currentClip == clipName && currentInstance.isValid())
        {
            currentInstance.getPlaybackState(out PLAYBACK_STATE state);
            if (state == PLAYBACK_STATE.PLAYING)
                return;
        }

        StopSound(); // Cortar cualquier sonido anterior

        currentInstance = RuntimeManager.CreateInstance(clipName);
        currentInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        RuntimeManager.AttachInstanceToGameObject(currentInstance, transform, GetComponent<Rigidbody>());
        currentInstance.start();

        currentClip = clipName;
    }

    public void StopSound()
    {
        if (currentInstance.isValid())
        {
            currentInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            currentInstance.release();
        }

        currentClip = null;
    }
}
