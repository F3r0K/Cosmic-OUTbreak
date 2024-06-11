using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animmal.NotificationSystem
{
    public class UIItemSpawnHelperBase : MonoBehaviour
    {
        public NotificationDisplay _NotificationDisplay;
        public NotificationDisplay NotificationDisplay { get { if (_NotificationDisplay == null) _NotificationDisplay = GetComponent<NotificationDisplay>(); return _NotificationDisplay; } }

        [Tooltip("Delay update operation by one frame for complex ui cases.")]
        public bool DelayByOneFrame = true;

        protected WaitForEndOfFrame _WaitForEndOfFrame = new WaitForEndOfFrame();


        protected virtual void Awake()
        {
            if (NotificationDisplay != null)
                NotificationDisplay.OnItemShown.AddListener(ItemShown);
        }

        protected virtual void ItemShown(NotificationItem _NotificaionItem)
        {
            UpdateOperation();
        }

        public virtual void UpdateOperation()
        {
            if (DelayByOneFrame)
                StartCoroutine(DelayedRunUpdate());
            else
                RunUpdate();
        }

        protected virtual IEnumerator DelayedRunUpdate()
        {
            yield return _WaitForEndOfFrame;
            RunUpdate();
        }

        protected virtual void RunUpdate()
        {

        }
    }
}