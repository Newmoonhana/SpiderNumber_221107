using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Newmoonhana.Tools
{
    public static class HADInputEventManager
    {
        public delegate void StartTouchEvent(Vector2 position, float time);
        public static event StartTouchEvent OnStartTouch;
        public delegate void DragTouchEvent(Vector2 position, float time);
        public static event DragTouchEvent OnDragTouch;
        public delegate void EndTouchEvent(Vector2 position, float time);
        public static event EndTouchEvent OnEndTouch;

        static TouchAction touch_control;

        //좌표 계산 전 저장 변수
        static Vector3 screenCoordinates;
        internal static Vector3 worldCoordinates;

        public static void Init()
        {
            touch_control = new TouchAction();
            touch_control.Touch.press.started += ctx => StartTouch(ctx);
            touch_control.Touch.drag.performed += ctx => DragTouch(ctx);
            touch_control.Touch.press.canceled += ctx => EndTouch(ctx);
        }

        public static void Enable()
        {
            touch_control.Enable();
        }
        public static void Disable()
        {
            touch_control.Disable();
        }

        static void StartTouch(InputAction.CallbackContext _ctx)
        {
            if (OnStartTouch != null)
            {
                CalPosition(touch_control.Touch.position.ReadValue<Vector2>(), (float)_ctx.startTime);
                OnStartTouch(touch_control.Touch.position.ReadValue<Vector2>(), (float)_ctx.startTime);
            }
            //Debug.Log("Touch started " + touch_control.Touch.position.ReadValue<Vector2>());
        }
        static void DragTouch(InputAction.CallbackContext _ctx)
        {
            if (OnDragTouch != null)
            {
                CalPosition(touch_control.Touch.position.ReadValue<Vector2>(), (float)_ctx.startTime);
                OnDragTouch(touch_control.Touch.position.ReadValue<Vector2>(), (float)_ctx.startTime);
            }
            //Debug.Log("Drag " + touch_control.Touch.position.ReadValue<Vector2>());
        }
        static void EndTouch(InputAction.CallbackContext _ctx)
        {
            if (OnEndTouch != null)
            {
                CalPosition(touch_control.Touch.position.ReadValue<Vector2>(), (float)_ctx.time);
                OnEndTouch(touch_control.Touch.position.ReadValue<Vector2>(), (float)_ctx.time);
            }
            //Debug.Log("Touch ended " + touch_control.Touch.position.ReadValue<Vector2>());
        }

        static void CalPosition(Vector2 _screen_pos, float time)
        {
            //좌표 계산
            screenCoordinates = Vector3.zero;
            screenCoordinates.x = _screen_pos.x;
            screenCoordinates.y = _screen_pos.y;
            screenCoordinates.z = Camera.main.nearClipPlane;

            worldCoordinates = Camera.main.ScreenToWorldPoint(screenCoordinates);
            worldCoordinates.z = 0;
        }
    }
}