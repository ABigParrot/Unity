using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;

public static class SoundManager {

    public enum Sound
    {
        SnakeMove,
        SnakeDie,
        ButtonClick,
        ButtonOver,
        SnakeEat1,
        SnakeEat2,
        SnakeEat3,
        SnakeEat4,
        SnakeEat5,
        SnakeEat6,
    }

    public static void PlaySound(Sound sound)
    {
        //Creates sound game object named Sound
        GameObject soundGameObject = new GameObject("Sound");
        //Adds an audiosource to the Sound game object
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        //Plays the sound assigned to the enum once
        audioSource.PlayOneShot(GetAudioClip(sound));
        //Destroy the game object in 1.5 seconds
        GameObject.Destroy(soundGameObject, 2.5f);
 
    }

    private static AudioClip GetAudioClip(Sound sound)
    {
        //For each sound audio clip (of the class SoundAudioClip) inside the soundAudioClipArray inside the GameAssets
        foreach (GameAssets.SoundAudioClip soundAudioClip in GameAssets.i.soundAudioClipArray) {
            //Return the audio clip if the sound audio clip is equal to the sound from the enum
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip;
            }
        }
        Debug.LogError("Sound " + sound + " not found");
        return null;
    }

    //Extension method adds an AddButtonSounds to the Button_UI class without you having to modify the class itself. 
    //VERY useful
    public static void AddButtonSounds(this Button_UI buttonUI)
    {
        buttonUI.MouseOverOnceFunc += () => PlaySound(Sound.ButtonOver);
        buttonUI.ClickFunc += () => PlaySound(Sound.ButtonClick);
    }
    
}
