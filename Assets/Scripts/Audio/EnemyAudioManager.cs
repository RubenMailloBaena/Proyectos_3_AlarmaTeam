using System.Collections.Generic;
using UnityEngine;

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

public static class EnemyAudioManager
{
    private static readonly Dictionary<SoundType, string> clipMap = new Dictionary<SoundType, string>
    {
        { SoundType.Idle,      "event:/Enemigo/enemigo_voz_tranquila"   },
        { SoundType.Walk,      "event:/Enemigo/enemigo_pasos_madera_andar" },
        { SoundType.Run,       "event:/Enemigo/enemigo_pasos_madera_correr" },
        { SoundType.Attack,    "event:/Enemigo/enemigo_rezo"            },
        { SoundType.Dead,      "event:/Enemigo/enemigo_voz_sorpresa"     },
        { SoundType.Charm,     "event:/Enemigo/enemigo_charmed"          },
        { SoundType.CharmWalk, "charm_footsteps"                        }
    };

    private static string currentClip;

    public static void SetSound(SoundType soundType, Vector3 position)
    {
        StopSound();
        if (!clipMap.TryGetValue(soundType, out var clipName))
        {
            Debug.LogWarning($"[EnemyAudio] No hay clip mapeado para {soundType}");
            return;
        }

        if (currentClip == clipName)
            return;

        if (!string.IsNullOrEmpty(currentClip))
            AudioManager.Instance.HandleStopSound(currentClip, true);

        AudioManager.Instance.HandlePlaySound3D(clipName, position);
        currentClip = clipName;
    }

    private static void StopSound()
    {
        if (string.IsNullOrEmpty(currentClip))
            return;

        AudioManager.Instance.HandleStopSound(currentClip, true);
        currentClip = null;
    }
}
