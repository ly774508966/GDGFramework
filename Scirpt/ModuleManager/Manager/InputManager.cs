using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GDG.Utils;
using GDG.Base;

/*
需求：
1. 支持改键
2. 支持长按，按下，抬起，双击
*/
namespace GDG.ModuleManager
{
    #region Key
    public class Key : IEquatable<Key>
    {
        public string keyName;
        public List<KeyCode> keyCodes;
        public Key() => keyCodes = new List<KeyCode>();
        public Key(string keyName, List<KeyCode> keyCodes)
        {
            this.keyName = keyName;
            this.keyCodes = keyCodes;
        }

        public bool Equals(Key other)
        {
            if (other?.keyCodes == null || this?.keyCodes == null || this.keyCodes.Count != keyCodes.Count)
                return false;
            for (var i = 0; i < keyCodes.Count; i++)
            {
                if (keyCodes[i] != other.keyCodes[i])
                {
                    return false;
                }
            }
            return true;
        }
        public static bool operator ==(Key left, Key right)
        {
            if (left?.keyCodes == null || right?.keyCodes == null || left.keyCodes.Count != right.keyCodes.Count)
                return false;
            for (var i = 0; i < left.keyCodes.Count; i++)
            {
                if (left.keyCodes[i] != right.keyCodes[i])
                {
                    return false;
                }
            }
            return true;
        }
        public static bool operator !=(Key left, Key right)
        {
            if (left?.keyCodes == null || right?.keyCodes == null || left?.keyCodes.Count != right.keyCodes.Count)
                return true;
            for (var i = 0; i < left.keyCodes.Count; i++)
            {
                if (left.keyCodes[i] != right.keyCodes[i])
                {
                    return true;
                }
            }
            return false;
        }
        public override int GetHashCode()
        {
            return keyName.GetHashCode();
        }

        public override string ToString()
        {
            if (keyCodes == null || keyCodes.Count == 0)
                return "Null";
            return string.Join(" + ", keyCodes.ToArray());
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Key);
        }
    }
    #endregion
    public class InputManager : LazySingleton<InputManager>
    {
        private readonly List<Key> m_keyList = new List<Key>();
        private readonly static double m_DoubleClickInterval = 280;//双击时间间隔（毫秒）
        private double m_CurrentTime;
        private string m_InputConfigPath = Configurations.ConfigPath + "/InputConfig.json";
        
        public InputManager()
        {    
            var inputConfig = JsonManager.LoadData<List<Key>>(m_InputConfigPath);
            if (inputConfig != null)
                m_keyList = inputConfig;
        }
        public void AddKeyMapping(Key key)
        {
            bool canAdd = true;
            foreach(var item in m_keyList)
            {
                if(item.keyName == key.keyName)
                {
                    Log.Error($"AddKeyMapping failed ! Repeated key name :{key.keyName}");
                    canAdd = false;
                }
            }
            if(canAdd)
            {
                m_keyList.Add(key);
            }
        }
        public void AddKeyMapping(string keyName, List<KeyCode> keyCodes)
        {
            bool canAdd = true;
            foreach(var item in m_keyList)
            {
                if(item.keyName == keyName)
                {
                    Log.Error($"AddKeyMapping failed ! Repeated key name :{keyName}");
                    canAdd = false;
                }
            }
            if(canAdd)
            {
                m_keyList.Add(new Key(keyName,keyCodes));
            }
        }
        public bool RemoveKeyMapping(string KeyName)
        {
            foreach(var key in m_keyList)
            {
                if(key.keyName.Equals(key.keyName))
                {
                    m_keyList.Remove(key);
                    return true;
                }
            }
            return false;
        }
        public bool GetKeyDown(string keyName)
        {
            foreach (var key in m_keyList)
            {
                if (keyName == key.keyName)
                {
                    var keyCodes = key.keyCodes;
                    switch (keyCodes.Count)
                    {
                        case 1: return Input.GetKeyDown(keyCodes[0]);
                        case 2:
                            if (Input.GetKey(keyCodes[0]))
                                return Input.GetKeyDown(keyCodes[1]);
                            return false;
                        case 3:
                            if (Input.GetKey(keyCodes[0]) && Input.GetKey(keyCodes[1]))
                                return Input.GetKeyDown(keyCodes[2]);
                            return false;
                    }
                }
            }
            return false;
        }
        public bool GetKeyUp(string keyName)
        {
            foreach (var key in m_keyList)
            {
                if (keyName == key.keyName)
                {
                    var keyCodes = key.keyCodes;
                    switch (keyCodes.Count)
                    {
                        case 1: return Input.GetKeyUp(keyCodes[0]);
                        case 2:
                            if (Input.GetKeyUp(keyCodes[0]) || Input.GetKeyUp(keyCodes[1]))
                                return true;
                            return false;
                        case 3:
                            if (Input.GetKeyUp(keyCodes[0]) || Input.GetKeyUp(keyCodes[1]) || Input.GetKeyUp(keyCodes[2]))
                                return true;
                            return false;
                    }
                }
            }
            return false;
        }
        public bool GetKeyPress(string keyName)
        {
            foreach (var key in m_keyList)
            {
                if (keyName == key.keyName)
                {
                    var keyCodes = key.keyCodes;
                    switch (keyCodes.Count)
                    {
                        case 1: return Input.GetKey(keyCodes[0]);
                        case 2:
                            if (Input.GetKey(keyCodes[0]))
                                return Input.GetKey(keyCodes[1]);
                            return false;
                        case 3:
                            if (Input.GetKey(keyCodes[0]) && Input.GetKey(keyCodes[1]))
                                return Input.GetKey(keyCodes[2]);
                            return false;
                    }
                }
            }
            return false;
        }
        public bool GetKeyDoubleClick(string keyName)
        {
            if (GetKeyUp(keyName))
            {
                m_CurrentTime = TimerManager.Instance.CurrentTime;
            }
            if (TimerManager.Instance.CurrentTime - m_CurrentTime < m_DoubleClickInterval)
            {
                return GetKeyDown(keyName);
            }
            else
            {
                m_CurrentTime = 0;
                return false;
            }
        }
        public void ChangeKey(string keyName, KeyCode keycode)
        {
            foreach (var key in m_keyList)
            {
                if (key.keyName == keyName)
                {
                    key.keyCodes = new List<KeyCode>() { keycode };
                }
            }
        }
        public void ChangeKey(string keyName, KeyCode keycode1, KeyCode keycode2)
        {
            foreach (var key in m_keyList)
            {
                if (key.keyName == keyName)
                {
                    key.keyCodes = new List<KeyCode>() { keycode1, keycode2 };
                }
            }
        }
        public void ChangeKey(string keyName, KeyCode keycode1, KeyCode keycode2, KeyCode keycode3)
        {
            foreach (var key in m_keyList)
            {
                if (key.keyName == keyName)
                {
                    key.keyCodes = new List<KeyCode>() { keycode1, keycode2, keycode3 };
                }
            }
        }
        public void SaveConfig()
        {
            JsonManager.SaveData<List<Key>>(m_keyList, m_InputConfigPath);
        }
    }
}
