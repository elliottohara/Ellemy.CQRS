namespace Ellemy.CQRS.Event
{
    public interface IEventPublisher
    {
        void Publish();
    }
}