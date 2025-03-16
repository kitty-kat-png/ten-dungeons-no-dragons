using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int currentSceneIndex = 0;
    private bool isInFinalFight = false;

    void Start()
    {
        LoadOpeningScene();
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

    private void LoadFinalFight()
    {
        isInFinalFight = true;
        SceneManager.LoadScene("FinalFight");
    }
}
