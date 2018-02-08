using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfo : MonoSingleton<GameInfo>
{
    public string playerName = "";
    public int computerId = 1;

    public ArtefactDataBase dataBase;
    
    [HideInInspector] public Location location = Location.Моя_улица;
    [HideInInspector] public int points;
        
    [HideInInspector] public int completeBasketBonus;
    [HideInInspector] public int completeAllBasketsBonus;

    [HideInInspector] public List<Status> statuses;
    [HideInInspector] public List<ArtefactInfo> artefacts;
    [HideInInspector] public List<ArtefactInfo> trash;        

    private void Awake()
    {
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
}