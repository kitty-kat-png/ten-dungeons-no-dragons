using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{  
    public void StartGame()
    {
        Debug.Log("Starting game");
        SceneManager.LoadScene("Game"); //When the time comes, this will need to be MODIFIED so that it chooses a random level. I'll entrust this to the programmer's hands.
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game");
        Application.Quit();
    }

}
