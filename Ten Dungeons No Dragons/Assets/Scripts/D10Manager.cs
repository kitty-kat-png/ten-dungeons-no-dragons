using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Singleton;

public class D10Manager : Singleton<D10Manager>
{
    public List<int> availableInts = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

    private void Awake()
    {
        SetInstance(this);
        DontDestroyOnLoad(this);
    }

    public int RollD10()
    {
        // plus one because Random.Range max value is exclusive for integer overload
        int num = Random.Range(1, availableInts.Count + 1);
        availableInts.Remove(num);
        return num;
    }
}
