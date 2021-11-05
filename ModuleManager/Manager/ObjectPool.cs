using System;
using System.Collections.Generic;
using GDG.Utils;

namespace GDG.ModuleManager
{
	public class ObjectPool : AbsLazySingleton<ObjectPool>
	{
        private Dictionary<Type, Stack<object>> poolDic = new Dictionary<Type, Stack<object>>();
		public T Pop<T>() where T:new()
		{
			if(poolDic.TryGetValue(typeof(T),out Stack<object> stack))
			{
				if(stack.Count == 0)
				{
                    return new T();
                }
				else
				{
					return (T)stack.Pop();
				}
			}
			else
			{
                poolDic.Add(typeof(T), new Stack<object>());
                return new T();
            }
		}
		public void Push<T>(T value)where T:class,new()
		{
			if(poolDic.TryGetValue(typeof(T),out Stack<object> stack))
			{
                stack.Push(value);
            }
			else
			{
                var statck = new Stack<object>();
                stack.Push(value);
                poolDic.Add(typeof(T), stack);
            }
		}
		public void Clear()
		{
			foreach (var stack in poolDic.Values)
			{
				foreach(var item in stack )
				{
					if(item is IDisposable disposable)
					{
                        disposable.Dispose();
                    }
				}
                stack.Clear();
            }
		}
    }
}