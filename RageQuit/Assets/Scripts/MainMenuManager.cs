using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //Ekleme

public class MainMenuManager : MonoBehaviour
{
    public GameObject panel; //settings paneli i�in.

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Settings()
    {
        panel.GetComponent<Animator>().SetTrigger("Pop"); //Panel nesnesi animatoru yakalay�p onun i�inde bulunan Pop parametresine eri�iyo.
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");//Start tu�una bas�ld��� zaman parantez i�irisindeki Scene g�t�r�cek bizi yan� oyuna.
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
