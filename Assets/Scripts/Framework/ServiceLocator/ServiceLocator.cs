using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Singleton;
using System.Linq;
using System;

namespace Framework.ServiceLocator
{
    public class Service : MonoBehaviour
    {
    }

    public class ServiceLocator : Singleton<ServiceLocator>
    {
        public T GetService<T>() where T : Service
        {
            return services.OfType<T>().FirstOrDefault();
        }

        public object GetService(Type type)
        {
            return services.FirstOrDefault(t => type.IsInstanceOfType(t));
        }

        public void InitService()
        {
            var comps = GetComponentsInChildren<Service>();
            services.AddRange(comps);
        }
    protected override void Awake()
    {
        base.Awake();
    }

        private List<Service> services = new List<Service>();
    }
}
