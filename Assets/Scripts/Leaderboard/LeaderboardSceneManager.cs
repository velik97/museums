using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderboardSceneManager : MonoBehaviour
{
    public LeaderBoardUI leaderBoardUI;
    public Location initialLocaion;

    public string initialSceneName;

    private void Awake()
    {
        WaveGameSetter.SubscribeOnWave(ShwoLeaderboardsOnWall, 0);
        WaveGameSetter.SubscribeOnWave(DeleteNotFoundArtefacts, 2);
    }

    private void ShwoLeaderboardsOnWall()
    {
        if (GameInfo.Instance.location != Location.Tutorial)
        {
            leaderBoardUI.myScore =
                LocalNetworkLeaderboard.Instance.AddNewScore(
                    (int)GameInfo.Instance.location,
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

    private void DeleteNotFoundArtefacts()
    {
        Dictionary<int, FinalPickUp>.KeyCollection pickUpsInSceneKeys = FinalPickUp.PickUpByIndex.Keys;
        
        foreach (var key in pickUpsInSceneKeys)
        {
            if (!GameInfo.Instance.ArtefactCollected(key))
            {
                FinalPickUp.PickUpByIndex[key].gameObject.SetActive(false);
            }
        }        
    }

    public void RestartGame()
    {
        DontDestroyOnSceneLoad.DestroyAll();
        SceneManager.LoadScene(initialSceneName);
    }
}
