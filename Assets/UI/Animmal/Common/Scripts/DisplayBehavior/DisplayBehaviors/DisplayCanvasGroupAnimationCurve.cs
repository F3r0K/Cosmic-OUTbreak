using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Animmal
{
    [AddComponentMenu("Animmal/DisplayBehavior/Display Animation Curve")]
    public class DisplayCanvasGroupAnimationCurve : DisplayCanvasGroupBase
    {
        public AnimationCurve ShowAnimationCurve = new AnimationCurve();
        public AnimationCurve HideAnimationCurve = new AnimationCurve();

        protected override float _ShownAlpha { get { return ShowAnimationCurve.Evaluate(GetAnimationCurveDuration(ShowAnimationCurve)); } }
        protected override float _HiddenAlpha { get { return HideAnimationCurve.Evaluate(GetAnimationCurveDuration(HideAnimationCurve)); } }
        protected override float _ShowDuration { get { return GetAnimationCurveDuration(ShowAnimationCurve); } }
        protected override float _HideDuration { get { return GetAnimationCurveDuration(HideAnimationCurve); } }


        protected virtual float GetAnimationCurveDuration (AnimationCurve _AnimationCurve)
        {
            if (_AnimationCurve.length == 0)
                return 0f;

            return _AnimationCurve[_AnimationCurve.length - 1].time;
        }
    }
}