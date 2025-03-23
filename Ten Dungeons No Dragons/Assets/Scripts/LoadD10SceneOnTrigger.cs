using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadD10SceneOnTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            SceneManager.LoadScene("D10 Scene");
        }
    }
}