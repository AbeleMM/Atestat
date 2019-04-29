using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public float levelStartDelay = 2f;
    public float turnDelay = .1f;

    public static GameManager instance = null;
    
    private BoardManager boardScript;
    public int playerFoodPoints;
    [HideInInspector]
    public bool playersTurn;

    private Text levelText;
    private GameObject levelImage;
    public static int level;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    public static bool doingSetup;

    public static string seed;
    public static bool useRandomSeed = true;
    public static System.Random pseudoRandom;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        Refresh();
        
        DontDestroyOnLoad(gameObject);

        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
	}

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        level++;
        instance.InitGame();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void InitGame()
    {
        doingSetup = true;

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        boardScript.SetupScene(level);
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    public void GameOver()
    {
        if(level > 1)
            levelText.text = "After " + level + " days, you starved.";
        else
            levelText.text = "After " + level + " day, you starved.";

        levelImage.SetActive(true);
        GameObject.Find("Canvas").transform.Find("GameOver").gameObject.SetActive(true);
    }

    public void Refresh()
    {
        if (useRandomSeed)
            seed = Time.time.ToString();
        pseudoRandom = new System.Random(seed.GetHashCode());

        playerFoodPoints = 100;
        level = 0;
        playersTurn = true;
        SoundManager.instance.musicSource.PlayDelayed(0.5f);
    }

    void Update()
    {
        if (Player.dead || PauseMenu.gameIsPaused)
            Cursor.visible = true;
        else
            Cursor.visible = false;

        if (playersTurn || enemiesMoving || doingSetup)
            return;

        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if(enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;
    }
}