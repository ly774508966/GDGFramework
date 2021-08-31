using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GDG.ECS
{
    public class MonoWorld : MonoBehaviour
    {
        public UnityAction awake=null;
        public UnityAction start=null;
        public UnityAction ongui = null;
        public UnityAction update=null;
        public UnityAction fixedUpdate=null;
        public UnityAction lateUpdate=null;
        public UnityAction onEnable=null;
        public UnityAction onDisable=null;
        public UnityAction destroy=null;
        public UnityAction proxyConvertExcute = null;
        void Awake() { if (awake != null) awake(); }
        void Start() { if (start != null) start(); if (proxyConvertExcute != null) proxyConvertExcute(); }
        void OnGUI() { if (ongui != null) ongui(); }
        void Update() { if (update != null) update(); }
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
                case "ProxyConvertExcute":
                    if (isAdd) proxyConvertExcute += fun;
                    else proxyConvertExcute -= fun;
                    break;
                case "OnGUI":
                    if (isAdd) ongui += fun;
                    else ongui -= fun;
                    break;
                case "Update":
                    if (isAdd) update += fun;
                    else update -= fun;
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