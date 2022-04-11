using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FruitButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    Image BackgroundImage;
    [SerializeField]
    GameObject FruitImage;
    [SerializeField]
    Sprite BananaSprite;
    [SerializeField]
    Sprite StrawberrySprite;
    [SerializeField]
    GameObject Text;

    [SerializeField]
    AudioClip CollectSound;
    [SerializeField]
    AudioClip ErrorSound;

    AudioSource audioSource;

    public bool isStrawberry = true;
    public bool isStart = false;
    public bool isStartRow = false;
    public bool collected = false;

    SceneManager sceneManager;

    void Awake()
    {
        audioSource = GameObject.Find("SceneManager").GetComponent<AudioSource>();
        sceneManager = GameObject.Find("SceneManager").GetComponent<SceneManager>();
    }

    public void Init(bool _isStrawberry, bool _isStart, bool _isStartRow)
    {
        isStrawberry = !_isStartRow && _isStrawberry;
        isStart = _isStart;
        Text.SetActive(isStart);
        isStartRow = _isStartRow;
        FruitImage.SetActive(!isStartRow);
        FruitImage.GetComponent<Image>().sprite = isStrawberry ? StrawberrySprite : BananaSprite;
        BackgroundImage.color = new Color(255f/255f, 255f/255f, 255f/255f);
        collected = false;
    }

    // Detect if a click occurs
    public void OnPointerDown(PointerEventData eventData)
    {
        SelectFruitButton();
    }

    public void SelectFruitButton()
    {
        if (Globals.CurrentGameState == Globals.GameState.Ready && isStart)
        {
            Text.SetActive(false);
            BackgroundImage.color = new Color(200f/255f, 200f/255f, 200f/255f);
            sceneManager.StartGame();
        }
        else if (Globals.CurrentGameState == Globals.GameState.Playing && !isStartRow && !collected)
        {
            if (isStrawberry)
            {
                FruitImage.SetActive(false);
                audioSource.PlayOneShot(CollectSound, 1f);
                sceneManager.IncrementScore();
                BackgroundImage.color = new Color(200f/255f, 200f/255f, 200f/255f);
                collected = true;
            }
            else
            {
                SetIncorrect(true);
            }
        }
    }

    public void SetIncorrect(bool hitBanana)
    {
        BackgroundImage.color = new Color(255f/255f, 0/255f, 0/255f);
        audioSource.PlayOneShot(ErrorSound, 1f);
        sceneManager.GameOver(hitBanana);
    }
}
