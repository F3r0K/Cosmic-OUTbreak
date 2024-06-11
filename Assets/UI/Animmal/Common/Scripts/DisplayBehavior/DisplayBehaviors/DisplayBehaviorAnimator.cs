using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animmal
{
    public enum ANIMATORPARAMETERTYPE { Animation, Float, Integer, Bool, Trigger}
      
    [System.Serializable]
    public partial class AnimatorParameter
    {
        public ANIMATORPARAMETERTYPE AnimatorParameterType;
        public string ParameterName;

        public int IntValue;
        public float FloatValue;
        public bool BoolValue;


        public virtual void ApplyParameter(Animator _Animator)
        {
            switch (AnimatorParameterType)
            {
                case ANIMATORPARAMETERTYPE.Animation:
                    _Animator.Play(ParameterName, 0, 0);
                    break;
                case ANIMATORPARAMETERTYPE.Float:
                    _Animator.SetFloat(ParameterName, FloatValue);
                    break;
                case ANIMATORPARAMETERTYPE.Integer:
                    _Animator.SetInteger(ParameterName, IntValue);
                    break;
                case ANIMATORPARAMETERTYPE.Bool:
                    _Animator.SetBool(ParameterName, BoolValue);
                    break;
                case ANIMATORPARAMETERTYPE.Trigger:
                    _Animator.SetTrigger(ParameterName);
                    break;
            }
        }
    }

    [AddComponentMenu("Animmal/DisplayBehavior/Display Animator")]
    /// <summary>
    /// You must call HidingFinished from animation when hiding is finished
    /// </summary>
    public partial class DisplayBehaviorAnimator : DisplayBehaviorBase
    {    
        public AnimatorParameter ShowAnimatorParameter;
        public AnimatorParameter HideAnimatorParameter;
        public AnimatorParameter ShowInstantAnimatorParameter;
        public AnimatorParameter HideInstantAnimatorParameter;

        public Animator _Animator;
        protected virtual Animator Animator { get { if (_Animator == null) _Animator = GetComponent<Animator>(); return _Animator; } }
        

        protected override void ShowItem(bool _Instant)
        {
            if (Animator == null)
            {
                Debug.LogError("Animmal DisplayBehavior: " + this.name + " on gameobject " + gameObject.name + "Animator not set.");
                return;
            }

            if (_Instant)
                ShowInstantAnimatorParameter.ApplyParameter(Animator);
            else
                ShowAnimatorParameter.ApplyParameter(Animator);

            if (AutoHide)
                StartCoroutine(DelayedHide());
        }

        protected override void HideItem(bool _Instant)
        {
            if (Animator == null)
            {
                Debug.LogError("Animmal DisplayBehavior: " + this.name + " on gameobject " + gameObject.name + "Animator not set.");
                return;
            }

            if (_Instant)
                HideInstantAnimatorParameter.ApplyParameter(Animator);
            else
                HideAnimatorParameter.ApplyParameter(Animator);
        }
    }
}