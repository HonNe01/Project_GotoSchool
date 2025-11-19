using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;
    public TMP_Dropdown resolutionDropDown;
    public Toggle fullScreenToggle;

    Resolution[] resolutions;

    // Start is called before the first frame update
    void Start()
    {
        // 해상도 드롭다운 채우기
        resolutionDropDown.ClearOptions();
        resolutions = new Resolution[4];

        resolutions[0] = new Resolution { width = 3840, height = 2160 };
        resolutions[1] = new Resolution { width = 2560, height = 1440 };
        resolutions[2] = new Resolution { width = 1920, height = 1080 };
        resolutions[3] = new Resolution { width = 1280, height = 720 };

        List<TMP_Dropdown.OptionData> options = new()
        {
            new TMP_Dropdown.OptionData("3840 x 2160"),
            new TMP_Dropdown.OptionData("2560 x 1440"),
            new TMP_Dropdown.OptionData("1920 x 1080"),
            new TMP_Dropdown.OptionData("1280 x 720")
        };

        
        resolutionDropDown.AddOptions(options);

        fullScreenToggle.isOn = Screen.fullScreen;
    }

    public void OnBgmChanged()
    {
        if (AudioManager.instance == null) return;

        float value = bgmSlider.value;

        AudioManager.instance.bgmSlider = value;
        AudioManager.instance.UpdateBgmVolume();
    }

    public void OnSfxChanged()
    {
        if (AudioManager.instance == null) return;

        float value = sfxSlider.value;

        AudioManager.instance.sfxSlider = value;
        AudioManager.instance.UpdateSfxVolume();
    }

    public void OnResolutionChanged()
    {
        Debug.Log($" [ Option Manger ] ResolutionChanged : {resolutions[resolutionDropDown.value]}");
        var resolution = resolutions[resolutionDropDown.value];

        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void OnFullScreenChanged()
    {
        Debug.Log($" [ Option Manger ] FullScreen : {fullScreenToggle.isOn}");

        Screen.fullScreen = fullScreenToggle.isOn;
    }

    public void CloseOption()
    {
        gameObject.SetActive(false);
    }
}
