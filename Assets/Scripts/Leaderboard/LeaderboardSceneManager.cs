using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardSceneManager : MonoBehaviour
{
    public LeaderBoardUI leaderBoardUI;

    private void Start()
    {
        if (GameInfo.Instance.locationId != -1)
        {
            leaderBoardUI.myScore =
                LocalNetworkLeaderboard.Instance.AddNewScore(
                    GameInfo.Instance.locationId,
                    GameInfo.Instance.computerId,
                    GameInfo.Instance.playerName,
                    GameInfo.Instance.points,
                    GameInfo.Instance.Status
                );
        }
        else
        {
            GameInfo.Instance.locationId = 3;
        }

        leaderBoardUI.ShowAllScores();
    }
}
