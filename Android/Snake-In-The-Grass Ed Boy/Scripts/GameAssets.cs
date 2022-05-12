/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class GameAssets : MonoBehaviour
{

    public static GameAssets i;

    private void Awake() {
        i = this;
    }
    
    public Sprite snakeHeadSprite;
    public Sprite snakeBodySprite;
    public Sprite foodSprite;

    public SoundAudioClip[] soundAudioClipArray;

    [Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }



}
