using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Animmal
{
    public class DisplayCanvasGroupBase : DisplayBehaviorBase
    {
        
        public CanvasGroup _CanvasGroup;
        protected CanvasGroup CanvasGroup { get { if (_CanvasGroup == null) _CanvasGroup = GetComponent<CanvasGroup>(); return _CanvasGroup; } }
        
        [Tooltip("Set Corresponding Option On Canvas Group Component.")]
        public bool SetInteractible = true;
        [Tooltip("Set Corresponding Option On Canvas Group Component.")]
        public bool SetBlockRaycasts = true;
               

        //Override this in child class to supply custom Alpha and Duration calculations 
        protected virtual float _ShownAlpha { get { return 1f; } }

        protected virtual float _HiddenAlpha { get { return 1f; } }

        protected virtual float _ShowDuration { get { return 1f; } }

        protected virtual float _HideDuration { get { return 1f; } }


        protected virtual float TargetAlpha { get { return CurrentState == CURRENTSTATE.Showing ? _ShownAlpha : _HiddenAlpha; } }
        protected virtual float TargetDuration { get { return CurrentState == CURRENTSTATE.Showing ? _ShowDuration : _HideDuration; } }

        protected IEnumerator AnimationCoroutine;

        protected override void ShowItem(bool _Instant)
        {
            if (CanvasGroup == null)
            {
                Debug.LogError("Animmal Display Behavior: " + this.name + " on gameobject " + gameObject.name + "CanvasGroup not set.");
                return;
            }

            if (_Instant)
            {
                CanvasGroup.alpha = _ShownAlpha;
                if (SetInteractible)
                    CanvasGroup.interactable = true;
                if (SetBlockRaycasts)
                    CanvasGroup.blocksRaycasts = true;

                CurrentState = CURRENTSTATE.Idle;
            }
            else
            {
                CanvasGroup.alpha = _HiddenAlpha;
                CurrentState = CURRENTSTATE.Showing;
                Animate();
            }

            if (AutoHide)
                StartCoroutine(DelayedHide());
        }

        protected override void HideItem(bool _Instant)
        {
            if (CanvasGroup == null)
            {
                Debug.LogError("Animmal Display Behavior: " + this.name + " on gameobject " + gameObject.name + "CanvasGroup not set.");
                return;
            }

            if (_Instant)
            {
                CanvasGroup.alpha = _HiddenAlpha;
                if (SetInteractible)
                    CanvasGroup.interactable = false;
                if (SetBlockRaycasts)
                    CanvasGroup.blocksRaycasts = false;
                CurrentState = CURRENTSTATE.Idle;
                StartCoroutine(HidingFinishedDelayed());            
            }
            else
            {
                CurrentState = CURRENTSTATE.Hiding;
                Animate();
            }
        }

        protected virtual void Animate()
        {
            if (AnimationCoroutine != null)
                StopCoroutine(AnimationCoroutine);
            AnimationCoroutine = Animation();
            StartCoroutine(AnimationCoroutine);
        }


        protected virtual IEnumerator Animation()
        {
            //If not animating exit 
            if (CurrentState == CURRENTSTATE.Idle)
            {
                yield break;
            }

            //Find the iteration by which we must advance animation 
            float _Value = (Mathf.Abs(_ShownAlpha - _HiddenAlpha) / TargetDuration) * Time.deltaTime;

            //reverse value if we must lerp down 
            if (CanvasGroup.alpha > TargetAlpha)
                _Value *= -1;

            //Lerp towards target value 
            CanvasGroup.alpha += _Value;

            //If we reached our target exit 
            if ((CanvasGroup.alpha >= TargetAlpha && _Value > 0)
                || (CanvasGroup.alpha <= TargetAlpha && _Value < 0))
            {
                CanvasGroup.alpha = TargetAlpha;
                if (SetInteractible)
                    CanvasGroup.interactable = (CurrentState == CURRENTSTATE.Showing ? true : false);
                if (SetBlockRaycasts)
                    CanvasGroup.blocksRaycasts = (CurrentState == CURRENTSTATE.Showing ? true : false);

                if (CurrentState == CURRENTSTATE.Hiding)
                    StartCoroutine(HidingFinishedDelayed());
                CurrentState = CURRENTSTATE.Idle;

                yield break;
            }
            // Restart the Coroutine
            else
            {
                yield return new WaitForEndOfFrame();
                AnimationCoroutine = Animation();
                StartCoroutine(AnimationCoroutine);
            }

        }
    }
}