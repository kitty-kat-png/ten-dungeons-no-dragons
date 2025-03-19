namespace PubSub
{

    public interface ISubscriber<T> where T : BaseEvent
    {
        public void Subscribe();
        public void Unsubscribe();
        public void HandleEvent(T evt);
    }

}