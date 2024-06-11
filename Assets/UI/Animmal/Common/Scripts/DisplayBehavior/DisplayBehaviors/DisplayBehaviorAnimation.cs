using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animmal.NotificationSystem
{
    [AddComponentMenu("Animmal/DisplayBehavior/Display Legacy Animation")]    
    /// <summary>
    /// You must call HidingFinished from animation when hiding is finished
    /// </summary>
    
    public class DisplayBehaviorAnimation : DisplayBehaviorBase
    {
        public string ShowAnimation;
        public string HideAnimation;
        public string ShowInstantAnimation;
        public string HideInstantAnimation;

        public Animation _Animation;
        protected virtual Animation Animation { get { if (_Animation == null) _Animation = GetComponent<Animation>(); return _Animation; } }

        protected override void ShowItem(bool _Instant)
        {
            if (Animation == null)
            {
                Debug.LogError("Animmal Display Behavior: " + this.name + " on gameobject " + gameObject.name + "Legacy Animation Component not set.");
                return;
            }

            if (_Instant)
                Animation.Play(ShowInstantAnimation);
            else
                Animation.Play(ShowAnimation);

            if (AutoHide)
                StartCoroutine(DelayedHide());
        }

        protected override void HideItem(bool _Instant)
        {
            if (Animation == null)
            {
                Debug.LogError("Animmal Display Behavior: " + this.name + " on gameobject " + gameObject.name + "Legacy Animation Component not set.");
                return;
            }

            if (_Instant)
                Animation.Play(HideInstantAnimation);
            else
                Animation.Play(HideAnimation);
        }
    }
}