using System;
using System.Collections.Generic;
using UnityEngine;

public class ManagerContainer
{
    private Dictionary<Type,object> managers = new Dictionary<Type, object>();

    public void Register<T>(T manager)
    {
        managers.Add(typeof(T), manager);
    }

    public T GetManager<T>()
    {
        if (managers.TryGetValue(typeof(T), out var manager))
        {
            return (T)manager;
        
        }
        throw new Exception($"[ManagerContainer] {typeof(T)} 매니저가 등록되지 않았습니다.");
    }

    public void InitializeAll()
    {
        foreach (var manager in managers.Values)
        {
            if (manager is IManager initManager)
            {
                initManager.Initialize(this);
            }
        }
    }
}
