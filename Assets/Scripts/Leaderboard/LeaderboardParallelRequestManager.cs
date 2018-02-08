using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(LocalNetworkLeaderboard))]
public class LeaderboardParallelRequestManager : MonoSingleton<LeaderboardParallelRequestManager>
{
    private Queue<ScoreListRequestCallback> scoreListCallbackQueue;
    private Queue<Action> simpleCallbackQueue;

    private LocalNetworkLeaderboard leaderboard;

    #region Awake & Update
    
    private void Awake()
    {
        leaderboard = GetComponent<LocalNetworkLeaderboard>();
        scoreListCallbackQueue = new Queue<ScoreListRequestCallback>();
        simpleCallbackQueue = new Queue<Action>();
    }

    private void Update()
    {
        lock (scoreListCallbackQueue)
        {
            if (scoreListCallbackQueue != null && scoreListCallbackQueue.Count > 0)
                scoreListCallbackQueue.Dequeue().Invoke();
        }

        lock (simpleCallbackQueue)
        {
            if (simpleCallbackQueue != null && simpleCallbackQueue.Count > 0)
                simpleCallbackQueue.Dequeue().Invoke();
        }
    }
    
    #endregion
    
    #region Public methods
    
    public void SynchronizeScores(Action callback)
    {
        ThreadStart threadStart = delegate
        {
            SynchronizeScoresParallel(callback);
        };
        
        new Thread(threadStart).Start();
    }
    
    public void GetLocalScores(Action<List<Score>> callback)
    {
        ThreadStart threadStart = delegate
        {
            GetLocalSoresParallel(false, callback);
        };
        
        new Thread(threadStart).Start();
    }
    
    public void GetLocalScores(int locationId, Action<List<Score>> callback)
    {
        ThreadStart threadStart = delegate
        {
            GetLocalSoresParallel(false, locationId, callback);
        };
        
        new Thread(threadStart).Start();
    }
    
    public void GetLocalScores(bool sync, Action<List<Score>> callback)
    {
        ThreadStart threadStart = delegate
        {
            GetLocalSoresParallel(sync, callback);
        };
        
        new Thread(threadStart).Start();
    }
    
    public void GetLocalScores(bool sync, int locationId, Action<List<Score>> callback)
    {
        ThreadStart threadStart = delegate
        {
            GetLocalSoresParallel(sync, locationId, callback);
        };
        
        new Thread(threadStart).Start();
    }
    
    #endregion

    #region Private methods (threads)
    
    private void SynchronizeScoresParallel(Action callback)
    {
        leaderboard.SynchronizeScores();
        
        simpleCallbackQueue.Enqueue(callback);
    }

    private void GetLocalSoresParallel(bool sync, Action<List<Score>> callback)
    {
        if (sync)
            leaderboard.SynchronizeScores();
            
        List<Score> scores = leaderboard.GetLocalScores();
        
        scoreListCallbackQueue.Enqueue(new ScoreListRequestCallback(scores, callback));
    }
    
    private void GetLocalSoresParallel(bool sync, int locationId, Action<List<Score>> callback)
    {
        if (sync)
            leaderboard.SynchronizeScores();
        
        List<Score> scores = leaderboard.GetLocalScores(locationId);
        
        scoreListCallbackQueue.Enqueue(new ScoreListRequestCallback(scores, callback));
    }
    
    #endregion
}

public class ScoreListRequestCallback
{
    private List<Score> scores;
    private Action<List<Score>> callback;

    public ScoreListRequestCallback(List<Score> scores, Action<List<Score>> callback)
    {
        this.scores = scores;
        this.callback = callback;
    }

    public void Invoke()
    {
        callback.Invoke(scores);
    }
}
