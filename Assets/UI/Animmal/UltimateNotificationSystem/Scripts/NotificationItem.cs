using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Animmal.NotificationSystem
{
    [AddComponentMenu("Animmal/NotificationSystem/Notification Item")]
    /// <summary>
    /// Notification item is what is actually shown in UI
    /// </summary>
    public class NotificationItem : MonoBehaviour, IDisplayable
    {

        #region Variables 
        [Header("Data Processors")]
        [Tooltip("Add Objects here that have monobehavior component with INotificationDataProcessor interface to do global data processing on all Notification Data passed through this display")]
        public List<GameObject> DataProcessorGameobjects = new List<GameObject>();

        [Header("Events")]
        [Tooltip("Called when item show instruction is recieved. Use it to define what happens when item must be shown.")]
        public UnityBoolEvent OnShow = new UnityBoolEvent();
        [Tooltip("Called when item hide instruction is recieved. Use it to define what happens when item must be hidden.")]
        public UnityBoolEvent OnHide = new UnityBoolEvent();
        [Tooltip("Use this to decide what to do with data passed to this item. For example show it in Unity UI text objects and images.")]
        public UnityNotificationDataEvent OnDataAssign = new UnityNotificationDataEvent();
        [Tooltip("Called when item hiding animation has finished. Used by NotificiationDisplay script to know when its safe to disable and recycle an item.")]
        public UnityNotificationItemEvent OnHidingFinished = new UnityNotificationItemEvent();

        public NotificationStatus Status { get; set; }

        //Data processors 
        protected List<INotificationDataProcessor> DataProcessors = new List<INotificationDataProcessor>();
        
        public int StyleVariationID { get; set; } // We store refernce to Variation ID here

        //Implementing INotificationDisplayable Interface 
        public virtual UnityBoolEvent OnShowEvent { get { return OnShow; } }
        public virtual UnityBoolEvent OnHideEvent { get { return OnHide; } }

        protected bool InitComplete = false; 

        #endregion

        #region INIT
        
        protected virtual void Init()
        {
            //Only run this once 
            if (InitComplete == false)
            {
                InitComplete = true;
                InitializeDataProcessors();
            }
        }

        /// <summary>
        /// Get references to INotificationDataProcessors
        /// </summary>
        protected virtual void InitializeDataProcessors()
        {
            for (int i = 0; i < DataProcessorGameobjects.Count; i++)
            {
                if (DataProcessorGameobjects[i] == null)
                {
                    Debug.LogError("Ultimate Notification System - Notification Display: " + gameObject.name + " No gameobject assigned to DataProcessorGameobjects list at index " + i.ToString());
                    continue; // Skip this loop iteration 
                }

                INotificationDataProcessor _Data = DataProcessorGameobjects[i].GetComponent<INotificationDataProcessor>();
                if (_Data == null)
                {
                    Debug.LogError("Ultimate Notification System - Notification Manager: " + gameObject.name + " No INotificationDataProcessor Component found on gameobject " + DataProcessorGameobjects[i].name);
                    continue; // Skip this loop iteration 
                }

                DataProcessors.Add(_Data);
            }
        }
        #endregion

        #region ITEMDISPLAY

        /// <summary>
        /// Show item bypassing data 
        /// </summary>
        /// <param name="_Instant">Show item instantly?</param>
        public virtual void Show(bool _Instant)
        {
            OnShow.Invoke(_Instant);
        }

        /// <summary>
        /// Setup data and show item 
        /// </summary>
        /// <param name="_NotificationData"></param>
        /// <param name="_Instant"></param>
        public virtual void Show(NotificationStatus _Status, bool _Instant = false)
        {
            Status = _Status;

            //Initialize item 
            Init();

            //cashe style variation
            StyleVariationID = _Status.NotificationData.StyleVariationID;

            // Process data through assigned data processors
            _Status.NotificationData = ProcessData(_Status.NotificationData);

            // call On Data Assign so visual display components can populate UI
            OnDataAssign.Invoke(_Status.NotificationData);

            //Show item 
            Show(_Instant);
        }

        public virtual void Hide(bool _Instant = false)
        {
            OnHide.Invoke(_Instant);
        }

        /// <summary>
        /// Called when hiding process has finished, listened to by NotificationDisplay to handle item queuing 
        /// </summary>
        public virtual void HidingFinished()
        {
            OnHidingFinished.Invoke(this);
        }  

        #endregion

        #region HELPERS

        /// <summary>
        /// Process Data through any custom data processors assigned to Notification Manager
        /// </summary>
        /// <param name="_NotificationData">Data to process</param>
        /// <returns></returns>
        protected virtual NotificationData ProcessData(NotificationData _NotificationData)
        {
            for (int i = 0; i < DataProcessors.Count; i++)
            {
                if (DataProcessors[i] == null)
                {
                    Debug.LogError("Ultimate Notification System: Notification Manager - DataProcessor is null at index:" + i.ToString());
                }
                else
                {
                    _NotificationData = DataProcessors[i].GetProcessedData(_NotificationData);
                }
            }
            return _NotificationData;
        }

        #endregion

    }
}