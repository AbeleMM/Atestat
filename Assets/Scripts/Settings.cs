using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using pm = PauseMenu;

public class Settings : MonoBehaviour {

    public AudioMixer audioMixer;

    Resolution[] resolutions;

    public Dropdown resolutionDropdown;

    public TextMeshProUGUI seedLoad;

    public Slider effectsSlider;
    public Slider musicSlider;

    public GameObject splashScreen;

    static bool firstTime = true;

    public TextMeshProUGUI scoreSearch;
    public TextMeshProUGUI displayScore;

    pm.LeaderboardList scoreTable;
    int nr;

    private void Awake()
    {
#if UNITY_EDITOR || UNITY_STANDALONE

        QualitySettings.vSyncCount = 1;

#endif

#if UNITY_EDITOR || UNITY_ANDROID|| UNITY_WEBPLAYER

        QualitySettings.vSyncCount = 2;

#endif

        if (firstTime)
        {
            splashScreen.SetActive(true);
            firstTime = false;
        }
    }

    void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for(int i = 0; i < resolutions.Length; i++)
        {
            options.Add(resolutions[i].width + " x " + resolutions[i].height);

            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        float value;
        audioMixer.GetFloat("EffectsVolume", out value);
        effectsSlider.value = value;
        audioMixer.GetFloat("MusicVolume", out value);
        musicSlider.value = value;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetEffectsVolume(float volume)
    {
        audioMixer.SetFloat("EffectsVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }

    public void SetQuality (int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFulscreen (int isFulscreen)
    {
        if (isFulscreen == 0)
            Screen.fullScreen = false;
        else
            Screen.fullScreen = true;
    }

    public void PlayGame(bool rand)
    {
        GameManager.useRandomSeed = rand;
        GameManager.seed = seedLoad.text;

        if(GameManager.instance != null)
        {
            GameManager.instance.enabled = true;
            GameManager.instance.Refresh();
        }
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    public void SearchSeedScores()
    {
        displayScore.text = "";

        if (PlayerPrefs.HasKey(scoreSearch.text))
        {
            nr = 0;
            scoreTable = JsonUtility.FromJson<pm.LeaderboardList>(PlayerPrefs.GetString(scoreSearch.text));
            foreach (pm.Leaderboard currentRecord in scoreTable.list)
            {
                nr++;
                displayScore.text += nr.ToString() + ") " + currentRecord.name + " | " + currentRecord.days.ToString() + "\n"; 
            }
        }
    }
}