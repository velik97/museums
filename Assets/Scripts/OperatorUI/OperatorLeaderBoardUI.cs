using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OperatorLeaderBoardUI : MonoBehaviour
{
    public ScoreUI scoreUIPrefab;
    public int visiableCount = 20;

    public GameObject leaderBoardObject;
    public List<Transform> listTransforms;

    public GameObject loadingImage;
    public List<Button> buttons;

    public SaveTableDialog saveTableDialog;
    public DeleteTableDialog deleteTableDialog;

    private ScoreUI[,] scoreUILists;

    public void ChangeState()
    {
        if (leaderBoardObject.activeSelf)
            Hide();
        else
            Open();
    }

    public void Open()
    {
        leaderBoardObject.SetActive(true);
        Show();
    }

    public void Hide()
    {
        leaderBoardObject.SetActive(false);
    }

    public void RequestClearScore()
    {
        deleteTableDialog.RequestDelete(-1, ClearScores);
    }
    
    public void RequestClearScore(int locationId)
    {
        deleteTableDialog.RequestDelete(locationId, delegate
        {
            ClearScores(locationId);
        });
    }

    private void ClearScores()
    {
        ShowLoading();
        LeaderboardParallelRequestManager.Instance.RemoveScores(Show);
    }

    private void ClearScores(int locationId)
    {
        ShowLoading();
        LeaderboardParallelRequestManager.Instance.RemoveScores(locationId, Show);
    }
    
    public void SaveScores()
    {
        saveTableDialog.RequestSave(-1);
    }

    public void SaveScores(int locationId)
    {
        saveTableDialog.RequestSave(locationId);
    }

    private void Show()
    {
        ShowLoading();
        LeaderboardParallelRequestManager.Instance.GetLocalScores(true, PrintScores);
    }

    private void PrintScores(List<Score> scores)
    {
        for (int locationId = 0; locationId < 3; locationId++)
        {
            PrintScores(scores.FindAll(o => o.locationId == locationId), locationId);
        }
        HideLoading();
    }

    private void PrintScores(List<Score> scores, int locationId)
    {
        scores.Sort();

        if (scoreUILists == null)
        {
            scoreUILists = new ScoreUI[3, visiableCount];


            for (var locId = 0; locId < 3; locId++)
            {
                for (var place = 0; place < visiableCount; place++)
                {
                    scoreUILists[locId, place] = Instantiate(scoreUIPrefab, listTransforms[locId]) as ScoreUI;
                }
            }
        }

        float step = 1f / (float) visiableCount;
        for (int place = 0; place < visiableCount; place++)
        {
            var scoreUi = scoreUILists[locationId, place];

            scoreUi.SetPoisition(step * (visiableCount - 1 - place), step * (visiableCount - place));

            if (place < scores.Count)
                scoreUi.SetInfo(scores[place], place + 1);
            else
                scoreUi.SetClear();
        }
    }

    private void ShowLoading()
    {
        foreach (var button in buttons)
        {
            button.interactable = false;
        }

        loadingImage.SetActive(true);
    }

    private void HideLoading()
    {
        foreach (var button in buttons)
        {
            button.interactable = true;
        }

        loadingImage.SetActive(false);
    }
}