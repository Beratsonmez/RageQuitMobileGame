using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionManager : MonoBehaviour
{
    private void OnApplicationQuit()
    {
        SavePlayerPosition();
    }

    private void OnDisable()
    {
        SavePlayerPosition();
    }

    private void Start()
    {
        LoadPlayerPosition();
    }

    private void SavePlayerPosition()
    {
        PlayerPrefs.SetFloat("PlayerPositionX", transform.position.x);
        PlayerPrefs.SetFloat("PlayerPositionY", transform.position.y);
        PlayerPrefs.Save();
    }

    private void LoadPlayerPosition()
    {
        if (PlayerPrefs.HasKey("PlayerPositionX") && PlayerPrefs.HasKey("PlayerPositionY"))
        {
            float posX = PlayerPrefs.GetFloat("PlayerPositionX");
            float posY = PlayerPrefs.GetFloat("PlayerPositionY");
            transform.position = new Vector2(posX, posY);
        }
    }
}
