﻿namespace Morpeh.Globals {
    using Unity.IL2CPP.CompilerServices;
    using UnityEngine;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = MenuPaths.GlobalEvents + "Event Bool")]
    public class GlobalEventBool : BaseGlobalEvent<bool> {
    }
}