using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class SoundPack
{
    public AudioMixerGroup mixer;
    [Range(-3f, 3f)] public float pitch = 1f;
    public Vector2 pitchSpread;
    public Sound[] sounds;

    public float GetTruePitch()
    {
        return pitch + Random.Range(pitchSpread.x, pitchSpread.y);
    }

    public Sound GetRandomSound()
    {
        return sounds[Random.Range(0, sounds.Length)];
    }
}
