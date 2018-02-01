using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

public class LocalNetworkLeaderboard : MonoSingleton<LocalNetworkLeaderboard>
{
    public string localLeaderboardFilePath = "C:\\net folder\\leaderboard.xml";
    private string remoteLeaderboardPath;

    private XDocument localLeaderboardXml;
    private XDocument remoteLeaderboardXml;

    #region Labels & Attributes

    private string rootLabel = "data";

    private string remotePathLabel = "remotePath";

    private string scoresLabel = "scores";
    private string scoreLabel = "score";

    private string computerIdAttribute = "computerId";
    private string locationIdAttribute = "loactionId";
    private string idAttribute = "id";

    private string nameLabel = "name";
    private string pointsLabel = "points";
    private string statusLabel = "status";

    #endregion

    private void Awake()
    {
        SynchronizeScores();
    }

    #region Public Methods

    /// <summary>
    /// Returns all scores listed on local computer
    /// </summary>
    public List<Score> GetLocalScores()
    {
        return GetScores(LocalLeaderboardXml);
    }
    
    /// <summary>
    /// Returns all scores listed on local computer for specific location
    /// </summary>
    public List<Score> GetLocalScores(int locationId)
    {
        return GetScores(LocalLeaderboardXml, locationId);
    }

    /// <summary>
    /// Synchronizes all local scores with remote scores
    /// </summary>
    public void SynchronizeScores()
    {
        if (LocalLeaderboardXml == null)
            return;

        remoteLeaderboardPath = LocalLeaderboardXml.Element(rootLabel).Element(remotePathLabel).Value;

        if (RemoteLeaderboardXml == null)
            return;

        List<Score> localScores = GetScores(LocalLeaderboardXml);
        List<Score> remoteScores = GetScores(RemoteLeaderboardXml);

        foreach (var remoteScore in remoteScores)
        {
            bool contains = false;
            foreach (var localScore in localScores)
            {
                if (localScore.computerId == remoteScore.computerId &&
                    localScore.id == remoteScore.id)
                {
                    contains = true;
                    break;
                }
            }

            if (!contains)
            {
                AddScoreToLocalLeaderboard(remoteScore);
            }
        }
        
        SaveLocalLeaderboard();
    }

    /// <summary>
    /// Adds new score to local leaderboard
    /// </summary>
    public Score AddNewScore(int locationId, int computerId, string playerName, int points, string status)
    {
        if (LocalLeaderboardXml == null)
            return null;

        List<Score> currentScores = GetScores(LocalLeaderboardXml);

        int maxId = 0;

        foreach (var currentScore in currentScores)
        {
            if (currentScore.locationId == locationId && currentScore.computerId == computerId
                && currentScore.id > maxId)
                maxId = currentScore.id;
        }

        int newId = maxId + 1;

        Score newScore = new Score(computerId, locationId, newId, playerName, points, status);

        AddScoreToLocalLeaderboard(newScore);
        SaveLocalLeaderboard();

        return newScore;
    }

    #endregion

    #region Private Methods

    private XDocument LocalLeaderboardXml
    {
        get
        {
            if (localLeaderboardXml == null)
            {
                try
                {
                    if (File.Exists(localLeaderboardFilePath))
                        localLeaderboardXml = XDocument.Load(localLeaderboardFilePath);
                    else
                    {
                        Debug.Log("[LeaderBoard] Can't find local leaderboard by path: \"" +
                                  localLeaderboardFilePath + "\". Creating new.");
                        localLeaderboardXml = CreateNewLocalXmlDocument();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                    localLeaderboardXml = CreateNewLocalXmlDocument();
                }
            }

            return localLeaderboardXml;
        }
    }

    private XDocument RemoteLeaderboardXml
    {
        get
        {
            if (remoteLeaderboardXml == null)
            {
                if (File.Exists(remoteLeaderboardPath))
                    remoteLeaderboardXml = XDocument.Load(remoteLeaderboardPath);
                else
                {
                    Debug.Log("[LeaderBoard] Can't find remote leaderboard by path: " +
                              remoteLeaderboardPath + ".");
                }
            }

            return remoteLeaderboardXml;
        }
    }

    private XDocument CreateNewLocalXmlDocument()
    {
        XDocument xDoc = new XDocument(
            new XElement(rootLabel,
                new XElement(remotePathLabel, remoteLeaderboardPath),
                new XElement(scoresLabel)
            )
        );

        xDoc.Save(localLeaderboardFilePath);

        return xDoc;
    }

    private List<Score> GetScores(XDocument xDoc, int locationId)
    {
        List<Score> scores = new List<Score>();
        foreach (var scoresElement in xDoc.Element(rootLabel).Element(scoresLabel).Elements(scoreLabel))
        {
            if (int.Parse(scoresElement.Attribute(locationIdAttribute).Value) != locationId)
                continue;
            
            Score newScore = new Score(
                int.Parse(scoresElement.Attribute(computerIdAttribute).Value),
                int.Parse(scoresElement.Attribute(locationIdAttribute).Value),
                int.Parse(scoresElement.Attribute(idAttribute).Value),
                scoresElement.Element(nameLabel).Value,
                int.Parse(scoresElement.Element(pointsLabel).Value),
                scoresElement.Element(statusLabel).Value
            );

            scores.Add(newScore);
        }
        return scores;
    }
    
    private List<Score> GetScores(XDocument xDoc)
    {
        List<Score> scores = new List<Score>();
        foreach (var scoresElement in xDoc.Element(rootLabel).Element(scoresLabel).Elements(scoreLabel))
        {
            Score newScore = new Score(
                int.Parse(scoresElement.Attribute(computerIdAttribute).Value),
                int.Parse(scoresElement.Attribute(locationIdAttribute).Value),
                int.Parse(scoresElement.Attribute(idAttribute).Value),
                scoresElement.Element(nameLabel).Value,
                int.Parse(scoresElement.Element(pointsLabel).Value),
                scoresElement.Element(statusLabel).Value
            );

            scores.Add(newScore);
        }
        return scores;
    }

    private void AddScoreToLocalLeaderboard(Score score)
    {
        if (LocalLeaderboardXml != null)
        {
            XElement newScore = new XElement(scoreLabel,
                new XAttribute(computerIdAttribute, score.computerId.ToString()),
                new XAttribute(locationIdAttribute, score.locationId.ToString()),
                new XAttribute(idAttribute,         score.id.ToString()),
                
                new XElement(  nameLabel,           score.name),
                new XElement(  pointsLabel,         score.points.ToString()),
                new XElement(  statusLabel,         score.status)
            );

            LocalLeaderboardXml.Element(rootLabel).Element(scoresLabel).Add(newScore);
        }
    }

    private void SaveLocalLeaderboard()
    {
        if (LocalLeaderboardXml != null)
            LocalLeaderboardXml.Save(localLeaderboardFilePath);
    }

    #endregion
}

public class Score : IComparable
{
    public int computerId;
    public int locationId;
    public int id;

    public string name;
    public int points;
    public string status;

    public Score(int computerId, int locationId, int id, string name, int points, string status)
    {
        this.computerId = computerId;
        this.locationId = locationId;
        this.id = id;

        this.name = name;
        this.points = points;
        this.status = status;
    }

    public int CompareTo(object obj)
    {
        if (obj == null)
            return 1;

        Score otherScore = (Score) obj;

        if (otherScore.locationId == this.locationId)
        {
            if (otherScore.points == this.points)
                return this.id - otherScore.id;

            return otherScore.points - this.points;
        }
        
        return this.locationId - otherScore.locationId;
    }

    public override string ToString()
    {
        return "computerId: " + computerId + ", locationId: " + locationId + ", id: " + id +
               "\nname: " + name + ", points: " + points + ", status: " + status;
    }
}