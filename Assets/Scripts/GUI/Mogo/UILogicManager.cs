using Mogo.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class MogoUIBehaviour : MFUIUnit
{
    protected UILogicManager m_uiLoginManager;
    protected virtual void OnEnable()
    {
        if (m_uiLoginManager != null)
        {
            m_uiLoginManager.UpdateUI();
        }
    }
    protected Dictionary<string, string> m_widgetToFullName = new Dictionary<string, string>();
    protected void FillFullNameData(Transform rootTransform) 
    {
        for (int i = 0; i < rootTransform.GetChildCount(); ++i)
        {
            AddWidgetToFullNameData(rootTransform.GetChild(i).name, GetFullName(rootTransform.GetChild(i)));
            FillFullNameData(rootTransform.GetChild(i));
        }
    }
    private void AddWidgetToFullNameData(string widgetName, string fullName)
    {
        if (m_widgetToFullName.ContainsKey(widgetName))
            ;
        m_widgetToFullName.Add(widgetName, fullName);
    }
    private string GetFullName(Transform currentTransform) 
    {
        string fullName = "";
        while (currentTransform != m_myTransform)
        {
            fullName = currentTransform.name + fullName;
            if (currentTransform.parent != m_myTransform)
                fullName = "/" + fullName;
            currentTransform = currentTransform.parent;
        }
        return fullName;
    }
    protected Transform FindTransform(string transformName) 
    {
        if (m_widgetToFullName.ContainsKey(transformName))
            return m_myTransform.Find(m_widgetToFullName[transformName]);
        return null;
    }
}

public abstract class UILogicManager
{
    private HashSet<INotifyPropChanged> m_itemSources = new HashSet<INotifyPropChanged>();
    private EventController m_eventController;

    public INotifyPropChanged ItemSource
    {
        set
        {
            if (value != null && !m_itemSources.Contains(value))
            {
                m_itemSources.Add(value);
                value.SetEventController(m_eventController);
                value.AddUnloadCallback(() =>
                {
                    if (m_itemSources != null && m_itemSources.Contains(value))
                    {
                        m_itemSources.Remove(value);
                    }
                });
            }
        }
    }
    public UILogicManager()
    {
        m_eventController = new EventController();
    }
    public void SetBinding<T>(String key, Action<T> action)
    {
        if (m_eventController.ContainsEvent(key))
            return;
        m_eventController.AddEventListener(key, action);
    }
    public void UpdateUI()
    {
        foreach (var itemSource in m_itemSources)
        {
            if (itemSource != null)
            {
                var type = itemSource.GetType();
                var mTriggerEvent = m_eventController.GetType().GetMethods().FirstOrDefault(t => t.Name == "TriggerEvent" && t.IsGenericMethod && t.GetGenericArguments().Length == 1);
                foreach (var item in m_eventController.TheRouter)
                {
                    var prop = type.GetProperty(item.Key);
                    if (prop == null)
                        continue;
                    var method = mTriggerEvent.MakeGenericMethod(prop.PropertyType);
                    var value = prop.GetGetMethod().Invoke(itemSource, null);
                    method.Invoke(m_eventController, new object[] { item.Key, value });
                        
                }
            }
        }
    }
    public virtual void Release()
    {
        foreach (var item in m_itemSources)
        {
            if (item != null)
                item.RemoveEventController(m_eventController);
        }
        m_itemSources.Clear();
        m_eventController.Cleanup();
    }
}

public interface INotifyPropChanged
{
    void SetEventController(EventController controller);
    void RemoveEventController(EventController controller);
    void OnPropertyChanged<T>(string propertyName, T value);
    void AddUnloadCallback(Action onUnload);
}

public abstract class NotifyPropChanged
{
    private HashSet<EventController> m_uiBindingSet = new HashSet<EventController>();
    private Action m_onUnload;

    public void SetEventController(EventController controller)
    {
        m_uiBindingSet.Add(controller);
    }
    public void RemoveEventController(EventController controller)
    {
        m_uiBindingSet.Remove(controller);
    }
    public void OnPropertyChanged<T>(string propertyName, T value)
    {
        foreach (var item in m_uiBindingSet)
        {
            if (item != null)
                item.TriggerEvent(propertyName, value);
        }
    }
    public void AddUnloadCallback(Action onUnload)
    {
        m_onUnload = onUnload;
    }
    protected void ClearBinding()
    {
        if (m_onUnload != null)
            m_onUnload();
        m_uiBindingSet.Clear();
    }

}