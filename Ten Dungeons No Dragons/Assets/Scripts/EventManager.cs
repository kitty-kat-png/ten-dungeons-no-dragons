using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    //Mini Bosses event
    public UnityEvent<int> OnMiniBoss;

    //Level roll event
    public UnityEvent<int> OnLevelRoll;

    //Final Boss trigger
    public UnityEvent OnFinalBoss;

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
            TriggerFinalBoss(); //Trigger final boss when two levels have been defeated
        }

        else
        {
            RollForNewLevel(); //Otherwise, roll for new level
        }
    }

    public void ResetVisitedLevels()
    {
        visitedLevels.Clear();
        levelCounter = 0; //Not needed but added just in case
    }

    private void RollForNewLevel()
    {
        int rollResult = Random.Range(1, 11); //Rolling D10 for new level
        
        while (visitedLevels.Contains(rollResult)) //Ensure level hasn't been visited
        {
            rollResult = Random.Range(1, 11);
        }

        visitedLevels.Add(rollResult); //Marking the level as visited
        OnLevelRoll?.Invoke(rollResult); //Triggering event for rolling level
    }

    private void TriggerFinalBoss()
    {
        OnFinalBoss?.Invoke(); //Trigger final boss event
    }
}
