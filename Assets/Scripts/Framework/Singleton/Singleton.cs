using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Singleton
{
    public class Singleton<T> : MonoBehaviour where T : class
    {
        public static T Instance { get; private set; } = null;

        protected virtual void Awake()
        {
            lock (initLock)
            {
                if (Instance == null)
                {
                    Instance = this as T;
                    Debug.Log("Singleton instantiated " + (Instance != null));
                }
            }
        }
        private static object initLock = new object();
    }
}