using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animmal.NotificationSystem
{
    public class TextMeshSupportChecker : MonoBehaviour
    {
        public Sprite TextMeshTutorialSprite;

        void Start()
        {
#if !TMP_PRESENT
            StartCoroutine(ErrorCheck());
#endif
        }

        IEnumerator ErrorCheck()
        {
            yield return new WaitForEndOfFrame();
            NotificationData _ErrorData = new NotificationData();
            _ErrorData.Texts.Add("You Need to Enable TextMeshPRO Support First.");
            _ErrorData.Texts.Add("To enable text mesh pro support go to EDIT/PROJECT SETTINGS/PLAYER and add TMP_PRESENT; in Scripting Define Symbols field.");
            _ErrorData.Sprites.Add(TextMeshTutorialSprite);
            NotificationManager.Instance.ShowNotification("TextMeshError", _ErrorData);
        }

    }
}