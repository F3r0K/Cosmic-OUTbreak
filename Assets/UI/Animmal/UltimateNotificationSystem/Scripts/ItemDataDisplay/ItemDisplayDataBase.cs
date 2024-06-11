using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animmal.NotificationSystem
{
    /// <summary>
    /// Make a child this class and override DataAssigned function to write your own custom code for showing notification data 
    /// </summary>
    public partial class ItemDisplayDataBase : MonoBehaviour
    {
        public NotificationItem _NotificationItem;
        protected NotificationItem NotificationItem { get { if (_NotificationItem == null) _NotificationItem = GetComponent<NotificationItem>(); return _NotificationItem; } }

        protected virtual void Awake()
        {
            Init();
        }

        protected virtual void Init()
        {
            if (NotificationItem == null)
            {
                Debug.LogError("Ultimate Notification System: " + this.name + " on gameobject " + gameObject.name + "NotificationItem not set.");
                return;
            }
            NotificationItem.OnDataAssign.AddListener(DataAssigned);
        }

        protected virtual void DataAssigned(NotificationData _Data)
        {
            
        }
    }
}