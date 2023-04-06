using UnityEngine;

[System.Serializable]
public class Sound 
{
    public AudioClip audioClip;
    [Range(0f, 1f)] public float volume = 1f;
}
