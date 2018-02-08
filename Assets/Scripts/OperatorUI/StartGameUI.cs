using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGameUI : MonoSingleton<StartGameUI>
{
    public InputField nameField;
    public InputField timeField;
    public Dropdown startTimeDropDown;
    public Button startButton;

    public Image loadingImage;
    public Image matchImage;
    public Image notMatchImage;

    public int minGameTime;
    public int maxGameTime;
    
    public float checkDuration = .4f;

    private List<Score> scores;

    [HideInInspector] public string finalPlayerName;
    private int gameMinutesTime;

    private string lastTimeStringInput;

    public int GameMinutesTime
    {
        get
        {
            if (gameMinutesTime == 0)
            {
                if (!PlayerPrefs.HasKey("GameTime"))
                {
                    gameMinutesTime = minGameTime;
                    PlayerPrefs.SetInt("GameTime", gameMinutesTime);
                }
                gameMinutesTime = PlayerPrefs.GetInt("GameTime");
            }
            return gameMinutesTime;
        }
        set
        {
            gameMinutesTime = value;
            PlayerPrefs.SetInt("GameTime", gameMinutesTime);
        }
    }

    public int StartTimeOption
    {
        get
        {
            int option;
            if (!PlayerPrefs.HasKey("StartTimeOptin"))
            {
                option = 0;
                PlayerPrefs.SetInt("StartTimeOptin", option);
            }
            else
            {
                option = PlayerPrefs.GetInt("StartTimeOptin");
            }
            return option;
        }
        set
        {
            PlayerPrefs.SetInt("StartTimeOptin", value);
        }
    }

    private void Start()
    {        
        startTimeDropDown.value = StartTimeOption;
        SetEmpty();
        
        startButton.onClick.AddListener(delegate
        {
            StartTimeOption = startTimeDropDown.value;
            GameManager.Instance.StartGame();
            gameObject.SetActive(false);
        });

        lastTimeStringInput = GameMinutesTime.ToString();
        timeField.text = lastTimeStringInput;
        
        timeField.onValueChanged.AddListener(CheckTimeFormat);
        timeField.onEndEdit.AddListener(CheckTimeBounds);
        
        LeaderboardParallelRequestManager.Instance.GetLocalScores(UpdateScores);
    }

    private void UpdateScores(List<Score> _scores)
    {
        scores = _scores;
        nameField.onValueChanged.AddListener(CheckName);
        if (nameField.textComponent.text != "")
            CheckName(nameField.textComponent.text);
    }

    private void CheckName(string playerName)
    {
        SetLoading();
        StopAllCoroutines();
        if (playerName == "")
        {
            SetEmpty();
            return;
        }
        StartCoroutine(WaitForCheck(playerName));
    }

    private void CheckTimeFormat(string timeString)
    {
        if (!int.TryParse(timeString, out gameMinutesTime))
            timeField.text = lastTimeStringInput;
        else
            lastTimeStringInput = timeString;
    }
    
    private void CheckTimeBounds(string timeString)
    {
        if (!int.TryParse(timeString, out gameMinutesTime))
        {
            timeField.text = lastTimeStringInput;
            gameMinutesTime = int.Parse(lastTimeStringInput);
        }

        gameMinutesTime = Mathf.Clamp(gameMinutesTime, minGameTime, maxGameTime);

        GameMinutesTime = gameMinutesTime;

        lastTimeStringInput = GameMinutesTime.ToString();
        timeField.text = lastTimeStringInput;
    }

    private IEnumerator WaitForCheck(string playerName)
    {
        yield return new WaitForSeconds(checkDuration);

        bool contains = false;
        
        foreach (var score in scores)
        {
            if (score.name == playerName)
            {
                contains = true;
                break;
            }                
        }

        if (contains)
            SetNotMatch();
        else
        {
            SetMatch();
            finalPlayerName = playerName;
        }        
    }

    private void SetLoading()
    {
        loadingImage.gameObject.SetActive(true);
        matchImage.gameObject.SetActive(false);
        notMatchImage.gameObject.SetActive(false);
        
        startButton.interactable = false;
    }
    
    private void SetMatch()
    {
        loadingImage.gameObject.SetActive(false);
        matchImage.gameObject.SetActive(true);
        notMatchImage.gameObject.SetActive(false);
        
        startButton.interactable = true;
    }
    
    private void SetNotMatch()
    {
        loadingImage.gameObject.SetActive(false);
        matchImage.gameObject.SetActive(false);
        notMatchImage.gameObject.SetActive(true);
        
        startButton.interactable = false;
    }

    private void SetEmpty()
    {
        loadingImage.gameObject.SetActive(false);
        matchImage.gameObject.SetActive(false);
        notMatchImage.gameObject.SetActive(false);
        
        startButton.interactable = false;
    }
}
