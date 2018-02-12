using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfo : MonoSingleton<GameInfo>
{
    public string playerName = "";

    public ArtefactDataBase dataBase;
    
    [HideInInspector] public Location location = Location.Моя_улица;
    [HideInInspector] public int points;
        
    [HideInInspector] public int completeBasketBonus;
    [HideInInspector] public int completeAllBasketsBonus;

    [HideInInspector] public List<Status> statuses;
    [HideInInspector] public List<ArtefactInfo> artefacts;
    [HideInInspector] public List<ArtefactInfo> trash;

    private List<int> collectedArtefactsIds;

    private void Awake()
    {
        collectedArtefactsIds = new List<int>();
        WaveGameSetter.SubscribeOnWave(SetInfo, 0);
    }

    private void SetInfo()
    {
        if (dataBase == null)
        {
            Debug.LogError("[GameInfo] Artefact Data Base is not Defined!");
            return;
        }

        completeBasketBonus = dataBase.completeBasketBonus;
        completeAllBasketsBonus = dataBase.completeAllBasketsBonus;

        statuses = dataBase.statuses;
        artefacts = dataBase.artefacts;
        trash = dataBase.trash;
    }

    public void FreeCollectedArtefactsIdsList()
    {
        if (collectedArtefactsIds != null && collectedArtefactsIds.Count != 0)
            collectedArtefactsIds.Clear();
    }

    public void OnArtefactCollecetd(int id)
    {
        collectedArtefactsIds.Add(id);
    }

    public bool ArtefactCollected(int id)
    {
        if (collectedArtefactsIds == null)
            return false;

        return collectedArtefactsIds.Contains(id);
    }
    
    public string Status
    {
        get
        {
            if (statuses == null || statuses.Count == 0)
                return "";

            statuses.Sort();
            for (var i = 0; i < statuses.Count; i++)
            {
                if (points <= statuses[i].upperPoints)
                    return statuses[i].status;
            }

            return statuses[statuses.Count - 1].status;
        }
    }

    public ArtefactInfo ArtefactById(int id)
    {
        if (id >= 0 && id < artefacts.Count)
            return artefacts.Find(o => o.id == id);
        
        if (id <= 0 && -id <= trash.Count)
            return trash.Find(o => o.id == id);
        
        Debug.LogError("[GameInfo]Database doesn't contain artefact with id " + id);
        
        return new ArtefactInfo("Неизвестное нечто", id, Location.Trash, Museum.Trash, ArtefactType.Trash, "...", 0);
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

    public static string MuseumNameByIndex(int index)
    {
        return MuseumNameByIndex((Museum) index);
    }
    
    public static string MuseumNameByIndex(Museum index)
    {
        switch (index)
        {
            case Museum.Trash:
                return "Мусор";
            case Museum.Музей_космонавтки:
                return "Музей космонавтки";
            case Museum.Музей_танка_Т_34:
                return "Музей танка Т 34";
            case Museum.Музей_Цветаевой:
                return "Музей Цветаевой";
            case Museum.Дарвиновский_музей:
                return "Дарвиновский музей";
            case Museum.Дом_Гоголя:
                return "Дом Гоголя";
            case Museum.Музей_Cкрябина:
                return "Музей Cкрябина";
            case Museum.Бородинская_битва:
                return "Бородинская битва";
            case Museum.Музей_имение_АС_Пушкина:
                return "Музей имение АС Пушкина";
            case Museum.Музей_Булгакова:
                return "Музей Булгакова";
            default:
                return "";
        }
    }

    public static string LocationByIndex(int index)
    {
        return LocationByIndex((Location) index);
    }

    public static string LocationByIndex(Location index)
    {
        switch (index)
        {
            case Location.Моя_улица:
                return "Моя улица";
            case Location.Раскопки:
                return "Раскопки";
            case Location.Чердак:
                return "Чердак";
            default:
                return "";
        }
    }
    
}