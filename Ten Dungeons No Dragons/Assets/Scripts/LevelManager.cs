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
        SpawnEnemies();
    }

    void Update()
    {
        CheckLevelCompletion();
    }

    public void EndLevel()
    {
        Debug.Log("Level completed:" + levelID);
        //Adding logic in order to end the levels once completed
        GameData.AddLevelCompleted();
        GrantUpgrade();
        TransitionToNextLevel();
    }

    private void GrantUpgrade()
    {
        string upgrade = "Level upgrade:" + levelID;
        GameData.Instance.AddItem(upgrade);
    }

    private void TransitionToNextLevel()
    {
        if (levelID < 10)
        {
            GameManager.Instance.DefeatBossAndRoll();
        }

        else
        {
            SceneManager.LoadScene("GameWon");
        }
    }

    private void SpawnEnemies()
    {
        Debug.Log("Spawning enemies for level" + levelID);
    }

    private void CheckLevelCompletion()
    {
        if (EnemiesDefeated())
        {
            EndLevel();
        }
    }

    private bool EnemiesDefeated()
    {
        return true; //Placeholder at the moment
    }
}
