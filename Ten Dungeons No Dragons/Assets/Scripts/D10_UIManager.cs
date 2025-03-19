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
        // replace this once we have a convention
        SceneManager.LoadScene("Testing");
    }
}
