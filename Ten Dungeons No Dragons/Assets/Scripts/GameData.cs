using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameData
{
    public static GameData Instance;

    public static List<string> items = new List<string>();
    public static int totalLevelsCompleted = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(string item)
    {
        items.Add(item);
        Debug.Log($"Added {item} to inventory");
    }

    public bool HasItem(string item)
    {
        return items.Contains(item);
    }

    public void ClearInventory()
    {
        items.Clear();
        Debug.Log("Inventory cleared");
    }

    public static void AddLevelCompleted()
    {
        totalLevelsCompleted++;
        Debug.Log($"Total levels completed: {totalLevelsCompleted}");
    }

    public static void ResetLevelCompletion()
    {
        totalLevelsCompleted = 0;
        Debug.Log("Level completion reset");
    }
}
