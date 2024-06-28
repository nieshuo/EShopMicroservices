using MediatR;

namespace Ordering.Domain.Abstractions
{
    public class IDomainEvent:INotification
    {
        Guid EventId = Guid.NewGuid();
        public DateTime OccurredOn => DateTime.Now;

        // 从当前程序集中获取事件类型的限定名称
        public string EventType => GetType().AssemblyQualifiedName;
    }
}
