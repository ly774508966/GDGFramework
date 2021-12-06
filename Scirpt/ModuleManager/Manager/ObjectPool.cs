using System;
using System.Collections.Generic;
using GDG.Base;
using GDG.Utils;

namespace GDG.ModuleManager
{
	public class ObjectPool : LazySingleton<ObjectPool>
	{
        private Dictionary<Type, Stack<object>> poolDic = new Dictionary<Type, Stack<object>>();
		public T Pop<T>()
		{
			if(poolDic.TryGetValue(typeof(T),out Stack<object> stack))
			{
				if(stack.Count == 0)
				{
                    return Activator.CreateInstance<T>();
                }
				else
				{
					return (T)stack.Pop();
				}
			}
			else
			{
                poolDic.Add(typeof(T), new Stack<object>());
                return Activator.CreateInstance<T>();
            }
		}
		public void Push<T>(T value)
		{
			if(value is IRecyclable recyclable)
			{
                recyclable.OnRecycle();
            }

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
	public class ObjectPool<T> : LazySingleton<ObjectPool<T>>
	{
        private Stack<T> objStack = new Stack<T>();
        public T Pop()
		{
			if(objStack.Count == 0)
			{
                return Activator.CreateInstance<T>();
            }
			else
			{
				return objStack.Pop();
			}

		}
		public void Push(T value)
		{
			if(value is IRecyclable recyclable)
                recyclable.OnRecycle();
            objStack.Push(value);
		}
		public void Clear()
		{
			foreach(var item in objStack )
			{
				if(item is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
			objStack.Clear();
		}
    }
}