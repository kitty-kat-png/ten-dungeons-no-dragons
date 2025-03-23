using UnityEngine;
using UnityEngine.Events;

public class OnTrigger : MonoBehaviour
{
    public UnityEvent OnEnter;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            OnEnter.Invoke();
        }
    }
}