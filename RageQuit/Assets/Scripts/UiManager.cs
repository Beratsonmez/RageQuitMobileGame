using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //Ekleme.

public class UiManager : MonoBehaviour
{

    public GameObject PausePanel;

    void Start()
    {
        
    }

   
    void Update()
    {
        
    }

    public void PauseGame()// burda pauseGame tuþunua basýldýgý zaman ki olaylar 
    {
        Time.timeScale = 0; // oyundaki akan süreyi ve hareketi keser.(karakterin bakýþyönü hariç)
        PausePanel.SetActive(true);//Tuþa basýldýgý zaman inaktive olan PausePaneli aktive eder.
    }

    public void ResumeGame()
    {
        Time.timeScale = 1; // oyundaki akan süreyi ve hareketi baslatir.
        PausePanel.SetActive(false); //Tuþa basýldýgý zaman aktive olan PausePaneli inaktive eder.
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");//Tuþa Basýldýgý zaman "" içindeki Scene Dönmesi için
        Time.timeScale = 1;
    }

    public void QuitGame()
    {
        Application.Quit();//Bunuda açýklamýyým aq.
    }
}
