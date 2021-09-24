using System;
using System.Collections;
using System.Collections.Generic;
using GDG.ModuleManager;
using UnityEngine;
using UnityEngine.Events;

namespace GDG.ECS
{
    public class MonoWorld : MonoBehaviour
    {
        private UnityAction timerUpdate;
        private UnityAction awake=null;
        private UnityAction start=null;
        private UnityAction ongui = null;
        private UnityAction beforeUpdate=null;
        private UnityAction afterUpdate=null;
        private UnityAction update=null;
        private UnityAction fixedUpdate=null;
        private UnityAction lateUpdate=null;
        private UnityAction onEnable=null;
        private UnityAction onDisable=null;
        private UnityAction destroy=null;
        void Awake() { timerUpdate += TimerManager.Instance.OnUpdate; DontDestroyOnLoad(this); if (awake != null) awake(); }
        void Start() { if (start != null) start(); }
        void OnGUI() { if (ongui != null) ongui(); }
        void Update() { beforeUpdate?.Invoke(); update?.Invoke(); timerUpdate?.Invoke(); afterUpdate?.Invoke(); }
        void FixedUpdate() { if (fixedUpdate != null) fixedUpdate(); }
        void LateUpdate() { if (lateUpdate != null) lateUpdate(); }
        void OnEnable() { if (onEnable != null) onEnable(); }
        void OnDisable() { if (onDisable != null) onDisable(); }
        void Destroy()
        {
            if (destroy != null) destroy();
            start = update = fixedUpdate = lateUpdate = onEnable = onDisable = destroy = null;
        }
        public void AddOrRemoveListener(UnityAction fun, string lifeFuncName, bool isAdd = true)
        {
            switch (lifeFuncName)
            {
                case "Awake":
                    if (isAdd) awake += fun;
                    else awake -= fun;
                    break;
                case "Start":
                    if (isAdd) start += fun;
                    else start -= fun;
                    break;
                case "OnGUI":
                    if (isAdd) ongui += fun;
                    else ongui -= fun;
                    break;
                case "BeforeUpdate":
                    if (isAdd) beforeUpdate += fun;
                    else beforeUpdate -= fun;
                    break;
                case "Update":
                    if (isAdd) update += fun;
                    else update -= fun;
                    break;
                case "AfterUpdate":
                    if (isAdd) afterUpdate += fun;
                    else afterUpdate -= fun;
                    break;
                case "FixedUpdate":
                    if (isAdd) fixedUpdate += fun;
                    else fixedUpdate -= fun;
                    break;
                case "LateUpdate":
                    if (isAdd) lateUpdate += fun;
                    else lateUpdate -= fun;
                    break;
                case "OnEnable":
                    if (isAdd) onEnable += fun;
                    else onEnable -= fun;
                    break;
                case "OnDisable":
                    if (isAdd) onDisable += fun;
                    else onDisable -= fun;
                    break;
                case "Destroy":
                    if (isAdd) destroy += fun;
                    else destroy -= fun;
                    break;
            }
        }
        public void StartTimer(Action callback,float secondTime)
        {
            StartCoroutine(Timer(callback,secondTime));
        }
        private IEnumerator Timer(Action callback,float secondTime)
        {
            yield return new WaitForSeconds(secondTime);
            if(callback!=null)
                callback();
        }
    }
}