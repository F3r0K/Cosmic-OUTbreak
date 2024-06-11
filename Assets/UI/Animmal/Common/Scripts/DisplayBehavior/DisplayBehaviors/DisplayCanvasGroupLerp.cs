using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Animmal
{
    [AddComponentMenu("Animmal/DisplayBehavior/Display Canvas Group Lerp")]
    public class DisplayCanvasGroupLerp : DisplayCanvasGroupBase
    {

        [Range(0, 1)]
        public float ShownAlpha = 1f;
        [Range(0, 1)]
        public float HiddenAlpha = 0f;
        [Min(0)]
        public float ShowDuration = 0.3f;
        [Min(0)]
        public float HideDuration = 0.3f;

        protected override float _ShownAlpha { get { return ShownAlpha; } }
        protected override float _HiddenAlpha { get { return HiddenAlpha; } }
        protected override float _ShowDuration { get { return ShowDuration; } }
        protected override float _HideDuration { get { return HideDuration; } }

    }
}