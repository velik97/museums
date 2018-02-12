using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

public class LocalNetworkLeaderboard : MonoSingleton<LocalNetworkLeaderboard>
{
    public string localLeaderboardFilePath = "C:\\net folder\\leaderboard.xml";
    private string remotePcName = "";

    private XDocument localLeaderboardXml;
    private XDocument remoteLeaderboardXml;

    private int computerId = -1;

    #region Labels & Attributes

    private const string RootLabel = "data";

    private const string RemotePcNameLabel = "remotePc";
    private const string ComputerIdLabel = "computerId";

    private const string ScoresLabel = "scores";
    private const string ScoreLabel = "score";

    private const string ComputerIdAttribute = "computerId";
    private const string LocationIdAttribute = "loactionId";
    private const string IdAttribute = "id";

    private const string NameLabel = "name";
    private const string PointsLabel = "points";
    private const string StatusLabel = "status";

    #endregion

    #region Properties

    public int ComputerId
    {
        get
        {
            if (computerId == -1)
            {
                computerId = int.Parse(LocalLeaderboardXml.Element(RootLabel).Element(ComputerIdLabel).Value);
            }
            return computerId;
        }
        set
        {
            LocalLeaderboardXml.Element(RootLabel).Element(ComputerIdLabel).Value = computerId.ToString();
            SaveLocalLeaderboard();
            computerId = value;
        }
    }

    public string RemotePcName
    {
        get
        {
            if (remotePcName == "")
            {
                remotePcName = LocalLeaderboardXml.Element(RootLabel).Element(RemotePcNameLabel).Value;
            }
            return remotePcName;
        }
        set
        {
            LocalLeaderboardXml.Element(RootLabel).Element(RemotePcNameLabel).Value = value;
            SaveLocalLeaderboard();
            remotePcName = value;
        }
    }

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
                catch
                {   
                    localLeaderboardXml = CreateNewLocalXmlDocument();
                    throw;                    
                }
            }

            return localLeaderboardXml;
        }
    }

    private XDocument RemoteLeaderboardXml
    {
        get
        {
            string path = "\\\\" + RemotePcName + "\\net folder\\leaderboard.xml";
            if (File.Exists(path))
                remoteLeaderboardXml = XDocument.Load(path);
            else
            {
                Debug.Log("[LeaderBoard] Can't find remote leaderboard by path: " +
                          path + ".");
            }

            return remoteLeaderboardXml;
        }
    }

    #endregion

    private void Awake()
    {
        remotePcName = "";
        computerId = -1;

//        AddNewScore(0, "velik90", 100, "good boy");
//        AddNewScore(0, "velik91", 101, "good boy1");
//        AddNewScore(1, "velik92", 102, "good boy2");
//        AddNewScore(1, "velik93", 103, "good boy3");
//        AddNewScore(2, "velik94", 104, "good boy4");
//        AddNewScore(2, "velik95", 105, "good boy5");
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

        if (RemoteLeaderboardXml == null)
            return;

        List<Score> localScores = GetScores(LocalLeaderboardXml);
        List<Score> remoteScores = GetScores(remoteLeaderboardXml);

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
    public Score AddNewScore(int locationId, string playerName, int points, string status)
    {
        if (LocalLeaderboardXml == null)
            return null;

        List<Score> currentScores = GetScores(LocalLeaderboardXml);

        int maxId = 0;

        foreach (var currentScore in currentScores)
        {
            if (currentScore.locationId == locationId && currentScore.computerId == ComputerId
                && currentScore.id > maxId)
                maxId = currentScore.id;
        }

        int newId = maxId + 1;

        Score newScore = new Score(ComputerId, locationId, newId, playerName, points, status);

        AddScoreToLocalLeaderboard(newScore);
        SaveLocalLeaderboard();

        return newScore;
    }

    /// <summary>
    /// Deletes all scores from local leaderboard
    /// </summary>
    public void ClearScores()
    {
        ClearScoresList();
        SaveLocalLeaderboard();
    }
    
    /// <summary>
    /// Deletes all scores with given locationId from local leaderboard
    /// </summary>
    public void ClearScores(int locationId)
    {
        ClearScoresList(locationId);
        SaveLocalLeaderboard();
    }

    /// <summary>
    /// Checks accessability of remote leaderboard for given remote PC name and my computerId
    /// </summary>
    public RemoteLeaderboardAccess GetRemoteLeaderboardAccessCode(string assumedRemotePcName, int myComputerId)
    {
        string path = "\\\\" + assumedRemotePcName + "\\net folder";

        if (!new DirectoryInfo(path).Exists)
            return RemoteLeaderboardAccess.NetFolderDoesntExistOrNotAccessible;

        path += "\\leaderboard.xml";

        if (!new FileInfo(path).Exists)
            return RemoteLeaderboardAccess.XmlDoesntExist;

        string prevRemotePcName = remotePcName;
        try
        {
            remotePcName = assumedRemotePcName;
            if (RemoteLeaderboardXml == null)
                return RemoteLeaderboardAccess.XmlIsNotAccessible;
        }
        catch
        {
            return RemoteLeaderboardAccess.BadRemoteXml;
        }
        remotePcName = prevRemotePcName;

        try
        {
            if (remoteLeaderboardXml.Element(RootLabel).Element(ComputerIdLabel).Value == myComputerId.ToString())
                return RemoteLeaderboardAccess.ComputerIdsAreSame;
        }
        catch
        {
            return RemoteLeaderboardAccess.BadRemoteXml;
        }

        return 0;
    }

    #endregion

    #region Private Methods    

    private XDocument CreateNewLocalXmlDocument()
    {
        XDocument xDoc = new XDocument(
            new XElement(RootLabel,
                new XElement(RemotePcNameLabel, remotePcName),
                new XElement(ComputerIdLabel, 1),
                new XElement(ScoresLabel)
            )
        );

        xDoc.Save(localLeaderboardFilePath);

        return xDoc;
    }

    private List<Score> GetScores(XDocument xDoc, int locationId)
    {
        List<Score> scores = new List<Score>();
        foreach (var scoresElement in xDoc.Element(RootLabel).Element(ScoresLabel).Elements(ScoreLabel))
        {
            if (int.Parse(scoresElement.Attribute(LocationIdAttribute).Value) != locationId)
                continue;

            Score newScore = new Score(
                int.Parse(scoresElement.Attribute(ComputerIdAttribute).Value),
                int.Parse(scoresElement.Attribute(LocationIdAttribute).Value),
                int.Parse(scoresElement.Attribute(IdAttribute).Value),
                scoresElement.Element(NameLabel).Value,
                int.Parse(scoresElement.Element(PointsLabel).Value),
                scoresElement.Element(StatusLabel).Value
            );

            scores.Add(newScore);
        }
        return scores;
    }

    private List<Score> GetScores(XDocument xDoc)
    {
        List<Score> scores = new List<Score>();
        foreach (var scoresElement in xDoc.Element(RootLabel).Element(ScoresLabel).Elements(ScoreLabel))
        {
            Score newScore = new Score(
                int.Parse(scoresElement.Attribute(ComputerIdAttribute).Value),
                int.Parse(scoresElement.Attribute(LocationIdAttribute).Value),
                int.Parse(scoresElement.Attribute(IdAttribute).Value),
                scoresElement.Element(NameLabel).Value,
                int.Parse(scoresElement.Element(PointsLabel).Value),
                scoresElement.Element(StatusLabel).Value
            );

            scores.Add(newScore);
        }
        return scores;
    }

    private void AddScoreToLocalLeaderboard(Score score)
    {
        if (LocalLeaderboardXml != null)
        {
            XElement newScore = new XElement(ScoreLabel,
                new XAttribute(ComputerIdAttribute, score.computerId.ToString()),
                new XAttribute(LocationIdAttribute, score.locationId.ToString()),
                new XAttribute(IdAttribute, score.id.ToString()),
                new XElement(NameLabel, score.name),
                new XElement(PointsLabel, score.points.ToString()),
                new XElement(StatusLabel, score.status)
            );

            LocalLeaderboardXml.Element(RootLabel).Element(ScoresLabel).Add(newScore);
        }
    }

    private void ClearScoresList()
    {
        LocalLeaderboardXml.Element(RootLabel).Element(ScoresLabel).RemoveAll();
    }
    
    private void ClearScoresList(int locationId)
    {
        foreach (var xElement in LocalLeaderboardXml.Element(RootLabel).Element(ScoresLabel).Elements(ScoreLabel))
        {
//            throw new Exception(xElement.Value);
            if (int.Parse(xElement.Attribute(LocationIdAttribute).Value) == locationId)
                xElement.Remove();
        }
    }

    private void SaveLocalLeaderboard()
    {
        lock (localLeaderboardXml)
        {
            if (LocalLeaderboardXml != null)
                localLeaderboardXml.Save(localLeaderboardFilePath);
        }
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

public enum RemoteLeaderboardAccess
{
    Accessible = 0,
    NetFolderDoesntExistOrNotAccessible = 1,
    XmlDoesntExist = 2,
    XmlIsNotAccessible = 3,
    BadRemoteXml = 4,
    ComputerIdsAreSame = 5
}