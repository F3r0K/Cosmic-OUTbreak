using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animmal.NotificationSystem
{
    public enum NotificationStatusEnum { Skipped, Queued, Shown, Hidden }
    public partial class NotificationStatus 
    {
        protected NotificationStatusEnum _CurrentStatus;
        public NotificationStatusEnum CurrentStatus { get { return _CurrentStatus; } }
        public virtual NotificationItem NotificationItem { get; set; }
        public virtual NotificationData NotificationData { get; set; }

        /// <summary>
        /// This is called when notification status changes, NotificationItem will be null until Shown status is shown
        /// </summary>
        public UnityNotificationStatusEvent StatusChangedEvent = new UnityNotificationStatusEvent();

        public NotificationStatus(NotificationStatusEnum _Status, NotificationData _Data, NotificationItem _Item)
        {
            _CurrentStatus = _Status;
            NotificationData = _Data;
            NotificationItem = _Item;
        }

        public void SetStatus(NotificationStatusEnum _Status, bool _Silent)
        {
            _CurrentStatus = _Status;
            if (_Silent == false)
                StatusChangedEvent.Invoke(this);
        }
    }
}