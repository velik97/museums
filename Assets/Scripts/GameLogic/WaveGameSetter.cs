using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class gives an opportunity to set different game logic in certain sequence instead of using Start and Awake
/// In any Monobehaviour in Awake() subscribe on a wave with certain number, to make sure, that logic is set in right sequnce 
/// </summary>
public class WaveGameSetter : MonoSingleton<WaveGameSetter>
{
    [Range(0, 10)]
    public int setWavesCount = 3;

    private UnityEvent[] setWaves;

    private UnityEvent[] SetWaves
    {
        get
        {
            if (setWaves == null)
                setWaves = new UnityEvent[setWavesCount];
            return setWaves;
        }
    }

    private void Start()
    {
        for (var i = 0; i < SetWaves.Length; i++)
        {
            if (SetWaves[i] != null)
            {
                SetWaves[i].Invoke();
                SetWaves[i].RemoveAllListeners();
            }
        }
    }

    public static void SubscribeOnWave(UnityAction setAction, int waveNum)
    {
        if (Instance.SetWaves.Length - 1 < waveNum)
        {
            Debug.LogError("[WaveGameSetter] Only " + Instance.setWavesCount + " waves are created." +
                           "But you are trying to subscribe on the wave " + waveNum);
            return;
        }
        if (waveNum < 0)
        {
            Debug.LogError("[WaveGameSetter] Wave number can't be negative: " + waveNum);
            return;
        }
        
        if (Instance.SetWaves[waveNum] == null)
            Instance.SetWaves[waveNum] = new UnityEvent();
        
        Instance.SetWaves[waveNum].AddListener(setAction);
    }
}
