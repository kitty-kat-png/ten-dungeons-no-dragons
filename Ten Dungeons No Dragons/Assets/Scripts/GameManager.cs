using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int currentSceneIndex = 0;
    private bool isInFinalFight = false;
    private EventManager eventManager; //Referencing EventManager

    void Start()
    {
        LoadOpeningScene();

        eventManager = FindObjectOfType<EventManager>(); //Finding event manager in the scene
        eventManager.OnLevelRoll += RollToScene; //Subscribe to level roll event
        eventManager.OnMiniBoss += HandleMiniBossDefeated; //Subscribe to mini boss defeated event
        eventManager.OnFinalBoss += LoadFinalFight; //Subscribe to final boss event
    }

    void LoadOpeningScene()
    {
        SceneManager.LoadScene("OpeningScene");
    }

    public void RollD10()
    {
        if (!isInFinalFight)
        {
            int roll = Random.Range(1, 11);
            RollToScene(roll);
        }
    }

    private void RollToScene(int roll)
    {
        if (roll >= 1 && roll <= 10)
        {
            SceneManager.LoadScene("Scene" + roll);
            currentSceneIndex = roll;
        }
    }

    public void DefeatBossAndRoll()
    {
        if (currentSceneIndex < 10)
        {
            RollD10();
        }

        else
        {
            LoadFinalFight();
        }
    }

    private void HandleMiniBossDefeated(int miniBossID)
    {
        Debug.Log($"Mini boss {miniBossID} defeated");

        if (currentSceneIndex < 10)
        {
            RollD10();
        }

        else
        {
            LoadFinalFight();
        }
    }

    private void LoadFinalFight()
    {
        isInFinalFight = true;
        SceneManager.LoadScene("FinalFight");
    }

    private void OnDestroy()
    {
        eventManager.OnLevelRoll -= RollToScene;
        eventManager.OnMiniBoss -= HandleMiniBossDefeated;
        eventManager.OnFinalBoss -= LoadFinalFight;
    }
}
