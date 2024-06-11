using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animmal.NotificationSystem
{
    [AddComponentMenu("Animmal/NotificationSystem/Notification Manager")]
    /// <summary>
    /// Notification Manager is an optional interface for you to access different notification Displays. 
    /// </summary>
    public partial class NotificationManager : MonoBehaviour
    {

        #region VARIABLES
        [Tooltip("Don't destroy gameobject this component is attached to when loading new scenes.")]
        public bool PersistentObject = true;
        [Tooltip("Add Objects here that have monobehavior component with INotificationDataProcessor interface to do global data processing on all Notification Data passed through Manager")]
        public List<GameObject> DataProcessorGameobjects = new List<GameObject>();


        public static NotificationManager Instance;

        //Registered Displays
        protected Dictionary<string, NotificationDisplay> NotificationDisplays = new Dictionary<string, NotificationDisplay>();

        //DataProcessors found on supplied objects
        protected List<INotificationDataProcessor> DataProcessors = new List<INotificationDataProcessor>();

        #endregion

        #region UNITYFUNCTIONS
        protected virtual void Awake()
        {
            Init();
        }
        #endregion

        #region INITIALIZATION
        protected virtual void Init()
        {
            // We don't want two copies of Manager 
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            //Make sure gameobject is not destroyed when new scene is loaded
            if (PersistentObject)
                DontDestroyOnLoad(gameObject);

            //Set instant static parameter so we can easily access manager
            Instance = this;

            InitializeDataProcessors();
        }

        protected virtual void InitializeDataProcessors()
        {
            for (int i = 0; i < DataProcessorGameobjects.Count; i++)
            {
                if (DataProcessorGameobjects[i] == null)
                {
                    Debug.LogError("Ultimate Notification System - Notification Manager: No gameobject assigned to DataProcessorGameobjects list at index " + i.ToString());
                    continue; // Skip this loop iteration 
                }

                INotificationDataProcessor _Data = DataProcessorGameobjects[i].GetComponent<INotificationDataProcessor>();
                if (_Data == null)
                {
                    Debug.LogError("Ultimate Notification System - Notification Manager: No INotificationDataProcessor Component found on gameobject " + DataProcessorGameobjects[i].name);
                    continue; // Skip this loop iteration 
                }

                DataProcessors.Add(_Data);
            }
        }
        #endregion

        #region DISPLAYMANAGEMENT
        public virtual void RegisterDisplay(string _UniqueID, NotificationDisplay _Display)
        {
            if (NotificationDisplays.ContainsKey(_UniqueID))
                NotificationDisplays[_UniqueID] = _Display;
            else
                NotificationDisplays.Add(_UniqueID, _Display);
        }

        public virtual void UnRegisterDisplay(string _UniqueID)
        {
            if (NotificationDisplays.ContainsKey(_UniqueID))
                NotificationDisplays.Remove(_UniqueID);
        }
        #endregion

        #region NOTIFICATIONS
        public virtual NotificationStatus ShowNotification(string _StyleID, NotificationData _NotificationData)
        {
            ProcessData(_NotificationData);

            if (NotificationDisplays.ContainsKey(_StyleID))
            {
                return NotificationDisplays[_StyleID].ShowNotification(_NotificationData);
            }
            else
            {
                Debug.LogError("Ultimate Notification System: Notification Manager - Notification Display can't be found with UniqueID:" + _StyleID.ToString());
                return new NotificationStatus(NotificationStatusEnum.Skipped, null, null);
            }
        }

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

        #region BULKOPERATIONS
        public virtual void HideAll(bool _Instant = false)
        {
            foreach (var item in NotificationDisplays)
            {
                item.Value.HideAll(_Instant);
            }
        }

        public virtual void UnhideAll(bool _Instant = false)
        {
            foreach (var item in NotificationDisplays)
            {
                item.Value.UnhideAll(_Instant);
            }
        }

        public virtual void HideOfStyle(string _StyleID, bool _Instant = false)
        {
            if (NotificationDisplays.ContainsKey(_StyleID))
            {
                NotificationDisplays[_StyleID].HideAll(_Instant);
            }
            else
            {
                Debug.LogError("Ultimate Notification System: Notification Manager - Notification Display can't be found with UniqueID:" + _StyleID.ToString());
            }
        }

        public virtual void UnhideOfStyle(string _StyleID, bool _Instant = false)
        {
            if (NotificationDisplays.ContainsKey(_StyleID))
            {
                NotificationDisplays[_StyleID].UnhideAll(_Instant);
            }
            else
            {
                Debug.LogError("Ultimate Notification System: Notification Manager - Notification Display can't be found with UniqueID:" + _StyleID.ToString());
            }
        }

        public virtual void HideAllDisplays(bool _Instant = false)
        {
            foreach (var item in NotificationDisplays)
            {
                item.Value.HideDisplay(_Instant);
            }
        }

        public virtual void ShowAllDisplays(bool _Instant = false)
        {
            foreach (var item in NotificationDisplays)
            {
                item.Value.ShowDisplay(_Instant);
            }
        }

        public virtual void HideDisplayOfStyle(string _StyleID, bool _Instant = false)
        {
            if (NotificationDisplays.ContainsKey(_StyleID))
            {
                NotificationDisplays[_StyleID].HideDisplay(_Instant);
            }
            else
            {
                Debug.LogError("Ultimate Notification System: Notification Manager - Notification Display can't be found with UniqueID:" + _StyleID.ToString());
            }
        }

        public virtual void ShowDisplayOfStyle(string _StyleID, bool _Instant = false)
        {
            if (NotificationDisplays.ContainsKey(_StyleID))
            {
                NotificationDisplays[_StyleID].ShowDisplay(_Instant);
            }
            else
            {
                Debug.LogError("Ultimate Notification System: Notification Manager - Notification Display can't be found with UniqueID:" + _StyleID.ToString());
            }
        }

        /// <summary>
        /// Calls Hide function with Instant Hide set to true.
        /// </summary>
        public virtual void ClearAll()
        {
            foreach (var item in NotificationDisplays)
            {
                item.Value.HideAll(true);
            }
        }

        /// <summary>
        /// Calls Hide function with Instant Hide set to true.
        /// </summary>
        public virtual void ClearOfStyle(string _StyleID)
        {
            if (NotificationDisplays.ContainsKey(_StyleID))
            {
                NotificationDisplays[_StyleID].HideAll(true);
            }
            else
            {
                Debug.LogError("Ultimate Notification System: Notification Manager - Notification Display can't be found with UniqueID:" + _StyleID.ToString());
            }
        }

        /// <summary>
        /// Disabling will ignore any new notifications. 
        /// </summary>
        public virtual void DisableAll()
        {
            foreach (var item in NotificationDisplays)
            {
                item.Value.DisableDisplay();
            }
        }


        /// <summary>
        /// Enable so new notifications are not ignored. 
        /// </summary>
        public virtual void EnableAll()
        {
            foreach (var item in NotificationDisplays)
            {
                item.Value.EnableDisplay();
            }
        }

        /// <summary>
        /// Disabling will ignore any new notifications. 
        /// </summary>
        public virtual void DisableOfStyle(string _StyleID)
        {
            if (NotificationDisplays.ContainsKey(_StyleID))
            {
                NotificationDisplays[_StyleID].DisableDisplay();
            }
            else
            {
                Debug.LogError("Ultimate Notification System: Notification Manager - Notification Display can't be found with UniqueID:" + _StyleID.ToString());
            }
        }

        /// <summary>
        /// Enable so new notifications are not ignored. 
        /// </summary>
        public virtual void EnableOfStyle(string _StyleID)
        {
            if (NotificationDisplays.ContainsKey(_StyleID))
            {
                NotificationDisplays[_StyleID].EnableDisplay();
            }
            else
            {
                Debug.LogError("Ultimate Notification System: Notification Manager - Notification Display can't be found with UniqueID:" + _StyleID.ToString());
            }
        }

        /// <summary>
        /// Pausing will automatically queue up new notifications and will show them display is unpaused. 
        /// </summary>
        public virtual void PauseAll()
        {
            foreach (var item in NotificationDisplays)
            {
                item.Value.PauseDisplay();
            }
        }


        /// <summary>
        /// Pausing will automatically queue up new notifications and will show them display is unpaused. 
        /// </summary>
        public virtual void UnpauseAll()
        {
            foreach (var item in NotificationDisplays)
            {
                item.Value.UnpauseDisplay();
            }
        }


        /// <summary>
        /// Pausing will automatically queue up new notifications and will show them display is unpaused. 
        /// </summary>
        public virtual void PauseOfStyle(string _StyleID)
        {
            if (NotificationDisplays.ContainsKey(_StyleID))
            {
                NotificationDisplays[_StyleID].PauseDisplay();
            }
            else
            {
                Debug.LogError("Ultimate Notification System: Notification Manager - Notification Display can't be found with UniqueID:" + _StyleID.ToString());
            }
        }

        /// <summary>
        /// Pausing will automatically queue up new notifications and will show them display is unpaused. 
        /// </summary>
        public virtual void UnpauseOfStyle(string _StyleID)
        {
            if (NotificationDisplays.ContainsKey(_StyleID))
            {
                NotificationDisplays[_StyleID].UnpauseDisplay();
            }
            else
            {
                Debug.LogError("Ultimate Notification System: Notification Manager - Notification Display can't be found with UniqueID:" + _StyleID.ToString());
            }
        }

        /// <summary>
        /// This will clear all queued notifications from all displays.
        /// </summary>
        public virtual void ClearQueuesAll()
        {
            foreach (var item in NotificationDisplays)
            {
                item.Value.ClearQueues();
            }
        }


        /// <summary>
        /// This will clear all queued notifications from specific display.
        /// </summary>
        /// <param name="_StyleID">Unique ID of notification display.</param>
        public virtual void ClearQueuesOfStyle(string _StyleID)
        {
            if (NotificationDisplays.ContainsKey(_StyleID))
            {
                NotificationDisplays[_StyleID].ClearQueues(-1);
            }
            else
            {
                Debug.LogError("Ultimate Notification System: Notification Manager - Notification Display can't be found with UniqueID:" + _StyleID.ToString());
            }
        }

            /// <summary>
            /// This will clear all queued notifications from specific display.
            /// </summary>
            /// <param name="_StyleID">Unique ID of notification display.</param>
            /// <param name="_VariationID">If this is not -1, than only queued notificaiton of this index will be cleared</param>
        public virtual void ClearQueuesOfStyle(string _StyleID, int _VariationID = -1)
        {
            if (NotificationDisplays.ContainsKey(_StyleID))
            {
                NotificationDisplays[_StyleID].ClearQueues(_VariationID);
            }
            else
            {
                Debug.LogError("Ultimate Notification System: Notification Manager - Notification Display can't be found with UniqueID:" + _StyleID.ToString());
            }
            #endregion

        }
    }
}