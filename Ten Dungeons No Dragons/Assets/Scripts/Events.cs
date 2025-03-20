using PubSub;

public class SomeEvent : BaseEvent
{
    public int somePayloadData;
}

public class UpgradePickedUp : BaseEvent
{
    public UpgradeType upgradeType;
}