using FMOD;
using FMOD.Studio;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Slider sonidoSlider;
    private Bus masterBus;
    private Resolution[] resolutions;

    void Start()
    {
        masterBus = RuntimeManager.GetBus("bus:/");

        float initialDb = 0f;
        SetVolume(initialDb); 
        sonidoSlider.value = initialDb; 

        SetupResolutionDropdown();
    }

    private void Awake()
    {
        masterBus = RuntimeManager.GetBus("bus:/");
    }
    public void SetVolume(float dbVolume)
    {
        float linearVolume = Mathf.Pow(10f, dbVolume / 20f);

        linearVolume = Mathf.Clamp(linearVolume, 0.0001f, 1f);

        RESULT result = masterBus.setVolume(linearVolume);
      

    }


    private void SetupResolutionDropdown()
    {
        resolutions = new Resolution[]
        {
            new Resolution { width = 640, height = 360 },
            new Resolution { width = 854, height = 480 },
            new Resolution { width = 960, height = 540 },
            new Resolution { width = 1280, height = 720 },
            new Resolution { width = 1366, height = 768 },
            new Resolution { width = 1600, height = 900 },
            new Resolution { width = 1920, height = 1080 },
            new Resolution { width = 2560, height = 1440 },
            new Resolution { width = 3200, height = 1800 },
            new Resolution { width = 3840, height = 2160 },
            new Resolution { width = 5120, height = 2880 },
            new Resolution { width = 7680, height = 4320 }
        };

        resolutionDropdown.ClearOptions();
        var options = new System.Collections.Generic.List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = $"{resolutions[i].width} x {resolutions[i].height}";
            options.Add(option);

            if (resolutions[i].width == Screen.width &&
                resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
}