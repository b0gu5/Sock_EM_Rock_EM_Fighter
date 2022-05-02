using UnityEngine.Audio;
using UnityEngine;

//Sound class that will store a single sound file and respective values for it
[System.Serializable]
public class Sound
{
    //Name variable to identify sound clip
    public string name;

    //Instance of audio clip class
    public AudioClip clip;

    //Float variable to determine how loud the sound is
    public float volume;
    //Boolean that determines if the sound will loop
    public bool loop;

    //Instance of the audio source class
    [HideInInspector]
    public AudioSource source;
}
