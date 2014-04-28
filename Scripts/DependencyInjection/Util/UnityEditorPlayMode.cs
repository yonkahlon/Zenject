using System;
using UnityEditor;

namespace ModestTree
{
    public enum PlayModeState
    {
        Stopped,
        Playing,
        Paused
    }

    [InitializeOnLoad]
    public class UnityEditorPlayMode
    {
        static PlayModeState _currentState = PlayModeState.Stopped;

        public static event Action<PlayModeState, PlayModeState> PlayModeChanged = delegate {};

        static UnityEditorPlayMode()
        {
            EditorApplication.playmodeStateChanged = OnUnityPlayModeChanged;
        }

        public static void Play()
        {
            EditorApplication.isPlaying = true;
        }

        public static void Pause()
        {
            EditorApplication.isPaused = true;
        }

        public static void Stop()
        {
            EditorApplication.isPlaying = false;
        }

        static void OnUnityPlayModeChanged()
        {
            var changedState = PlayModeState.Stopped;

            switch (_currentState)
            {
                case PlayModeState.Stopped:
                {
                    if (EditorApplication.isPlayingOrWillChangePlaymode)
                    {
                        changedState = PlayModeState.Playing;
                    }
                    break;
                }
                case PlayModeState.Playing:
                {
                    if (EditorApplication.isPaused)
                    {
                        changedState = PlayModeState.Paused;
                    }
                    else
                    {
                        changedState = PlayModeState.Stopped;
                    }
                    break;
                }
                case PlayModeState.Paused:
                {
                    if (EditorApplication.isPlayingOrWillChangePlaymode)
                    {
                        changedState = PlayModeState.Playing;
                    }
                    else
                    {
                        changedState = PlayModeState.Stopped;
                    }
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }

            var oldState = _currentState;
            _currentState = changedState;

            PlayModeChanged(oldState, changedState);
        }
    }
}
