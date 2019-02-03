using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    AudioSource audioSource;

    AudioClip music;
    AudioClip pickUpFood;
    AudioClip pickUpPoison;
    AudioClip pickUpShield;
    AudioClip destroyPoison;
    AudioClip activate;
    AudioClip gameOver;
    AudioClip complete;
    AudioClip pickupAppear;
    AudioClip pickupDisappear;

    void Awake() 
    {
        music = Resources.Load<AudioClip>("Audio/happy_light_loop");        
        pickUpFood = Resources.Load<AudioClip>("Audio/ClipsAccept1");
        pickUpPoison = Resources.Load<AudioClip>("Audio/DM-CGS-33");
        pickUpShield = Resources.Load<AudioClip>("Audio/DM-CGS-45");
        destroyPoison = Resources.Load<AudioClip>("Audio/DM-CGS-32");
        activate = Resources.Load<AudioClip>("Audio/Clicks_13");
        gameOver = Resources.Load<AudioClip>("Audio/Xylo_13");
        complete = Resources.Load<AudioClip>("Audio/Coin_Pick_Up_03");
        pickupAppear = Resources.Load<AudioClip>("Audio/BellishAccept6");
        pickupDisappear = Resources.Load<AudioClip>("Audio/Blips_14");

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

    public void PlayPickUpFoodFX() 
    {
        audioSource.PlayOneShot(pickUpFood);
    }

    public void PlayPickUpPoisonFX() 
    {
        audioSource.PlayOneShot(pickUpPoison);
    }

    public void PlayPickUpShieldFX() 
    {
        audioSource.PlayOneShot(pickUpShield);
    }

    public void PlayDestroyPoisonFX() 
    {
        audioSource.PlayOneShot(destroyPoison);
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

    public void PlayPickupAppearFX() 
    {
        audioSource.PlayOneShot(pickupAppear);
    }

    public void PlayPickupDisappearFX() 
    {
        audioSource.PlayOneShot(pickupDisappear);
    }
}
