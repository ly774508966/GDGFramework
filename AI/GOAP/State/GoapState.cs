using System.Linq;
using System;
using System.Collections.Generic;
using GDG.Utils;
using System.Text;

namespace GDG.AI
{
    public abstract class GoapState : IGoapState
    {
        private Dictionary<string, bool> m_StateDic = new Dictionary<string, bool>();
        public GoapState(params (string,bool)[] stateArgs)
        {
            foreach ((string,bool) item in stateArgs)
            {
                Add(item.Item1, item.Item2);
            }
        }
        public void Add(string key, bool value)
        {
            m_StateDic.Add(key, value);
        }
        public Dictionary<string, bool>.KeyCollection GetKeys()
        {
            return m_StateDic.Keys;
        }
        public bool ContainsKey(string key)
        {
            return m_StateDic.ContainsKey(key);
        }
        public bool TryGetValue(string key, out bool value)
        {
            return m_StateDic.TryGetValue(key, out value);
        }
		public bool GetValue(string key)
		{
            return m_StateDic[key];
        }
        public bool SetValue(string key, bool value)
        {
            if (m_StateDic.TryGetValue(key, out bool _value))
            {
                m_StateDic[key] = value;
                return true;
            }
            else
            {
                Log.Error($"SetValue failed ! Key does't exist ! Key: {key}");
                return false;
            }
        }
        /// <summary>
        /// 比较是否包含了其他的state
        /// </summary>
        public bool ContainsState(IGoapState other)
        {
            var keys = other.GetKeys();
            if(keys.Count==0)
                return false;
            foreach (var key in keys)
            {
                if (!ContainsKey(key) || m_StateDic[key] != other.GetValue(key))
                {
                    return false;
                }
            }
            return true;
        }
        public bool ContainsPairs(List<KeyValuePair<string, bool>> pairs)
        {
            foreach (var pair in pairs)
            {
                if(!ContainsKey(pair.Key) || m_StateDic[pair.Key] != pair.Value)
                    return false;
            }
            return true;
        }
        public List<KeyValuePair<string, bool>> GetDifferentStates(IGoapState other)
        {
            List<KeyValuePair<string, bool>> list = new List<KeyValuePair<string, bool>>();
            foreach (var key in other.GetKeys())
            {
                if (!m_StateDic.ContainsKey(key) || m_StateDic[key] != other.GetValue(key))
                {
                    list.Add(new KeyValuePair<string, bool>(key, m_StateDic[key]));
                }
            }
            return list;
        }
        public List<KeyValuePair<string, bool>> GetDifferentStates(List<KeyValuePair<string, bool>> pairs)
        {
            List<KeyValuePair<string, bool>> list = new List<KeyValuePair<string, bool>>();
            foreach (var pair in pairs)
            {
                if(!ContainsKey(pair.Key) || m_StateDic[pair.Key] != pair.Value)
                    list.Add(pair);
            }
            return list;            
        }
        public List<KeyValuePair<string, bool>> GetPairs()
        {
            List<KeyValuePair<string, bool>> list = new List<KeyValuePair<string, bool>>();
            foreach(var item in m_StateDic)
            {
                list.Add(item);
            }
            return list;
        }
        public void SetDifferentStates(IGoapState other)
        {
            foreach (var key in other.GetKeys())
            {
                if (!m_StateDic.ContainsKey(key))
                {
                    m_StateDic.Add(key, other.GetValue(key));
                }
                if (m_StateDic[key] != other.GetValue(key))
                {
                    m_StateDic[key] = other.GetValue(key);
                }
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\n");
            foreach (var key in m_StateDic.Keys)
            {
                sb.Append($"{key}: {m_StateDic[key]}\n");
            }
            return sb.ToString();
        }
    }
	public class GoapState<TState> : GoapState where TState:Enum
	{
		public GoapState() : base()
		{
			foreach(var item in Enum.GetValues(typeof(TState)))
			{
                Add(item.ToString(),false);
            }
		}
        public GoapState(params (TState,bool)[] stateArgs)
        {
            foreach ((TState,bool) item in stateArgs)
            {
                Add(item.Item1, item.Item2);
            }
        }
        public void Add(TState key, bool value)
        {
            Add(key.ToString(), value);
        }
        public bool ContainsKey(TState key)
        {
            return ContainsKey(key.ToString());
        }
        public bool TryGetValue(TState key, out bool value)
        {
            return TryGetValue(key.ToString(), out value);
        }
		public bool GetValue(TState key)
		{
            return GetValue(key.ToString());
        }
        public bool SetValue(TState key, bool value)
		{
            return SetValue(key.ToString(),value);
        }
	}
}