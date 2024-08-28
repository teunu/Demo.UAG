using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Health : MonoBehaviour
{
    Image[] health_icons;
    [SerializeField] float amplitude;

    [SerializeField] Sprite notch_a, notch_x;

    private void Awake()
    {
        health_icons = GetComponentsInChildren<Image>();

        if (PlayerController.instance)
            PlayerController.instance.OnHurt += OnHit;

        OnHit();
    }

    private void Update()
    {
        AnimateHealth();
    }

    void AnimateHealth() {
        float agitation = 10 - PlayerData.health * 2;
        const float offset = 20;

        for (int i = 0; i < health_icons.Length; i++) {
            float sine = 0;
            
            //If this is an alive notch, then animate it
            if (i < PlayerData.health)
                sine = Mathf.Sin((float)Time.frameCount / (PlayerData.health * offset) + i * offset);

            float y = sine * agitation * amplitude;
            health_icons[i].rectTransform.anchoredPosition = new Vector2(0, y);
        }
    }

    void OnHit()
    {
        for (int i = 0; i < health_icons.Length; i++)
        {
            health_icons[i].sprite = i < PlayerData.health ? notch_a : notch_x;
        }
    }

}
