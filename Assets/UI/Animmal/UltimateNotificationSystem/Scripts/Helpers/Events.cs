using UnityEngine.Events;

namespace Animmal.NotificationSystem
{

    [System.Serializable]
    public class UnityNotificationItemEvent : UnityEvent<NotificationItem> { }
    [System.Serializable]
    public class UnityNotificationDataEvent : UnityEvent<NotificationData> { }
    public class UnityNotificationStatusEvent : UnityEvent<NotificationStatus> { }

    
}