using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    //Mini Bosses event
    public delegate void MiniBoss(int miniBossID);
    public event MiniBoss OnMiniBoss;

    //Level roll event
    public delegate void LevelRolls(int rollResult);
    public event LevelRolls OnLevelRoll;

    //Final Boss trigger
    public delegate void FinalBoss();
    public event FinalBoss OnFinalBoss;

    // Tracking how many levels player has completed
    private int levelCounter = 0;
    private HashSet<int> visitedLevels = new HashSet<int>();

    //Trigger for mini bosses defeated
    public void TriggerMiniBoss(int miniBossID)
    {
        //Level Counter... obvi
        levelCounter++;

        //Trigger what happens when mini boss is defeated
        OnMiniBoss?.Invoke(miniBossID);

        if (levelCounter >= 2)
        {
            TriggerFinalBoss();
        }

        else
        {
            RollForNewLevel();
        }
    }

    private void RollForNewLevel()
    {
        int rollResult = Random.Range(1, 11);
        
        while (visitedLevels.Contains(rollResult))
        {
            rollResult = Random.Range(1, 11);
        }

        visitedLevels.Add(rollResult);
        TriggerLevelRoll(rollResult);
    }

    private void TriggerLevelRoll(int rollResult)
    {
        OnLevelRoll?.Invoke(rollResult);
    }

    private void TriggerFinalBoss()
    {
        OnFinalBoss?.Invoke();
    }
}
