using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class HealthUI : MonoBehaviour
{
    public List<GameObject> heartObjects;

    public void HandleHealthChanged(int health)
    {
        for(int i=0; i<heartObjects.Count; i++)
        {
            heartObjects[i].SetActive(false);
        }

        for(int i=0;i<health; i++)
        {
            heartObjects[i].SetActive(true);
        }
    }
}