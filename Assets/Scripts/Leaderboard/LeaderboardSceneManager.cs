using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderboardSceneManager : MonoBehaviour
{
    public LeaderBoardUI leaderBoardUI;
    public Location initialLocaion;

    public string initialSceneName;

    private void Start()
    {
        ShwoLeaderboardsOnWall();
    }

    private void ShwoLeaderboardsOnWall()
    {
        if (GameInfo.Instance.location != Location.Tutorial)
        {
            leaderBoardUI.myScore =
                LocalNetworkLeaderboard.Instance.AddNewScore(
                    (int)GameInfo.Instance.location,
                    GameInfo.Instance.computerId,
                    GameInfo.Instance.playerName,
                    GameInfo.Instance.points,
                    GameInfo.Instance.Status
                );
        }
        else
        {
            GameInfo.Instance.location = initialLocaion;
        }
        
        LeaderboardParallelRequestManager.Instance.GetLocalScores(
            true, (int)GameInfo.Instance.location, leaderBoardUI.ShowAllScores);
    }

    public void RestartGame()
    {
        DontDestroyOnSceneLoad.DestroyAll();
        SceneManager.LoadScene(initialSceneName);
    }
}
