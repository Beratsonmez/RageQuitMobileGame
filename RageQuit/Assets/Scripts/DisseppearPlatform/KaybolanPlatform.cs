using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaybolanPlatform : MonoBehaviour
{
    public float disappearTime = 5f; // Platformun kaybolma s�resi
    public float repairTime = 2f;
    private bool isDisappearing = false; // Platformun kaybolma durumu
    private float timer = 0f; // Zamanlay�c�

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // E�er oyuncu platforma temas ederse
        {
            isDisappearing = true; // Platformu kaybolma durumuna getir
            timer = 0f; // Zamanlay�c�y� s�f�rla
        }
    }

    private void Update()
    {
        if (isDisappearing)
        {
            timer += Time.deltaTime; // Zamanlay�c�y� g�ncelle

            if (timer >= disappearTime)
            {
                // Belirtilen s�re kadar zaman ge�ti�inde
                isDisappearing = false; // Kaybolma durumunu s�f�rla
                timer = 0f; // Zamanlay�c�y� s�f�rla
                gameObject.SetActive(false); // Platformu devre d��� b�rak
                Invoke("ReappearPlatform", repairTime); // 2 saniye sonra platformun geri gelmesini tetikle
            }
        }
    }

    private void ReappearPlatform()
    {
        gameObject.SetActive(true); // Platformu tekrar etkinle�tir
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player")) // E�er oyuncu platforma temas ederse
        {
            isDisappearing = true; // Platformu kaybolma durumuna getir
            timer = 0f; // Zamanlay�c�y� s�f�rla
        }
    }
}
