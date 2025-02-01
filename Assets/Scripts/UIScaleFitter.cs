using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utilities
{
    [ExecuteInEditMode]
    public class UIScaleFitter : UIBehaviour
    {
        public enum AspectMode
        {
            None,
            FitInParent,
            EnvelopeParent
        }

        [SerializeField] AspectMode aspectMode = AspectMode.None;

        /// <summary>
        /// The mode to use to enforce the aspect ratio.
        /// </summary>
        public AspectMode Mode
        {
            get => aspectMode;
            set
            {
                if (aspectMode == value)
                    return;

                aspectMode = value;
                UpdateRect();
            }
        }


        // This "delayed" mechanism is required for case 1014834.
        bool delayedSetDirty;

        //Does the gameobject has a parent for reference to enable FitToParent/EnvelopeParent modes.
        bool DoesParentExist => RectTransform.parent != null;

        RectTransform rectTransform;
        RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                    rectTransform = GetComponent<RectTransform>();
                return rectTransform;
            }
        }
        

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateRect();
        }

        protected override void Start()
        {
            base.Start();
            //Disable the component if the aspect mode is not valid or the object state/setup is not supported with AspectRatio setup.
            if (!IsComponentValidOnObject() || !IsAspectModeValid())
                enabled = false;
        }

        void LateUpdate()
        {
            UpdateRect();
        }

        void UpdateRect()
        {
            if (!IsActive() || !IsComponentValidOnObject())
                return;

            switch (Mode)
            {
                case AspectMode.None:
                {
                    RectTransform.localScale = Vector3.one;
                    break;
                }
                
                case AspectMode.FitInParent:
                {
                    if(!DoesParentExist)
                        break;

                    Vector2 parentSize = GetParentSize();
                    Vector2 selfSize = RectTransform.rect.size;
                    
                    Vector2 deltaSize = parentSize - selfSize;

                    float deltaScale = deltaSize.x < deltaSize.y
                        ? deltaSize.x / selfSize.x
                        : deltaSize.y / selfSize.y;

                    RectTransform.localScale = Vector3.one * (1 + deltaScale);
                    break;
                }
                case AspectMode.EnvelopeParent:
                {
                    if(!DoesParentExist)
                        break;

                    Vector2 parentSize = GetParentSize();
                    Vector2 selfSize = RectTransform.rect.size;
                    
                    Vector2 deltaSize = parentSize - selfSize;

                    float deltaScale = deltaSize.x > deltaSize.y
                        ? deltaSize.x / selfSize.x
                        : deltaSize.y / selfSize.y;

                    RectTransform.localScale = Vector3.one * (1 + deltaScale);
                    break;
                }
            }
        }

        Vector2 GetParentSize()
        {
            RectTransform parent = RectTransform.parent as RectTransform;
            return !parent ? Vector2.zero : parent.rect.size;
        }

        bool IsAspectModeValid()
        {
            return DoesParentExist || aspectMode != AspectMode.EnvelopeParent && aspectMode != AspectMode.FitInParent;
        }
        
        bool IsComponentValidOnObject()
        {
            // if (RectTransform.anchorMax != new Vector2(0.5f, 0.5f) 
            //     || RectTransform.anchorMin != new Vector2(0.5f, 0.5f))
            // {
            //     Debug.LogWarning("UIScaleFitter doesn't work with non 0.5, 0.5, 0.5, 0.5 anchors ;)");
            //     return false;
            // }
            
            Canvas canvas = gameObject.GetComponent<Canvas>();
            return !canvas || !canvas.isRootCanvas || canvas.renderMode == RenderMode.WorldSpace;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            UpdateRect();
        }

#endif
    }
}