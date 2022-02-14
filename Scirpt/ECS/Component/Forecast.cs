using System;
using System.Collections.Generic;
using GDG.ModuleManager;
namespace GDG.ECS
{
    public class Forecast<T>
    {
        private T data;
        public Action action;
        T Value{
            get => data;
            set
            {
                if(!value.Equals(this.data))
                {
                    action?.Invoke();
                }
                this.data = value;
            }
        }
        
    }
}