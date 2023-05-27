using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaybolanPlatform : MonoBehaviour
{
    public float disappearTime = 5f; // Platformun kaybolma süresi
    public float repairTime = 2f;
    private bool isDisappearing = false; // Platformun kaybolma durumu
    private float timer = 0f; // Zamanlayýcý

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Eðer oyuncu platforma temas ederse
        {
            isDisappearing = true; // Platformu kaybolma durumuna getir
            timer = 0f; // Zamanlayýcýyý sýfýrla
        }
    }

    private void Update()
    {
        if (isDisappearing)
        {
            timer += Time.deltaTime; // Zamanlayýcýyý güncelle

            if (timer >= disappearTime)
            {
                // Belirtilen süre kadar zaman geçtiðinde
                isDisappearing = false; // Kaybolma durumunu sýfýrla
                timer = 0f; // Zamanlayýcýyý sýfýrla
                gameObject.SetActive(false); // Platformu devre dýþý býrak
                Invoke("ReappearPlatform", repairTime); // 2 saniye sonra platformun geri gelmesini tetikle
            }
        }
    }

    private void ReappearPlatform()
    {
        gameObject.SetActive(true); // Platformu tekrar etkinleþtir
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player")) // Eðer oyuncu platforma temas ederse
        {
            isDisappearing = true; // Platformu kaybolma durumuna getir
            timer = 0f; // Zamanlayýcýyý sýfýrla
        }
    }
}
