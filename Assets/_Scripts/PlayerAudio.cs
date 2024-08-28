using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    AudioSource source;
    PlayerController controller;

    [SerializeField] AudioClip freefall_impact;
    [SerializeField] AudioClip hurt_sharp;
    [SerializeField] AudioClip land;
    [SerializeField] AudioClip[] jump;
    [SerializeField] AudioClip[] step;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        controller = GetComponentInParent<PlayerController>();

        controller.OnHurt += S_Hurt_Sharp;
        controller.OnJump += S_Jump;
    }

    public void S_Jump()
    {
        int i = Random.Range(0, jump.Length);

        source.pitch = Random.Range(0.8f, 1.3f);
        source.PlayOneShot(jump[i], 0.3f);
    }

    public void S_Step()
    {
        int i = Random.Range(0, step.Length);

        source.pitch = 1.5f;
        source.PlayOneShot(step[i], 0.3f);
    }

    public void S_Chrouch()
    {
        int i = Random.Range(0, step.Length);

        source.pitch = 0.8f;
        source.PlayOneShot(step[i], 0.2f);
    }


    public void S_Land()
    {
        source.pitch = Random.Range(0.9f, 1.8f);
        source.PlayOneShot(land, 0.3f);
    }

    public void S_Hurt_Sharp()
    {
        source.pitch = Random.Range(0.8f, 1.3f);
        source.PlayOneShot(hurt_sharp, 0.6f);
    }

    public void S_FreefallImpact()
    {
        source.pitch = Random.Range(0.8f, 1.2f);
        source.PlayOneShot(freefall_impact, 0.6f);
    }
}
