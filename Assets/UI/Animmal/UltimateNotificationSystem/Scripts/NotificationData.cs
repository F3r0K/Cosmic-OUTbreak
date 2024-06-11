using System.Collections.Generic;
using UnityEngine;

namespace Animmal.NotificationSystem
{
    [System.Serializable]
    public partial class NotificationData
    {
        public int StyleVariationID = 0;
        public List<string> Texts = new List<string>();
        public List<Sprite> Sprites = new List<Sprite>();
    }
}