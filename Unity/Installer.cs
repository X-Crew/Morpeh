using Frigg;

namespace Morpeh {
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using System.Linq;
    using UnityEngine;
    using Utils;
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [AddComponentMenu("ECS/" + nameof(Installer))]
    public sealed class Installer : BaseInstaller {
        [Required]
        [InfoBox("Order collision with other installer!", InfoMessageType = InfoMessageType.Error, 
            Member = nameof(IsCollisionWithOtherInstaller))]
        [Order(-5)]
        public int order;

        private bool IsCollisionWithOtherInstaller 
            => this.IsPrefab() == false && FindObjectsOfType<Installer>().Where(i => i != this).Any(i => i.order == this.order);
        
        private bool IsPrefab() => this.gameObject.scene.name == null;

        [Order(-5)]
        [PropertySpace(SpaceBefore = 5)]
        public Initializer[] initializers;
        
        [Order(-4)]
        [OnValueChanged(nameof(OnValueChangedUpdate))]
        public UpdateSystemPair[] updateSystems;
        
        [Order(-3)]
        [OnValueChanged(nameof(OnValueChangedFixedUpdate))]
        public FixedSystemPair[] fixedUpdateSystems;
        
        [Order(-2)]
        [OnValueChanged(nameof(OnValueChangedLateUpdate))]
        public LateSystemPair[] lateUpdateSystems;

        private SystemsGroup group;

        private void OnValueChangedUpdate() {
            if (Application.isPlaying) {
                this.RemoveSystems(this.updateSystems);
                this.AddSystems(this.updateSystems);
            }
        }
        
        private void OnValueChangedFixedUpdate() {
            if (Application.isPlaying) {
                this.RemoveSystems(this.fixedUpdateSystems);
                this.AddSystems(this.fixedUpdateSystems);
            }
        }
        
        private void OnValueChangedLateUpdate() {
            if (Application.isPlaying) {
                this.RemoveSystems(this.lateUpdateSystems);
                this.AddSystems(this.lateUpdateSystems);
            }
        }
        

        protected override void OnEnable() {
            this.group = World.Default.CreateSystemsGroup();
            
            for (int i = 0, length = this.initializers.Length; i < length; i++) {
                var initializer = this.initializers[i];
                this.group.AddInitializer(initializer);
            }

            this.AddSystems(this.updateSystems);
            this.AddSystems(this.fixedUpdateSystems);
            this.AddSystems(this.lateUpdateSystems);
            
            World.Default.AddSystemsGroup(this.order, this.group);
        }

        protected override void OnDisable() {
            this.RemoveSystems(this.updateSystems);
            this.RemoveSystems(this.fixedUpdateSystems);
            this.RemoveSystems(this.lateUpdateSystems);
            
            World.Default.RemoveSystemsGroup(this.group);
        }

        private void AddSystems<T>(BasePair<T>[] pairs) where T : class, ISystem {
            for (int i = 0, length = pairs.Length; i < length; i++) {
                var pair   = pairs[i];
                var system = pair.System;
                pair.group = this.group;
                if (system != null) {
                    this.group.AddSystem(system, pair.Enabled);
                }
                else {
                    this.SystemNullError();
                }
            }
        }

        private void SystemNullError() {
            var go = this.gameObject;
            Debug.LogError($"[MORPEH] System null in installer {go.name} on scene {go.scene.name}", go);
        }

        private void RemoveSystems<T>(BasePair<T>[] pairs) where T : class, ISystem {
            for (int i = 0, length = pairs.Length; i < length; i++) {
                var system = pairs[i].System;
                if (system != null) {
                    this.group.RemoveSystem(system);
                }
            }
        }
        
#if UNITY_EDITOR
        [MenuItem("GameObject/ECS/", true, 10)]
        private static bool OrderECS() => true;

        [MenuItem("GameObject/ECS/Installer", false, 1)]
        private static void CreateInstaller(MenuCommand menuCommand) {
            var go = new GameObject("[Installer]");
            go.AddComponent<Installer>();
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
#endif
    }

    namespace Utils {
        using System;
        using JetBrains.Annotations;
        
        [Serializable]
        public abstract class BasePair<T> where T : class, ISystem {
            internal SystemsGroup group;
            
            [SerializeField]
            //[HorizontalGroup("Pair", 10)]
            [HideLabel]
            [OnValueChanged(nameof(OnChange))]
            private bool enabled;
            
#pragma warning disable CS0649
            [SerializeField]
            //[HorizontalGroup("Pair")]
            [HideLabel]
            [Required]
            [CanBeNull]
            private T system;
#pragma warning restore CS0649

            public bool Enabled {
                get => this.enabled;
                set => this.enabled = value;
            }

            [CanBeNull]
            public T System => this.system;

            public BasePair() => this.enabled = true;

            private void OnChange() {
#if UNITY_EDITOR

                if (Application.isPlaying) {
                    if (this.enabled) {
                        this.group.EnableSystem(this.system);
                    }
                    else {
                        this.group.DisableSystem(this.system);
                    }
                }
#endif
            }
        }

        [Serializable]
        public class UpdateSystemPair : BasePair<UpdateSystem> {
        }

        [Serializable]
        public class FixedSystemPair : BasePair<FixedUpdateSystem> {
        }

        [Serializable]
        public class LateSystemPair : BasePair<LateUpdateSystem> {
        }
    }
}