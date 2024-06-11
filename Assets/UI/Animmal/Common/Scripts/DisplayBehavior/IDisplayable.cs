using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animmal
{
    public interface IDisplayable 
    {
        UnityBoolEvent OnShowEvent { get; }
        UnityBoolEvent OnHideEvent { get; }
               

        void HidingFinished();
    }
}