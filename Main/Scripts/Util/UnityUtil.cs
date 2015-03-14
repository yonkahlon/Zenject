using System;
using UnityEngine;

namespace ModestTree
{
    public enum MouseWheelScrollDirections
    {
        None,
        Up,
        Down,
    }

    public static class UnityUtil
    {
        // Due to the way that Unity overrides the Equals operator,
        // normal null checks such as (x == null) do not always work as
        // expected
        // In those cases you can use this function which will also
        // work with non-unity objects
        public static bool IsNull(System.Object obj)
        {
            return obj == null || obj.Equals(null);
        }

        public static bool IsAltKeyDown
        {
            get
            {
                return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
            }
        }

        public static bool IsControlKeyDown
        {
            get
            {
                return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            }
        }

        public static bool IsShiftKeyDown
        {
            get
            {
                return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            }
        }

        public static bool WasShiftKeyJustPressed
        {
            get
            {
                return Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);
            }
        }

        public static bool WasAltKeyJustPressed
        {
            get
            {
                return Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt);
            }
        }

        public static MouseWheelScrollDirections CheckMouseScrollWheel()
        {
            var value = Input.GetAxis("Mouse ScrollWheel");

            if (Mathf.Approximately(value, 0.0f))
            {
                return MouseWheelScrollDirections.None;
            }

            if (value < 0)
            {
                return MouseWheelScrollDirections.Down;
            }

            return MouseWheelScrollDirections.Up;
        }
    }
}
