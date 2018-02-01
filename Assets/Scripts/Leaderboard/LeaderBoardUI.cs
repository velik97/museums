using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardUI : MonoBehaviour
{
    public ScoreUI scoreUIPrefab;
    public int visiableTopCount = 3;
    public int visiableNearCount = 3;
    public Color playerHighlightColor;

    private List<Score> scores;

    public Score myScore;
    private int myPlace = -1;

    private int AllPositionsCount
    {
        get { return visiableNearCount + visiableTopCount + 1; }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ShowAllScores();
        }
    }

    public void ShowAllScores()
    {
        LocalNetworkLeaderboard.Instance.SynchronizeScores();
        scores = LocalNetworkLeaderboard.Instance.GetLocalScores(GameInfo.Instance.locationId);

        scores.Sort();
        float step = 1f / (float) AllPositionsCount;
        int place;
        ScoreUI scoreUi;

        if (myScore != null)
        {
            myPlace = scores.FindIndex(
                o => o.id == myScore.id && o.computerId == myScore.computerId && o.locationId == myScore.locationId);

            if (myPlace < AllPositionsCount)
            {
                for (place = 0; place < AllPositionsCount && place < scores.Count; place++)
                {
                    scoreUi = Instantiate(scoreUIPrefab, transform) as ScoreUI;
                    scoreUi.SetPoisition(step * (AllPositionsCount - 1 - place), step * (AllPositionsCount - place));

                    scoreUi.SetInfo(scores[place], place + 1, place == myPlace ? playerHighlightColor : Color.clear);
                }
            }
            else
            {
                for (place = 0; place < visiableTopCount; place++)
                {
                    scoreUi = Instantiate(scoreUIPrefab, transform) as ScoreUI;
                    scoreUi.SetPoisition(step * (AllPositionsCount - 1 - place), step * (AllPositionsCount - place));

                    scoreUi.SetInfo(scores[place], place + 1);
                }

                scoreUi = Instantiate(scoreUIPrefab, transform) as ScoreUI;
                scoreUi.SetPoisition(step * (AllPositionsCount - 1 - visiableTopCount),
                    step * (AllPositionsCount - visiableTopCount));

                scoreUi.SetEmpty();

                int lastShownNearPlace = Mathf.Clamp(myPlace + (visiableNearCount - 1), 0, scores.Count - 1);
                int firstShownNearPlace = lastShownNearPlace - (visiableNearCount - 1);

                int pos;
                for (place = firstShownNearPlace, pos = visiableTopCount + 1;
                    place < lastShownNearPlace + 1;
                    place++, pos++)
                {
                    scoreUi = Instantiate(scoreUIPrefab, transform) as ScoreUI;
                    scoreUi.SetPoisition(step * (AllPositionsCount - 1 - pos), step * (AllPositionsCount - pos));

                    scoreUi.SetInfo(scores[place], place + 1, place == myPlace ? playerHighlightColor : Color.clear);
                }
            }
        }
        else
        {
            for (place = 0; place < scores.Count && place < AllPositionsCount; place++)
            {
                scoreUi = Instantiate(scoreUIPrefab, transform) as ScoreUI;
                scoreUi.SetPoisition(step * (AllPositionsCount - 1 - place), step * (AllPositionsCount - place));

                scoreUi.SetInfo(scores[place], place + 1);
            }
        }
    }
    
}