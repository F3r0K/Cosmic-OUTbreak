using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animmal
{
    public partial class DisplayBehaviorBase : MonoBehaviour
    {
        public bool AutoHide;
        [Tooltip("Only used if AutoHide is toggled.")]
        [Min(0)]
        public float AutoHideTriggerDelay;

        [Tooltip("GameObject that has IDisplayable interface")]
        public GameObject DisplayableGameObject;
        public IDisplayable _Displayable;
        protected IDisplayable Displayable
        {
            get
            {
                if (_Displayable == null)
                {
                    _Displayable = DisplayableGameObject == null?  GetComponent<IDisplayable>() : DisplayableGameObject.GetComponent<IDisplayable>(); 
                }

                return _Displayable;
            }
        }

        public enum CURRENTSTATE { Idle, Showing, Hiding };
        public CURRENTSTATE CurrentState { get; set; }

        IEnumerator AutoHideCoroutine;

        protected virtual void Awake()
        {
            Init();
        }

        protected virtual void Init()
        {
            if (Displayable == null)
            {
                Debug.LogError("Animmal: " + this.name + " on gameobject " + gameObject.name + "Displayable not set.");
                return;
            }
            Displayable.OnShowEvent.AddListener(ShowItem);
            Displayable.OnHideEvent.AddListener(HideItem);
        }

        protected virtual void ShowItem(bool _Instant)
        {
            if (AutoHide)
            {
                if (AutoHideCoroutine != null)
                    StopCoroutine(AutoHideCoroutine);

                AutoHideCoroutine = DelayedHide();
                StartCoroutine(AutoHideCoroutine);
            }
        }

        protected IEnumerator DelayedHide()
        {
            yield return new WaitForSeconds(AutoHideTriggerDelay);
            HideItem(false);
        }

        protected virtual void HideItem(bool _Instant)
        {
            Displayable.HidingFinished();
        }

        public virtual IEnumerator HidingFinishedDelayed()
        {
            CurrentState = CURRENTSTATE.Idle;
            yield return new WaitForEndOfFrame();
            Displayable.HidingFinished();
        }
    }
}