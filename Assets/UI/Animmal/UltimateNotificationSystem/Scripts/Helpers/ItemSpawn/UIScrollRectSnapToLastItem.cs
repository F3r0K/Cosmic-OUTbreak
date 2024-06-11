using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Animmal.NotificationSystem
{
    [AddComponentMenu("Animmal/NotificationSystem/Helpers/UI Scroll Snap On New Notification Item")]
    public class UIScrollRectSnapToLastItem : UIItemSpawnHelperBase
    {
        #region VARIABLES       
        public ScrollRect _ScrollRect;
        public ScrollRect ScrollRect { get { if (_ScrollRect == null) _ScrollRect = GetComponent<ScrollRect>(); return _ScrollRect; } }

        [Range(0,1)]
        public float SnapValue;        
        public enum ScrollDirection { Horizontal, Vertical}
        public ScrollDirection ScrollRectDirection = ScrollDirection.Vertical;
        #endregion;

        protected override void RunUpdate()
        {
            if (ScrollRect != null)
            {
                if (ScrollRectDirection == ScrollDirection.Vertical)
                    _ScrollRect.verticalNormalizedPosition = SnapValue;
                else if (ScrollRectDirection == ScrollDirection.Horizontal)
                    _ScrollRect.horizontalNormalizedPosition = SnapValue;
            }
        }
    }
}