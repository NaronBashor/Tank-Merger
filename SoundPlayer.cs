using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClips;

    public AudioSource mainMenuMusic;

    private Dictionary<string, AudioClip> clipDictionary;
    private Dictionary<string, float> volumeDictionary;

    private void Awake()
    {
        clipDictionary = new Dictionary<string, AudioClip>();
        volumeDictionary = new Dictionary<string, float>();

        for (int i = 0; i < audioClips.Length; i++) {
            string clipName = audioClips[i].name;
            clipDictionary[clipName] = audioClips[i];

            switch (clipName) {
                case "shot":
                    volumeDictionary[clipName] = 0.25f;
                    break;
                case "game":
                    volumeDictionary[clipName] = 0.3f;
                    break;
                default:
                    volumeDictionary[clipName] = 1.0f;
                    break;
            }
        }
    }

    private void OnEnable()
    {
        if (SoundManager.Instance != null) {
            SoundManager.Instance.OnSoundEvent += PlaySound;
        } else {
            Debug.LogWarning("SoundManager instance is not available.");
        }
    }

    private void OnDisable()
    {
        if (SoundManager.Instance != null) {
            SoundManager.Instance.OnSoundEvent -= PlaySound;
        }
    }


    private void PlaySound(string soundName)
    {
        if (clipDictionary.TryGetValue(soundName, out AudioClip clip) && volumeDictionary.TryGetValue(soundName, out float volume)) {
            if (soundName == "game") {
                // Stop any currently playing clip on this AudioSource
                audioSource.Stop();

                // Set the clip, volume, and loop
                audioSource.clip = clip;
                audioSource.volume = volume;
                audioSource.loop = true;

                // Play the clip
                audioSource.Play();
            } else {
                // Play the clip once if it's not "game"
                audioSource.PlayOneShot(clip, volume);
            }
        } else {
            Debug.LogWarning("Sound not found: " + soundName);
        }
    }

}
