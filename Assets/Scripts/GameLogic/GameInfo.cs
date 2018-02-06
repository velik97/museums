using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfo : MonoSingleton<GameInfo>
{
    public string playerName = "";
    public int computerId = 1;
    public int locationId = 1;

    public int points;

    public List<Status> statuses;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public string Status
    {
        get
        {
            statuses.Sort();
            for (var i = 0; i < statuses.Count; i++)
            {
                if (points <= statuses[i].upperPoints)
                    return statuses[i].status;
            }

            return statuses[statuses.Count - 1].status;
        }
    }
}

[System.Serializable]
public class Status : IComparable
{
    public string status;
    public int upperPoints;

    public int CompareTo(object obj)
    {
        if (obj == null)
            return 0;
        
        Status otherStatus = (Status)obj;
        return this.upperPoints - otherStatus.upperPoints;
    }
}
