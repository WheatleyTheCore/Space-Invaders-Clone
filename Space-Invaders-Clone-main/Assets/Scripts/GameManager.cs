using UnityEngine;
using UnityEngine.UI;

public sealed class GameManager : MonoBehaviour
{
    private Player player;
    private Invaders invaders;
    private MysteryShip mysteryShip;
    private Bunker[] bunkers;
    private string gameState = "start";

    public GameObject MainMenuUI;
    public GameObject gameOverUI;
    public Text scoreText;
    public Text livesText;

    public int score { get; private set; } = 0;
    public int lives { get; private set; } = 0;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        invaders = FindObjectOfType<Invaders>();
        mysteryShip = FindObjectOfType<MysteryShip>();
        bunkers = FindObjectsOfType<Bunker>();

        
    }

    private void Start()
    {
        player.killed += OnPlayerKilled;
        mysteryShip.killed += OnMysteryShipKilled;
        invaders.killed += OnInvaderKilled;
        gameOverUI.gameObject.SetActive(false);
        MainMenuUI.gameObject.SetActive(true);
        player.gameObject.SetActive(false);
        invaders.gameObject.SetActive(false);
        for (int i = 0; i < bunkers.Length; i++)
        {
            bunkers[i].gameObject.SetActive(false);
        }

    }

    private void Update()
    {
        if (gameState == "start" && Input.GetKeyDown(KeyCode.Return))
        {
            gameState = "playing";
            MainMenuUI.gameObject.SetActive(false);
            NewGame();
        } else if (lives <= 0 && Input.GetKeyDown(KeyCode.Return))
        {
            NewGame();
        }
    }

    private void NewGame()
    {
        gameOverUI.SetActive(false);
        MainMenuUI.SetActive(false);

        SetScore(0);
        SetLives(3);
        NewRound();
    }

    private void NewRound()
    {
        invaders.ResetInvaders();
        invaders.gameObject.SetActive(true);

        for (int i = 0; i < bunkers.Length; i++)
        {
            bunkers[i].ResetBunker();
        }

        Respawn();
    }

    private void Respawn()
    {
        Vector3 position = player.transform.position;
        position.x = 0f;
        player.transform.position = position;
        player.gameObject.SetActive(true);
    }

    private void GameOver()
    {
        gameOverUI.SetActive(true);
        invaders.gameObject.SetActive(false);
        for (int i = 0; i < bunkers.Length; i++)
        {
            bunkers[i].gameObject.SetActive(false);
        }
    }

    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString().PadLeft(4, '0');
    }

    private void SetLives(int lives)
    {
        this.lives = Mathf.Max(lives, 0);
        livesText.text = "Lives: " + lives.ToString();
    }

    private void OnPlayerKilled()
    {
        SetLives(lives - 1);

        player.gameObject.SetActive(false);

        if (lives > 0)
        {
            Invoke(nameof(NewRound), 1f);
        }
        else
        {
            GameOver();
        }
    }

    private void OnInvaderKilled(Invader invader)
    {
        SetScore(score + invader.score);

        if (invaders.AmountKilled == invaders.TotalAmount)
        {
            NewRound();
        }
    }

    private void OnMysteryShipKilled(MysteryShip mysteryShip)
    {
        SetScore(score + mysteryShip.score);
    }

}
