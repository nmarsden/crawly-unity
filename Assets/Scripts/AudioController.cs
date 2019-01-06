﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    AudioSource audioSource;

    AudioClip music;
    AudioClip pickUp;
    AudioClip activate;
    AudioClip gameOver;
    AudioClip complete;
    AudioClip foodAppear;
    AudioClip foodDisappear;

    void Awake() 
    {
        music = Resources.Load<AudioClip>("Audio/happy_light_loop");        
        pickUp = Resources.Load<AudioClip>("Audio/ClipsAccept1");        
        activate = Resources.Load<AudioClip>("Audio/Clicks_13");
        gameOver = Resources.Load<AudioClip>("Audio/Xylo_13");
        complete = Resources.Load<AudioClip>("Audio/Coin_Pick_Up_03");
        foodAppear = Resources.Load<AudioClip>("Audio/BellishAccept6");
        foodDisappear = Resources.Load<AudioClip>("Audio/Blips_14");

        gameObject.AddComponent<AudioSource>();
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.bypassEffects = true;
        audioSource.bypassListenerEffects = true;
        audioSource.bypassReverbZones = true;
        audioSource.clip = music;
        audioSource.loop = true;
    }

    public void PlayMusic() 
    {
        audioSource.Play();
    }

    public void StopMusic() 
    {
        audioSource.Stop();
    }

    public void PlayPickupFX() 
    {
        audioSource.PlayOneShot(pickUp);
    }

    public void PlayActivateFX() 
    {
        audioSource.PlayOneShot(activate);
    }

    public void PlayCompleteFX() 
    {
        audioSource.PlayOneShot(complete);
    }

    public void PlayGameOverFX() 
    {
        audioSource.PlayOneShot(gameOver);
    }

    public void PlayFoodAppearFX() 
    {
        audioSource.PlayOneShot(foodAppear);
    }

    public void PlayFoodDisappearFX() 
    {
        audioSource.PlayOneShot(foodDisappear);
    }

}
