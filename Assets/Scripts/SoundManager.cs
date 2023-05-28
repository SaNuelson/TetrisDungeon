using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : Singleton<SoundManager>
{
    private AudioSource player;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip)
    {
        player.PlayOneShot(clip);
    }
}
