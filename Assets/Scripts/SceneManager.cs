using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SceneManager : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField]
    AudioClip BlipSound;

    // titles and messages
    [SerializeField]
    GameObject HUDTitle;
    [SerializeField]
    GameObject HUDHowToPlay;
    [SerializeField]
    GameObject HUDStart;
    [SerializeField]
    GameObject HUDGameOver;
    [SerializeField]
    GameObject HUDGameOverReason;
    [SerializeField]
    GameObject HUDGameOverReasonBack;
    [SerializeField]
    GameObject HUDFinalScore;
    [SerializeField]
    GameObject HUDTopScore;
    [SerializeField]
    GameObject HUDScore;
    [SerializeField]
    GameObject HUDCredits;

    [SerializeField]
    GameObject Canvas;
    [SerializeField]
    GameObject FruitButtonPrefab;
    int Columns = 4;
    int Rows = 10;
    float buttonWidth = 192f;
    float buttonHeight = 256f;
    List<GameObject> fruitButtonsList = new List<GameObject>();

    float gameOverTimer = 0f;
    float gameOverTimerMax = 2f;

    // game scroll speed
    Vector2 initialScrollSpeed = new Vector2(0, 300f);
    Vector2 maxScrollSpeed = new Vector2(0, 1000f);
    Vector2 scrollSpeed = new Vector2(0, 300f);
    // moving direction
    Vector2 scrollDirection = new Vector2(0, -1f);

    // Start is called before the first frame update
    void Awake()
    {
        Application.targetFrameRate = 60;

        Globals.BestScore = Globals.LoadFromPlayerPrefs(Globals.BestScorePlayerPrefsKey);

        audioSource = this.GetComponent<AudioSource>();

        for (int r = 0; r < Rows; r++)
        {
            int strawberryIndex = Random.Range(0, 4);
            for (int c = 0; c < Columns; c++)
            {
                GameObject button = Instantiate(
                    FruitButtonPrefab,
                    new Vector3(c * buttonWidth + buttonWidth * .5f, r * buttonHeight + buttonHeight * .5f, 0),
                    Quaternion.identity,
                    Canvas.transform);
                button.GetComponent<FruitButton>().Init(c == strawberryIndex, c == 0 && r == 0, r == 0);
                fruitButtonsList.Add(button);
            }
        }

        HUDTitle.GetComponent<MoveNormal>().MoveDown();
        HUDHowToPlay.GetComponent<MoveNormal>().MoveRight();
        HUDStart.GetComponent<MoveNormal>().MoveLeft();
        HUDCredits.GetComponent<MoveNormal>().MoveRight();
    }

    // Update is called once per frame
    void Update()
    {
        if (Globals.CurrentGameState == Globals.GameState.TitleScreen)
        {
            UpdateTitleScreenState();
        }
        else if (Globals.CurrentGameState == Globals.GameState.Playing)
        {
            UpdatePlaying();
        }
        else if (Globals.CurrentGameState == Globals.GameState.Dead)
        {
            UpdateDead();
        }
        else if (Globals.CurrentGameState == Globals.GameState.Restart)
        {
            UpdateRestart();
        }
    }

    void FixedUpdate()
    {
        if (Globals.CurrentGameState == Globals.GameState.Playing)
        {
            float speed = Mathf.Min(maxScrollSpeed.y, initialScrollSpeed.y + Mathf.Floor(Globals.CurrentScore / 2) * 35f);
            Vector2 movement = new Vector2(0, speed * scrollDirection.y);
            for (int i = 0; i < fruitButtonsList.Count; i++)
            {
                fruitButtonsList[i].GetComponent<Rigidbody2D>().velocity = movement;
            }
        }
        else
        {
            Vector2 movement = new Vector2(0, 0);
            for (int i = 0; i < fruitButtonsList.Count; i++)
            {
                fruitButtonsList[i].GetComponent<Rigidbody2D>().velocity = movement;
            }
        }
    }

    void UpdateTitleScreenState()
    {

    }

    void UpdatePlaying()
    {
        float minY = -128f;
        for (int r = 0; r < Rows; r++)
        {
            int strawberryIndex = Random.Range(0, 4);
            for (int c = 0; c < Columns; c++)
            {
                int i = r * Columns + c;
                RectTransform rectTransform = fruitButtonsList[i].GetComponent<RectTransform>();
                FruitButton fruitButton = fruitButtonsList[i].GetComponent<FruitButton>();
                if (rectTransform.anchoredPosition.y < minY)
                {
                    int abutIndex = i < Columns ? fruitButtonsList.Count - 1 - i : i - Columns;
                    fruitButtonsList[i].transform.localPosition = new Vector2(
                            fruitButtonsList[i].transform.localPosition.x,
                            fruitButtonsList[abutIndex].transform.localPosition.y + 256f
                        );
                    fruitButton.Init(c == strawberryIndex, false, false);
                }
                if (rectTransform.anchoredPosition.y < 0)
                {
                    if (fruitButton.isStrawberry && !fruitButton.collected)
                    {
                        fruitButton.SetIncorrect(false);
                    }
                }
            }
        }
    }

    void UpdateDead()
    {
        gameOverTimer -= Time.deltaTime;
        if (gameOverTimer <= 0)
        {
            HUDStart.GetComponent<MoveNormal>().MoveLeft();
            Globals.CurrentGameState = Globals.GameState.Restart;
        }
    }

    void UpdateRestart()
    {

    }

    public void StartReadyMode()
    {
        audioSource.PlayOneShot(BlipSound, 1f);

        HUDGameOver.transform.localPosition = new Vector3(0, 2000f, 0);
        HUDTitle.SetActive(false);
        HUDHowToPlay.SetActive(false);
        HUDCredits.SetActive(false);
        HUDStart.transform.localPosition = new Vector3(2000f, -300f, 0);
        HUDScore.SetActive(true);

        for (int r = 0; r < Rows; r++)
        {
            int strawberryIndex = Random.Range(0, 4);
            for (int c = 0; c < Columns; c++)
            {
                int i = r * Columns + c;
                RectTransform rectTransform = fruitButtonsList[i].GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector3(c * buttonWidth + buttonWidth * .5f, r * buttonHeight + buttonHeight * .5f, 0);
                fruitButtonsList[i].GetComponent<FruitButton>().Init(c == strawberryIndex, c == 0 && r == 0, r == 0);
                fruitButtonsList[i].SetActive(true);
            }
        }
        Globals.CurrentGameState = Globals.GameState.Ready;
        Globals.CurrentScore = 0;
        HUDScore.GetComponent<TextMeshProUGUI>().text = Globals.CurrentScore.ToString();
    }

    public void StartGame()
    {
        audioSource.PlayOneShot(BlipSound, 1f);
        Globals.CurrentGameState = Globals.GameState.Playing;
    }

    public void IncrementScore()
    {
        Globals.CurrentScore++;
        HUDScore.GetComponent<TextMeshProUGUI>().text = Globals.CurrentScore.ToString();
    }

    public void GameOver(bool hitBanana)
    {
        HUDGameOverReason.GetComponent<TextMeshProUGUI>().text = hitBanana ? "that's a banana!" : "you missed a strawberry!";
        HUDGameOverReasonBack.GetComponent<TextMeshProUGUI>().text = HUDGameOverReason.GetComponent<TextMeshProUGUI>().text;
        HUDGameOverReason.GetComponent<TextMeshProUGUI>().color = hitBanana ? new Color(255f/255f, 255f/255f, 0) : new Color(255f/255f, 0, 0);
        if (Globals.CurrentScore > Globals.BestScore)
        {
            Globals.BestScore = Globals.CurrentScore;
            Globals.SaveToPlayerPrefs(Globals.BestScorePlayerPrefsKey, Globals.BestScore);
        }

        HUDFinalScore.GetComponent<TextMeshProUGUI>().text = "YOUR SCORE: " + Globals.CurrentScore.ToString();
        HUDTopScore.GetComponent<TextMeshProUGUI>().text = "BEST SCORE: " + Globals.BestScore.ToString();
        HUDGameOver.GetComponent<MoveNormal>().MoveDown();
        gameOverTimer = gameOverTimerMax;
        Globals.CurrentGameState = Globals.GameState.Dead;
    }
}
