using System;
using System.Collections;
using System.Collections.Generic;
using GDG.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace GDG.UI
{
    public class ImageHover : MonoBehaviour
    {
        public Sprite hoverTexture;
        public bool transparentRayCast = true;
        public float RayCastMaxAlpha = 0.5f;
        public Action HoverAction;
        public Action ExitAction;
        public Action EnterAction;
        private Sprite originTexture;
        private Image image;
        private bool isEnter = false;
        public bool IsHover { get; private set; }
        private bool isExist = false;
        private Collider2D hoverCollider;
        void Awake()
        {
            image = this.GetComponent<Image>();
            originTexture = image.sprite;
            if (transparentRayCast)
                image.alphaHitTestMinimumThreshold = RayCastMaxAlpha;
            hoverCollider = GetComponent<Collider2D>();
            if (hoverCollider == null)
            {
                Log.Error("Never set Collider2D.");
            }
        }
        void Update()
        {
            if (hoverCollider != null)
            {
                if (hoverCollider.OverlapPoint(Input.mousePosition))
                {
                    if (!isEnter)
                    {
                        if (hoverTexture != null)
                        {
                            image.sprite = hoverTexture;
                        }
                        isExist = false;
                        isEnter = true;
                        IsHover = true;
                        EnterAction?.Invoke();
                    }
                    HoverAction?.Invoke();
                }
                else
                {
                    if (!isExist)
                    {
                        image.sprite = originTexture;
                        isEnter = false;
                        IsHover = false;
                        isExist = true;
                        ExitAction?.Invoke();
                    }

                }
            }
        }
    }
}