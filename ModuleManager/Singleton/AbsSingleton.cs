using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDG.ModuleManager
{
    public abstract class AbsSingleton<T> where T : new()
    {
        private static  T instance;
        private static object syncRootObject = new object();
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRootObject)
                    {
                        if (instance == null)
                        {
                            instance = new T();
                        }
                    }
                }
                return instance;
            }
        }
    }
}