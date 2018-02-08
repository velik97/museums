using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OperatorLeaderBoardUI : MonoBehaviour
{
    public ScoreUI scoreUIPrefab;
    public int visiableCount = 10;

    public GameObject leaderBoardObject;
    public Transform listTransform;

    public RectTransform loadBar;
    public Text header;

    public List<string> locationNames;
    public float waitbeforeNextTime;
    
    private List<ScoreUI> scoresUIs;
    private int showingLocationIndex = 1;

    private bool waitingForScoresLoading;

    private void Start()
    {
        waitingForScoresLoading = true;
        Show();
    }

    public void ChangeState()
    {
        if (leaderBoardObject.activeSelf)
            Hide();
        else
            Show();        
    }

    public void Show()
    {
        leaderBoardObject.SetActive(true);
        StartCoroutine(ShowScores());
    }

    public void Hide()
    {
        StopAllCoroutines();
        leaderBoardObject.SetActive(false);        
    }

    private IEnumerator ShowScores()
    {
        while (true)
        {
            header.text = locationNames[showingLocationIndex - 1];
            
            LeaderboardParallelRequestManager.Instance.GetLocalScores(true, showingLocationIndex, PrintScores);
            waitingForScoresLoading = true;
            
            while (waitingForScoresLoading)
            {
                yield return null;
            }
            
            float startTime = Time.time;
            float t = 0;
            
            loadBar.anchorMin = new Vector2(0f, 0f);
            loadBar.anchorMax = new Vector2(0f , 1f);
            loadBar.offsetMin = Vector2.zero;
            loadBar.offsetMax = Vector2.zero;
            
            while ((t = (Time.time - startTime) / waitbeforeNextTime) < 1f)                
            {
                loadBar.anchorMin = new Vector2(0f, 0f);
                loadBar.anchorMax = new Vector2(t , 1f);
                loadBar.offsetMin = Vector2.zero;
                loadBar.offsetMax = Vector2.zero;
                yield return null;
            }            
            loadBar.anchorMin = new Vector2(0f, 0f);
            loadBar.anchorMax = new Vector2(1f , 1f);
            loadBar.offsetMin = Vector2.zero;
            loadBar.offsetMax = Vector2.zero;
            
            showingLocationIndex %= 3;
            showingLocationIndex++;
        }
    }

    private void PrintScores(List<Score> scores)
    {
        scores.Sort();
        float step = 1f / (float) visiableCount;
        
        if (scoresUIs == null)
            scoresUIs = new List<ScoreUI>();

        for (int place = 0; place < visiableCount; place++)
        {
            ScoreUI scoreUi;
            if (scoresUIs.Count - 1 < place)
            {
                scoreUi = Instantiate(scoreUIPrefab, listTransform) as ScoreUI;
                scoresUIs.Add(scoreUi);
            }
            else
            {
                scoreUi = scoresUIs[place];
            }
            
            scoreUi.SetPoisition(step * (visiableCount - 1 - place), step * (visiableCount - place));
            
            if (place < scores.Count)
                scoreUi.SetInfo(scores[place], place + 1);
            else
                scoreUi.SetClear();
        }

        waitingForScoresLoading = false;
    }
}