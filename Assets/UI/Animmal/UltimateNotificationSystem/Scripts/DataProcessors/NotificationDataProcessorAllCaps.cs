using System.Collections.Generic;
using UnityEngine;

namespace Animmal.NotificationSystem
{
    [AddComponentMenu("Animmal/NotificationSystem/DataProcessors/Text ALLCAPS")]
    public class NotificationDataProcessorAllCaps : MonoBehaviour, INotificationDataProcessor
    {
        public NotificationData GetProcessedData(NotificationData _Data)
        {
            for (int i = 0; i < _Data.Texts.Count; i++)
            {
                _Data.Texts[i] = _Data.Texts[i].ToUpper();
            }

            return _Data;
        }
    }
}