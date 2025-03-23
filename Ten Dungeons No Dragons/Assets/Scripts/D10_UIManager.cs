using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class D10_UIManager : MonoBehaviour
{
    public TextMeshProUGUI d10Number;
    public Button continueButton;
    public Button rollButton;

    private int currentD10Number = -1;

    private void Awake()
    {
        continueButton.interactable = false;
        rollButton.interactable = true;
    }

    public void OnRollClicked()
    {
        currentD10Number = D10Manager.Instance.RollD10();
        d10Number.text = currentD10Number.ToString();
        continueButton.interactable = true;
        rollButton.interactable = false;
    }

    public void OnContinueClicked()
    {
        switch (currentD10Number / 2)
        {
            case 1:
            case 2:
                SceneManager.LoadScene("Forest Dungeon");
                break;
            case 3:
            case 4:
                SceneManager.LoadScene("Cave Dungeon");
                break;
            case 5:
            case 6:
                SceneManager.LoadScene("Dungeon Dungeon");
                break;
            case 7:
            case 8:
                SceneManager.LoadScene("Desert Dungeon");
                break;
            case 9:
            case 10:
                SceneManager.LoadScene("Underwater Dungeon");
                break;

        }
    }
}
