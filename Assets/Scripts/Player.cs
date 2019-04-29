using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject {

    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public Text foodText;
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;


    private Animator animator;
    private int food;
    public static bool dead = false;
    private Vector2 touchOrigin = -Vector2.one;
    
	protected override void Start() {
        animator = GetComponent<Animator>();
        dead = false;
        food = GameManager.instance.playerFoodPoints;

        foodText.text = "Food: " + food;

        base.Start();
    }

    private void OnDisable()
    {
        if(!dead)
            GameManager.instance.playerFoodPoints = food;
    }

    void Update() {
        if (!GameManager.instance.playersTurn)
            return;

        int horizontal = 0;
        int vertical = 0;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
            vertical = 0;

#endif

#if UNITY_EDITOR || UNITY_ANDROID

        if (Input.touchCount > 0)
        {
            Touch myTouch = Input.touches[0];
            if(myTouch.phase == TouchPhase.Began)
            {
                touchOrigin = myTouch.position;
            }
            else if(myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
            {
                Vector2 touchEnd = myTouch.position;
                float x = touchEnd.x - touchOrigin.x;
                float y = touchEnd.y - touchOrigin.x;
                touchOrigin.x = -1;
                if (Mathf.Abs(x) > Mathf.Abs(y))
                    horizontal = x > 0 ? 1 : -1;
                else
                    vertical = y > 0 ? 1 : -1;
            }
        }
#endif

        if ((horizontal != 0 || vertical != 0) && !dead && !GameManager.doingSetup)
            AttemptMove<Wall> (horizontal, vertical);
	}

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        base.AttemptMove<T>(xDir, yDir);

        if (canMove)
        {
            food--;
            foodText.text = "Food: " + food;
        }

        RaycastHit2D hit;
        if(Move(xDir, yDir, out hit))
        {
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

        CheckIfGameOver();

        GameManager.instance.playersTurn = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if(collision.tag == "Food")
        {
            if(food + pointsPerFood <= int.MaxValue)
                food += pointsPerFood;
            foodText.text = "+" + pointsPerFood + " Food: " + food;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            collision.gameObject.SetActive(false);
        }
        else if (collision.tag == "Soda")
        {
            food += pointsPerSoda;
            foodText.text = "+" + pointsPerSoda + " Food: " + food;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            collision.gameObject.SetActive(false);
        }
    }

    protected override void OnCantMove<T>(T component, int xDir, int yDir)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        if (xDir != 0)
            animator.SetTrigger("playerChop");
        else
            animator.SetTrigger("playerSpecial");
    }

    private void Restart()
    {
        SceneManager.LoadScene("Main");
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        foodText.text = "-" + loss + " Food: " + food;
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if (food <= 0)
        {
            foodText.text = "";
            SoundManager.instance.musicSource.Stop();
            SoundManager.instance.PlaySingle(gameOverSound);
            animator.SetTrigger("playerDead");
            dead = true;
            StartCoroutine(WaitForGO());
        }
    }

    IEnumerator WaitForGO()
    {
        yield return new WaitForSecondsRealtime(1.5f);

        GameManager.instance.GameOver();
    }
}
