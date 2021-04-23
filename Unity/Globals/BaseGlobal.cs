namespace Morpeh.Globals {
    using System;
    using ECS;
    using JetBrains.Annotations;
    using Unity.IL2CPP.CompilerServices;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using UnityEngine;
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public abstract class BaseGlobal : BaseSingleton, IDisposable {
        public bool IsPublished {
            get {
#if UNITY_EDITOR
                if (!Application.isPlaying) {
                    return default;
                }
                this.CheckIsInitialized();
                return this.InternalEntity.Has<GlobalEventPublished>();
#else
                return this.internalEntity.Has<GlobalEventPublished>();
#endif
            }
        }
        
#if UNITY_EDITOR
        public abstract Type GetValueType();
#endif
        public abstract string LastToString();

        public static implicit operator bool(BaseGlobal exists) => exists != null && exists.IsPublished;

        protected class Unsubscriber : IDisposable {
            private readonly Action unsubscribe;
            public Unsubscriber(Action unsubscribe) => this.unsubscribe = unsubscribe;
            public void Dispose() => this.unsubscribe();
        }
    }
}