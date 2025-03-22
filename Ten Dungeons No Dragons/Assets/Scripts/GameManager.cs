using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int currentSceneIndex = 0;
    private bool isInFinalFight = false;
    private EventManager eventManager; //Referencing EventManager

    private HashSet<int> visitedLevels = new HashSet<int>();

    void Start()
    {
        LoadOpeningScene();

        eventManager = FindObjectOfType<EventManager>();

        eventManager.OnLevelRoll.AddListener(RollToScene); //Subscribe to level roll event
        eventManager.OnMiniBoss.AddListener(HandleMiniBossDefeated); //Subscribe to mini boss defeated event
        eventManager.OnFinalBoss.AddListener(LoadFinalFight); //Subscribe to final boss event

        //Finding player health and subscribing to death event
        PlayerController playerController = FindObjectOfType<PlayerController>();
        playerController.OnDie.AddListener(HandlePlayerDeath);
    }

    void LoadOpeningScene()
    {
        SceneManager.LoadScene("OpeningScene");
    }

    public void RollD10()
    {
        if (!isInFinalFight) return;

        int roll = Random.Range(1, 11);
        RollToScene(roll);
    }

    private void RollToScene(int roll)
    {
        if (visitedLevels.Contains(roll))
        {
            RollD10();
            return;
        }

        visitedLevels.Add(roll);
        SceneManager.LoadScene("Scene" +  roll);
        currentSceneIndex = roll;
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

        PlayerController playerController = FindObjectOfType<PlayerController>();
        playerController.health = playerController.maxHealth;
    }

    void HandlePlayerDeath()
    {
        ResetGameState();
        eventManager.ResetVisitedLevels();
        SceneManager.LoadScene("GameOver");
    }

    private void ResetGameState()
    {
        visitedLevels.Clear();
        isInFinalFight = false;

        PlayerController playerController = FindObjectOfType<PlayerController>();
        playerController.health = playerController.maxHealth;
    }

    private void OnDestroy()
    {
        eventManager.OnLevelRoll.RemoveListener(RollToScene);
        eventManager.OnMiniBoss.RemoveListener(HandleMiniBossDefeated);
        eventManager.OnFinalBoss.RemoveListener(LoadFinalFight);
    }
}
