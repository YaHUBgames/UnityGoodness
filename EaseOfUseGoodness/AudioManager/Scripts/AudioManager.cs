using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    [Header("Audio Settings")]
    [SerializeField] private GameObject audio3DPrefab;
    [SerializeField] private GameObject audio2DPrefab;

    [SerializeField] private AudioDictionary soundDictionary;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void PlayMonoSound(ESound soundType)
    {
        _instance.PlayMonoSoundLocal(soundType);
    }

    public static void PlayStereoSound(ESound soundType, Vector3 soundPosition, Transform soundParent = null)
    {
        _instance.PlayStereoSoundLocal(soundType, soundPosition, soundParent);
    }

    public void PlayMonoSoundLocal(ESound soundType)
    {
        PlaySoundLocal(soundType, transform.position, transform, false);
    }

    public void PlayStereoSoundLocal(ESound soundType, Vector3 soundPosition, Transform soundParent)
    {
        PlaySoundLocal(soundType, soundPosition, soundParent == null ? transform : soundParent, true);
    }

    public void PlaySoundLocal(ESound soundType, Vector3 soundPosition, Transform parent, bool is3D)
    {
        SoundPack soundPackToPlay = GetSoundPack(soundType);
        if (soundPackToPlay == null) return;
        Sound soundToPlay = soundPackToPlay.GetRandomSound();
        if (soundToPlay == null) return;

        GameObject audioPrefabInstance = Instantiate(is3D ? audio3DPrefab : audio2DPrefab, soundPosition, Quaternion.identity, parent);
        AudioSource source = audioPrefabInstance.GetComponent<AudioSource>();
        source.pitch = soundPackToPlay.GetTruePitch();
        source.volume = soundToPlay.volume;
        source.clip = soundToPlay.audioClip;
        source.outputAudioMixerGroup = soundPackToPlay.mixer;
        source.Play();

        // Rework this with pooling later
        Destroy(audioPrefabInstance, source.clip.length);
    }

    private SoundPack GetSoundPack(ESound soundType)
    {
        soundDictionary.TryGetValue(soundType, out SoundPack soundPack);
        if (soundPack == null)
        {
            print("No sound pack for \"" + System.Enum.GetName(typeof(ESound), soundType) + "\" found!");
            return null;
        }
        return soundPack;
    }

}
