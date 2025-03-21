using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private int levelID;

    private void Start()
    {
        levelID = SceneManager.GetActiveScene().buildIndex;
        InitalizeLevel();
    }

    void InitalizeLevel()
    {
        Debug.Log("Initalizing Level " + levelID);

        //Where I am going to set up level specific events like spawning enemies
    }

    void Update()
    {
        //Level specific events and player progress (when enemies are defeated)
    }

    public void EndLevel()
    {
        //Adding logic in order to end the levels once completed
        GameData.totalLevelsCompleted++;
    }
}
