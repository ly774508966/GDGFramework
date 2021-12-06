using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GDG.Base
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static volatile T instance;
        private static object initLock = new object();
        public static T Instance
        {
            get
            {
                return instance;
            }
        }
        protected virtual void Awake()
        {
            if (instance == null)
            {
                lock (initLock)
                {
                    if (instance == null)
                    {
                        instance = this as T;
                    }
                }
            }
        }
    }
}