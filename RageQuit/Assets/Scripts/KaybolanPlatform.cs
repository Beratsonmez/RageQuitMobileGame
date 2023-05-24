using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaybolanPlatform : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private ParticleSystem particleSystem;
    private bool isDisappearing;

    public float kaybolmaSuresi = 2f; // Kaybolma süresi
    public float tekrarGelmeSuresi = 2f; // Geri gelme süresi

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        particleSystem = GetComponent<ParticleSystem>();
        isDisappearing = false;
        InvokeRepeating("ToggleDisappear", kaybolmaSuresi, kaybolmaSuresi + tekrarGelmeSuresi);
    }

    void ToggleDisappear()
    {
        isDisappearing = !isDisappearing;

        if (isDisappearing)
        {
            spriteRenderer.enabled = false;
            particleSystem.Stop(); // Particle System'ý durdur
            GetComponent<BoxCollider2D>().enabled = false;
        }
        else
        {
            spriteRenderer.enabled = true;
            particleSystem.Play(); // Particle System'ý baþlat
            GetComponent<BoxCollider2D>().enabled = true;
        }
    }
}
