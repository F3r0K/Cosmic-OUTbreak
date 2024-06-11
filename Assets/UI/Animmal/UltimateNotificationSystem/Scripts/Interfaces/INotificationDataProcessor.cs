using System.Collections.Generic;


namespace Animmal.NotificationSystem
{   
    /// <summary>
    /// Use this to make any changes you need to notification data before it is shown in item. 
    /// </summary>
    public interface INotificationDataProcessor
    {
        NotificationData GetProcessedData(NotificationData _Data);
    }
}