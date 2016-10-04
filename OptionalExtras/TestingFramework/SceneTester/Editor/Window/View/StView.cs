using System;
using System.Collections.Generic;
using ModestTree.Util;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace SceneTester
{
    public class StView : IGuiRenderable
    {
        readonly StListView _list;
        readonly EditorWindow _window;
        readonly Settings _settings;

        public StView(
            Settings settings,
            EditorWindow window,
            StListView list)
        {
            _list = list;
            _window = window;
            _settings = settings;
        }

        public string BlockedStatusMessage
        {
            get;
            set;
        }

        public string BlockedStatusTitle
        {
            get;
            set;
        }

        public bool IsBlocking
        {
            get;
            set;
        }

        public void GuiRender()
        {
            GUI.skin = _settings.GUISkin;

            var fullRect = new Rect(
                0, 0, _window.position.width, _window.position.height);

            _list.Draw(fullRect);

            if (IsBlocking)
            {
                ImguiUtil.DrawColoredQuad(fullRect, _settings.Theme.LoadingOverlayColor);

                DisplayGenericProcessingDialog(fullRect);
            }
        }

        public void DrawPopupCommon(Rect fullRect, Rect popupRect)
        {
            ImguiUtil.DrawColoredQuad(popupRect, _settings.Theme.LoadingOverlapPopupColor);
        }

        void DisplayGenericProcessingDialog(Rect fullRect)
        {
            var skin = _settings.AsyncPopupPane;
            var popupRect = ImguiUtil.CenterRectInRect(fullRect, skin.PopupSize);

            DrawPopupCommon(fullRect, popupRect);

            var contentRect = ImguiUtil.CreateContentRectWithPadding(
                popupRect, skin.PanelPadding);

            GUILayout.BeginArea(contentRect);
            {
                string title;

                if (string.IsNullOrEmpty(BlockedStatusTitle))
                {
                    title = "Processing";
                }
                else
                {
                    title = BlockedStatusTitle;
                }

                GUILayout.Label(title, skin.HeadingTextStyle, GUILayout.ExpandWidth(true));
                GUILayout.Space(skin.HeadingBottomPadding);

                string statusMessage = "";

                if (!string.IsNullOrEmpty(BlockedStatusMessage))
                {
                    statusMessage = BlockedStatusMessage;

                    int numExtraDots = (int)(Time.realtimeSinceStartup * skin.DotRepeatRate) % 4;

                    statusMessage += new String('.', numExtraDots);

                    // This is very hacky but the only way I can figure out how to keep the message a fixed length
                    // so that the text doesn't jump around as the number of dots change
                    // I tried using spaces instead of _ but that didn't work
                    statusMessage += ImguiUtil.WrapWithColor(new String('_', 3 - numExtraDots), _settings.Theme.LoadingOverlapPopupColor);
                }

                GUILayout.Label(statusMessage, skin.StatusMessageTextStyle, GUILayout.ExpandWidth(true));
            }

            GUILayout.EndArea();
        }

        [Serializable]
        public class Settings
        {
            public GUISkin GUISkinDark;
            public GUISkin GUISkinLight;

            public ThemeProperties Light;
            public ThemeProperties Dark;

            public AsyncPopupPaneProperties AsyncPopupPane;

            public GUISkin GUISkin
            {
                get
                {
                    return EditorGUIUtility.isProSkin ? GUISkinDark : GUISkinLight;
                }
            }

            public ThemeProperties Theme
            {
                get
                {
                    return EditorGUIUtility.isProSkin ? Dark : Light;
                }
            }

            [Serializable]
            public class ThemeProperties
            {
                public Color LoadingOverlayColor;
                public Color LoadingOverlapPopupColor;
            }

            [Serializable]
            public class AsyncPopupPaneProperties
            {
                public float PanelPadding;
                public Vector2 PopupSize;
                public float DotRepeatRate;
                public float HeadingBottomPadding;

                public GUIStyle HeadingTextStyle
                {
                    get
                    {
                        return GUI.skin.GetStyle("StProcessingPopupTextStyle");
                    }
                }

                public GUIStyle StatusMessageTextStyle
                {
                    get
                    {
                        return GUI.skin.GetStyle("StProcessingPopupStatusTextStyle");
                    }
                }
            }
        }
    }
}
