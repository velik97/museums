using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArtefactDataBase", menuName = "ArtefactDataBase", order = 1)]
public class ArtefactDataBase : ScriptableObject
{
    public List<ArtefactInfo> artefacts;

    public List<ArtefactInfo> trash;

    public List<Status> statuses;

    public int completeBasketBonus;
    public int completeAllBasketsBonus;

    [Header("Max points")]
    public int Моя_улица_maxPoints;
    public int Раскопки_maxPoints;
    public int Чердак_maxPoints;

    private void OnValidate()
    {
        for (var i = 0; i < artefacts.Count; i++)
        {
            artefacts[i].id = i;
        }
        
        for (var i = 0; i < trash.Count; i++)
        {
            trash[i].id = -i - 1;
        }

        List<ArtefactInfo> artefactInfos;
        
        Моя_улица_maxPoints = 0;
        artefactInfos = ArtefatsByLocation(Location.Моя_улица);
        for (var i = 0; i < artefactInfos.Count; i++)
        {
            Моя_улица_maxPoints += artefactInfos[i].points;
        }
        Моя_улица_maxPoints += completeBasketBonus * 3;
        Моя_улица_maxPoints += completeAllBasketsBonus;
        
        Раскопки_maxPoints = 0;
        artefactInfos = ArtefatsByLocation(Location.Раскопки);
        for (var i = 0; i < artefactInfos.Count; i++)
        {
            Раскопки_maxPoints += artefactInfos[i].points;
        }
        Раскопки_maxPoints += completeBasketBonus * 3;
        Раскопки_maxPoints += completeAllBasketsBonus;
        
        Чердак_maxPoints = 0;
        artefactInfos = ArtefatsByLocation(Location.Чердак);
        for (var i = 0; i < artefactInfos.Count; i++)
        {
            Чердак_maxPoints += artefactInfos[i].points;
        }
        Чердак_maxPoints += completeBasketBonus * 3;
        Чердак_maxPoints += completeAllBasketsBonus;
    }

    public List<ArtefactInfo> ArtefatsByLocation(Location location)
    {
        return artefacts.FindAll(o => o.location == location);
    }
    
    public List<ArtefactInfo> ArtefatsByMuseum(Museum museum)
    {
        return artefacts.FindAll(o => o.museum == museum);
    }
    
    public List<ArtefactInfo> ArtefatsByType(ArtefactType type)
    {
        return artefacts.FindAll(o => o.type == type);
    }
}

[System.Serializable]
public class ArtefactInfo
{
    public string name;
    
    public int id;
    public Location location;
    public Museum museum;
    
    public ArtefactType type;
    public string description;

    public int points;

    public ArtefactInfo(string name, int id, Location location, Museum museum, ArtefactType type, string description, int points)
    {
        this.name = name;
        this.id = id;
        this.location = location;
        this.museum = museum;
        this.type = type;
        this.description = description;
        this.points = points;
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

public enum ArtefactType
{
    Trash = -1,
    
    Small = 0,
    Medium = 1,
    Large = 2,
    Painting = 3
}

public enum Location
{
    Trash = -2,
    Tutorial = -1,
    
    Моя_улица = 0,
    Раскопки = 1,
    Чердак = 2
}

public enum Museum
{
    Trash = -1,
    
    Музей_космонавтки = 0,
    Музей_танка_Т_34 = 1,
    Музей_Цветаевой = 2,
    
    Дарвиновский_музей = 3,
    Дом_Гоголя = 4,
    Музей_Cкрябина = 5,
    
    Бородинская_битва = 6,
    Музей_имение_АС_Пушкина = 7,
    Музей_Булгакова = 8,
}