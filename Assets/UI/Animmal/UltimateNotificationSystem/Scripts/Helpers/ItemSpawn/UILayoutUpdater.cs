using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Animmal.NotificationSystem
{
    [AddComponentMenu("Animmal/NotificationSystem/Helpers/UI Layout Updater")]
    public partial class UILayoutUpdater : UIItemSpawnHelperBase
    {
        [Tooltip("What object to call update on")]
        public List<RectTransform> LayoutGameObjects;        
        
        protected override void RunUpdate()
        {
            for (int i = 0; i < LayoutGameObjects.Count; i++)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(LayoutGameObjects[i]);
            }            
        }
    }
}