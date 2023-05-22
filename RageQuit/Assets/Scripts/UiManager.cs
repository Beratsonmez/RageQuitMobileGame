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

    public void PauseGame()// burda pauseGame tu�unua bas�ld�g� zaman ki olaylar 
    {
        Time.timeScale = 0; // oyundaki akan s�reyi ve hareketi keser.(karakterin bak��y�n� hari�)
        PausePanel.SetActive(true);//Tu�a bas�ld�g� zaman inaktive olan PausePaneli aktive eder.
    }

    public void ResumeGame()
    {
        Time.timeScale = 1; // oyundaki akan s�reyi ve hareketi baslatir.
        PausePanel.SetActive(false); //Tu�a bas�ld�g� zaman aktive olan PausePaneli inaktive eder.
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");//Tu�a Bas�ld�g� zaman "" i�indeki Scene D�nmesi i�in
        Time.timeScale = 1;
    }

    public void QuitGame()
    {
        Application.Quit();//Bunuda a��klam�y�m aq.
    }
}
