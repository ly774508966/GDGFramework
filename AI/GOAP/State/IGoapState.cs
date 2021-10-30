using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ECS;
using GDG.Utils;
using System;

namespace GDG.AI
{
	public interface IGoapState
	{
        bool ContainsKey(string key);
        bool ContainsState(IGoapState state);
        bool ContainsPairs(List<KeyValuePair<string, bool>> pairs);
        void Add(string key, bool value);
        Dictionary<string, bool>.KeyCollection GetKeys();
        bool GetValue(string key);
        bool TryGetValue(string key,out bool value);
        bool SetValue(string key,bool value);
        void SetDifferentStates(IGoapState other);
        List<KeyValuePair<string, bool>> GetDifferentStates(IGoapState other);
        List<KeyValuePair<string, bool>> GetDifferentStates(List<KeyValuePair<string, bool>> pairs);
        List<KeyValuePair<string, bool>> GetPairs();
    }
}