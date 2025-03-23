using UnityEngine;
using PubSub;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour, ISubscriber<PlayerDiedEvent>
{
    [SerializeField]
    private GameObject gameOverPanel;

    private void Awake()
    {
        Subscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    public void HandleEvent(PlayerDiedEvent evt)
    {
        gameOverPanel.SetActive(true);
    }

    public void Subscribe()
    {
        PubSub.PubSub.Instance.Subscribe<PlayerDiedEvent>(this);
    }

    public void Unsubscribe()
    {
        PubSub.PubSub.Instance.Unsubscribe<PlayerDiedEvent>(this);
    }

    public void OnMainMenuClicked()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
