using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public event Action<string> OnSoundEvent;

    public void TriggerSoundEvent(string soundName)
    {
        OnSoundEvent?.Invoke(soundName);
    }
}
