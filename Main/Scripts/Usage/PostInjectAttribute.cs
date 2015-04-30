using System;
using JetBrains.Annotations;

namespace Zenject
{
    [AttributeUsage(AttributeTargets.Method), MeansImplicitUse]
    public class PostInjectAttribute : Attribute
    {
    }
}