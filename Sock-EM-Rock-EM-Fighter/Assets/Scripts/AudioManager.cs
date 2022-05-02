using System;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine;

//Class that stores the different audio files and allows scripts to play and stop them
[System.Serializable]
public class AudioManager : MonoBehaviour
{
    //Array of the Sound class to store multiple sounds
    public Sound[] sounds;
    
    void Awake()
    {
        //Loops through the sounds array and assigns all the corresponding values
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;

            s.source.loop = s.loop;
        }
    }

    //Function that will play a sound file based on the name passed into the function
    public void Play (string name)
    {
        //Loops through the sounds array until a Sound object is found with the same name 
        Sound s = Array.Find(sounds, sound => sound.name == name);
        //Plays the sound found in the sounds array
        s.source.Play();
    }

    //Function that will stop a sound file based on the name passed into the function
    public void Stop(string name)
    {
        //Loops through the sounds array until a Sound object is found with the same name 
        Sound s = Array.Find(sounds, sound => sound.name == name);
        //stops the sound found in the sounds array
        s.source.Stop();
    }
}
