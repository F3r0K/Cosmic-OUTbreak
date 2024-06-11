using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animmal.NotificationSystem
{
    [AddComponentMenu("Animmal/NotificationSystem/Helpers/Notification Helper Component")]
    public partial class NotificationHelperComponent : MonoBehaviour
    {
        public string StyleID;
        public NotificationData NotificationData;

        public virtual void ShowNotification()
        {
            NotificationManager.Instance.ShowNotification(StyleID, Helpers.GetACopyOfNotificationData(NotificationData));
        }
    }
}