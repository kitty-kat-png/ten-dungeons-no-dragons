using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class UpgradeItem : MonoBehaviour
{
    public UpgradeType upgrade;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            UpgradeManager.Instance.AddUpgradeItem(upgrade);
            PubSub.PubSub.Instance.PostEvent<UpgradePickedUp>(new UpgradePickedUp { upgradeType = upgrade });
        }

        Destroy(gameObject);
    }
}
