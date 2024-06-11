using UnityEngine.Events;

namespace Animmal
{
    [System.Serializable]
    public class UnityBoolEvent : UnityEvent<bool> { }
    [System.Serializable]
    public class UnityFloatEvent : UnityEvent<float> { }
    [System.Serializable]
    public class UnityIntEvent : UnityEvent<int> { }
    [System.Serializable]
    public class UnityStringEvent : UnityEvent<string> { }

}