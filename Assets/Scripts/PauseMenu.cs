using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using TMPro;

public class PauseMenu : MonoBehaviour {

    public static bool gameIsPaused = false;

    public GameObject pauseMenuUI;
    public GameObject levelImageUI;
    public GameObject foodTextUI;
    Text seedText;
    public TextMeshProUGUI textMeshPro;

    [Serializable]
    public class Leaderboard
    {
        public string name;
        public int days;

        public Leaderboard(string n, int d)
        {
            name = n;
            days = d;
        }
    }

    [Serializable]
    public class LeaderboardList
    {
        public List<Leaderboard> list = new List<Leaderboard>();
    }

    void Awake()
    {
        seedText = pauseMenuUI.GetComponentInChildren<Text>();
        seedText.text = "The current seed is: " + GameManager.seed;
    }

    void Update () {
        if (Input.GetKeyDown(KeyCode.Escape) && levelImageUI.activeSelf == false && Player.dead == false)
        {
            if (gameIsPaused)
                Resume();
            else
                Pause();
        }
	}

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        foodTextUI.SetActive(true);
        Time.timeScale = 1f;

        gameIsPaused = false;
    }

    public void Retry()
    {
        gameIsPaused = false;

        GameManager.useRandomSeed = false;
        GameManager.instance.Refresh();
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    void Pause()
    {
        gameIsPaused = true;

        pauseMenuUI.SetActive(true);
        foodTextUI.SetActive(false);
        Time.timeScale = 0f;
    }

    public void LoadMenu()
    {
        gameIsPaused = false;

        Time.timeScale = 1f;
        SceneManager.LoadScene("Start", LoadSceneMode.Single);
        SoundManager.instance.musicSource.Stop();
        GameManager.instance.enabled = false; 
    }

    public void SubmitHighScore()
    {
        LeaderboardList scoreTable;

        if (PlayerPrefs.HasKey(GameManager.seed))
            scoreTable = JsonUtility.FromJson<LeaderboardList>(PlayerPrefs.GetString(GameManager.seed));
        else
            scoreTable = new LeaderboardList();

        scoreTable.list.Add(new Leaderboard(textMeshPro.text, GameManager.level));
        scoreTable.list = scoreTable.list.OrderByDescending(x => x.days).ThenBy(x => x.name).ToList();

        PlayerPrefs.SetString(GameManager.seed, JsonUtility.ToJson(scoreTable));

         LoadMenu();
    }
}