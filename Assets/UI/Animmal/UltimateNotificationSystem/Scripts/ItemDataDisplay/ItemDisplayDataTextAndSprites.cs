using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
#if TMP_PRESENT
using TMPro;
#endif

namespace Animmal.NotificationSystem
{
    [AddComponentMenu("Animmal/NotificationSystem/DataDisplay/Text And Sprites Unity UI")]    
    public class ItemDisplayDataTextAndSprites : ItemDisplayDataBase
    {
        #region VARIABLES
#if TMP_PRESENT
        public List<TextMeshProUGUI> TextMeshProTextItems = new List<TextMeshProUGUI>(); 
#endif
        public List<Text> TextItems = new List<Text>();

        public List<Image> Images = new List<Image>();

        public List<SpriteRenderer> SpriteRenderers = new List<SpriteRenderer>();

        #endregion

        protected override void DataAssigned(NotificationData _Data)
        {
            ShowTexts(_Data);
            ShowImages(_Data);
            ShowSprites(_Data);
        }

        #region DISPLAYDATA
        protected virtual void ShowTexts(NotificationData _Data)
        {
            for (int i = 0; i < _Data.Texts.Count; i++)
            {
#if TMP_PRESENT
                if (TextMeshProTextItems.Count > 0)
                {
                    if (i < TextMeshProTextItems.Count)
                    {
                        TextMeshProTextItems[i].text = _Data.Texts[i];
                    }
                    else
                    {
                        Debug.LogError("Ultimate Notification System: NotificationItem - " + gameObject.name + " does not have enough Text objects to display all Notification Data strings");
                    }
                }
                else
#endif
                if (TextItems.Count > 0)
                {
                    if (i < TextItems.Count)
                    {
                        TextItems[i].text = _Data.Texts[i];
                    }
                    else
                    {
                        Debug.LogError("Ultimate Notification System: NotificationItem - " + gameObject.name + " does not have enough Text objects to display all Notification Data strings");
                    }
                }
            }
        }

        protected virtual void ShowImages(NotificationData _Data)
        {
            for (int i = 0; i < _Data.Sprites.Count; i++)
            {
                if (i < Images.Count)
                {
                    Images[i].sprite = _Data.Sprites[i];
                }
                else if (Images.Count > 0)
                {
                    Debug.LogError("Ultimate Notification System: NotificationItem - " + gameObject.name + " does not have enough Images to display all Notification Data sprites");
                }
            }
        }

        protected virtual void ShowSprites(NotificationData _Data)
        {
            for (int i = 0; i < _Data.Sprites.Count; i++)
            {
                if (i < SpriteRenderers.Count)
                {
                    SpriteRenderers[i].sprite = _Data.Sprites[i];
                }
                else if (SpriteRenderers.Count > 0)
                {
                    Debug.LogError("Ultimate Notification System: NotificationItem - " + gameObject.name + " does not have enough Sprite Renderers to display all Notification Data sprites");
                }
            }
        }
        #endregion
    }
}