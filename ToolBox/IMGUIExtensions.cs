using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToolBox.Extensions {
    internal static class IMGUIExtensions
    {
        public static void SetGlobalFontSize(int size)
        {
            foreach (GUIStyle style in GUI.skin)
                style.fontSize = size;
            GUI.skin = GUI.skin;
        }

        public static void ResetFontSize()
        {
            SetGlobalFontSize(0);
        }

        private static readonly GUIStyle _customBoxStyle = new GUIStyle { normal = new GUIStyleState { background = Texture2D.whiteTexture } };
#if HONEYSELECT || PLAYHOME || KOIKATSU
        private static readonly Color _backgroundColor = new Color(1f, 1f, 1f, 0.5f);
#elif AISHOUJO
        private static readonly Color _backgroundColor = new Color(0f, 0f, 0f, 0.5f);
#endif

        public static void DrawBackground(Rect rect)
        {
            Color c = GUI.backgroundColor;
            GUI.backgroundColor = _backgroundColor;
            GUI.Box(rect, "", _customBoxStyle);
            GUI.backgroundColor = c;

        }

        private static readonly Stack<Color> _colorStack = new Stack<Color>(new[] {Color.white});
        public static void PushColor(Color c)
        {
            _colorStack.Push(c);
            GUI.color = c;
        }

        public static void PopColor()
        {
            _colorStack.Pop();
            GUI.color = _colorStack.Peek();
        }

        public static void HorizontalSliderWithValue(string label, float value, float left, float right, string valueFormat = "", Action<float> onChanged = null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.ExpandWidth(false));
            float newValue = GUILayout.HorizontalSlider(value, left, right);
            string valueString = newValue.ToString(valueFormat);
            string newValueString = GUILayout.TextField(valueString, 5, GUILayout.Width(50f));

            if (newValueString != valueString)
            {
                float parseResult;
                if (float.TryParse(newValueString, out parseResult))
                    newValue = parseResult;
            }
            GUILayout.EndHorizontal();

            if (onChanged != null && !Mathf.Approximately(value, newValue))
                onChanged(newValue);
        }
    }
}