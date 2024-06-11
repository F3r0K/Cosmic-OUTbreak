using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CompassNavigatorPro {

    public class MiniMapInteraction : MonoBehaviour, IPointerClickHandler { // IPointerEnterHandler, IPointerExitHandler, , IPointerDownHandler, IPointerUpHandler {

        [NonSerialized]
        public CompassPro compass;

        public void OnPointerEnter(PointerEventData eventData) {
            if (compass != null) {
                compass.BubbleEvent(compass.OnMiniMapMouseEnter, eventData.position);
            }
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (compass != null) {
                compass.BubbleEvent(compass.OnMiniMapMouseExit, eventData.position);
            }
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (compass != null) {
                Vector2 position = eventData.position;
                RectTransform rect = GetComponent<RectTransform>();
                Rect screenRect = rect.GetScreenRect();

                Vector2 uv;
                uv.x = (eventData.position.x - screenRect.xMin) / screenRect.width;
                uv.y = (eventData.position.y - screenRect.yMin) / screenRect.height;

                Vector3 worldPos = compass.GetMiniMapWorldPositionFromUV(uv);
                compass.BubbleEvent(compass.OnMiniMapMouseClick, worldPos);
            }
        }

        //public void OnPointerDown(PointerEventData eventData) {
        //}

        //public void OnPointerUp(PointerEventData eventData) {
        //}


    }


}