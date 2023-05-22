using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //Ekleme

public class MainMenuManager : MonoBehaviour
{
    public GameObject panel; //settings paneli için.

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Settings()
    {
        panel.GetComponent<Animator>().SetTrigger("Pop"); //Panel nesnesi animatoru yakalayýp onun içinde bulunan Pop parametresine eriþiyo.
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");//Start tuþuna basýldýðý zaman parantez içirisindeki Scene götürücek bizi yaný oyuna.
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
