using System.IO;
using ModestTree;
using ModestTree.Util;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;
using Zenject;

namespace SceneTester
{
    public class ContextMenuItem
    {
        public readonly bool IsEnabled;
        public readonly string Caption;
        public readonly Action Handler;
        public readonly bool IsChecked;

        public ContextMenuItem(
            bool isEnabled, string caption, bool isChecked, Action handler)
        {
            IsEnabled = isEnabled;
            Caption = caption;
            Handler = handler;
            IsChecked = isChecked;
        }
    }
}
