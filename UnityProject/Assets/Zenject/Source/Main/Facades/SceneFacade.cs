#if !ZEN_NOT_UNITY3D

using System;

namespace Zenject
{
    public class SceneFacade : MonoFacade
    {
        // Intentional left empty
        // We just need this class to exist so we can make GlobalFacade have a higher 
        // script execution order
    }
}

#endif
