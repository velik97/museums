using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(LocalNetworkLeaderboard))]
public class LeaderboardParallelRequestManager : MonoSingleton<LeaderboardParallelRequestManager>
{
    private Queue<GenericRequestCallback<List<Score>>> scoreListCallbackQueue;
    private Queue<GenericRequestCallback<RemoteLeaderboardAccess>> remoteLeaderboardAccessCallbackQueue;
    private Queue<GenericRequestCallback<string>> stringCallbackQueue;
    private Queue<GenericRequestCallback<int>> intCallbackQueue;
    private Queue<Action> simpleCallbackQueue;

    private Queue<Exception> errors;

    private LocalNetworkLeaderboard leaderboard;

    #region Awake & Update

    private void Awake()
    {
        leaderboard = GetComponent<LocalNetworkLeaderboard>();

        scoreListCallbackQueue = new Queue<GenericRequestCallback<List<Score>>>();
        remoteLeaderboardAccessCallbackQueue = new Queue<GenericRequestCallback<RemoteLeaderboardAccess>>();
        stringCallbackQueue = new Queue<GenericRequestCallback<string>>();
        intCallbackQueue = new Queue<GenericRequestCallback<int>>();
        simpleCallbackQueue = new Queue<Action>();

        errors = new Queue<Exception>();
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

        lock (remoteLeaderboardAccessCallbackQueue)
        {
            if (remoteLeaderboardAccessCallbackQueue != null && remoteLeaderboardAccessCallbackQueue.Count > 0)
                remoteLeaderboardAccessCallbackQueue.Dequeue().Invoke();
        }

        lock (stringCallbackQueue)
        {
            if (stringCallbackQueue != null && stringCallbackQueue.Count > 0)
                stringCallbackQueue.Dequeue().Invoke();
        }

        lock (intCallbackQueue)
        {
            if (intCallbackQueue != null && intCallbackQueue.Count > 0)
                intCallbackQueue.Dequeue().Invoke();
        }

        lock (errors)
        {
            if (errors != null && errors.Count > 0)
                Debug.LogError(errors.Dequeue().ToString());
        }
    }

    #endregion

    #region Public methods

    public void SynchronizeScores(Action callback)
    {
        ThreadStart threadStart = delegate { SynchronizeScoresParallel(callback); };

        new Thread(threadStart).Start();
    }

    public void GetLocalScores(Action<List<Score>> callback)
    {
        ThreadStart threadStart = delegate { GetLocalSoresParallel(false, callback); };

        new Thread(threadStart).Start();
    }

    public void GetLocalScores(int locationId, Action<List<Score>> callback)
    {
        ThreadStart threadStart = delegate { GetLocalSoresParallel(false, locationId, callback); };

        new Thread(threadStart).Start();
    }

    public void GetLocalScores(bool sync, Action<List<Score>> callback)
    {
        ThreadStart threadStart = delegate { GetLocalSoresParallel(sync, callback); };

        new Thread(threadStart).Start();
    }

    public void GetLocalScores(bool sync, int locationId, Action<List<Score>> callback)
    {
        ThreadStart threadStart = delegate { GetLocalSoresParallel(sync, locationId, callback); };

        new Thread(threadStart).Start();
    }

    public void GetRemoteLeaderboardAccessCode(string assumedRemotePcName, int myComputerId,
        Action<RemoteLeaderboardAccess> callback)
    {
        ThreadStart threadStart = delegate
        {
            GetRemoteLeaderboardAccessCodeParallel(assumedRemotePcName, myComputerId, callback);
        };

        new Thread(threadStart).Start();
    }

    public void GetComputerId(Action<int> callback)
    {
        ThreadStart threadStart = delegate { GetComputerIdParallel(callback); };

        new Thread(threadStart).Start();
    }

    public void GetRemotePcName(Action<string> callback)
    {
        ThreadStart threadStart = delegate { GetRemotePcNameParallel(callback); };

        new Thread(threadStart).Start();
    }

    public void SetComputerId(int computerId)
    {
        ThreadStart threadStart = delegate { SetComputerIdParallel(computerId); };

        new Thread(threadStart).Start();
    }

    public void SetRemotePcName(string pcName)
    {
        ThreadStart threadStart = delegate { SetRemoterPcNameParallel(pcName); };

        new Thread(threadStart).Start();
    }

    #endregion

    #region Private methods (threads)

    private void SynchronizeScoresParallel(Action callback)
    {
        try
        {
            leaderboard.SynchronizeScores();

            simpleCallbackQueue.Enqueue(callback);
        }
        catch (Exception e)
        {
            errors.Enqueue(e);
        }
    }

    private void GetLocalSoresParallel(bool sync, Action<List<Score>> callback)
    {
        try
        {
            if (sync)
                leaderboard.SynchronizeScores();

            List<Score> scores = leaderboard.GetLocalScores();

            scoreListCallbackQueue.Enqueue(new GenericRequestCallback<List<Score>>(scores, callback));
        }
        catch (Exception e)
        {
            errors.Enqueue(e);
        }
    }

    private void GetLocalSoresParallel(bool sync, int locationId, Action<List<Score>> callback)
    {
        try
        {
            if (sync)
                leaderboard.SynchronizeScores();

            List<Score> scores = leaderboard.GetLocalScores(locationId);

            scoreListCallbackQueue.Enqueue(new GenericRequestCallback<List<Score>>(scores, callback));
        }
        catch (Exception e)
        {
            errors.Enqueue(e);
        }
    }

    private void GetRemoteLeaderboardAccessCodeParallel(string assumedRemotePcName, int myComputerId,
        Action<RemoteLeaderboardAccess> callback)
    {
        try
        {
            RemoteLeaderboardAccess access =
                leaderboard.GetRemoteLeaderboardAccessCode(assumedRemotePcName, myComputerId);

            remoteLeaderboardAccessCallbackQueue.Enqueue(
                new GenericRequestCallback<RemoteLeaderboardAccess>(access, callback));
        }
        catch (Exception e)
        {
            errors.Enqueue(e);
        }
    }

    private void GetComputerIdParallel(Action<int> callback)
    {
        try
        {
            intCallbackQueue.Enqueue(new GenericRequestCallback<int>(leaderboard.ComputerId, callback));
        }
        catch (Exception e)
        {
            errors.Enqueue(e);
        }
    }

    private void GetRemotePcNameParallel(Action<string> callback)
    {
        try
        {
            stringCallbackQueue.Enqueue(new GenericRequestCallback<string>(leaderboard.RemotePcName, callback));
        }
        catch (Exception e)
        {
            errors.Enqueue(e);
        }
    }

    private void SetComputerIdParallel(int computerId)
    {
        try
        {
            leaderboard.ComputerId = computerId;
        }
        catch (Exception e)
        {
            errors.Enqueue(e);
        }
    }

    private void SetRemoterPcNameParallel(string pcName)
    {
        try
        {
            leaderboard.RemotePcName = pcName;
        }
        catch (Exception e)
        {
            errors.Enqueue(e);
        }
    }

    #endregion
}

public class GenericRequestCallback<T>
{
    private T data;
    private Action<T> callback;

    public GenericRequestCallback(T data, Action<T> callback)
    {
        this.data = data;
        this.callback = callback;
    }

    public void Invoke()
    {
        callback.Invoke(data);
    }
}