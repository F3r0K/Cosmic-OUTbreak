using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animmal.NotificationSystem.FeatureDemos
{
    public class TutorialDemo : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(StartTutorial());
        }

        IEnumerator StartTutorial()
        {
            yield return new WaitForEndOfFrame();
            ShowTutorialOne();
        }

        private void ShowTutorialOne()
        {
           NotificationStatus _Status = NotificationManager.Instance.ShowNotification("ButtonAlert1", GetData("Tutorial One", "We are going to wait for callback from this Notification Item to learn when it's hidden to show next Tutorial"));
            _Status.StatusChangedEvent.AddListener(NotificationStatusChanged);
        }

        private void ShowTutorialTwo()
        {
            NotificationManager.Instance.ShowNotification("ButtonAlert2", GetData("Tutorial Two", "We listened to Tutorial One notification item callback to show this Notification when the first one was hidden. Check script on TutorialDemo gameobject to see code in action."));
        }

        private void NotificationStatusChanged(NotificationStatus _Status)
        {
            if (_Status.CurrentStatus == NotificationStatusEnum.Hidden)
            {
                _Status.StatusChangedEvent.RemoveListener(NotificationStatusChanged);
                ShowTutorialTwo();
            }            
        }

        private NotificationData GetData(string _Title, string _Content)
        {
            NotificationData _Data = new NotificationData();
            _Data.Texts.Add(_Title);
            _Data.Texts.Add(_Content);

            return _Data;
        }
    }
}