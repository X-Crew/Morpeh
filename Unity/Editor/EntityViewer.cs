using Frigg;

#if UNITY_EDITOR
namespace Morpeh.Editor {
    using System;
    using System.Collections.Generic;
    using Collections;
    using Morpeh;
    using UnityEngine;

    [Serializable]
    [InlineProperty]
    //[HideReferenceObjectPicker]
    [HideLabel]
    internal class EntityViewer {
        internal Func<Entity> getter = () => null;
        private  Entity       entity => this.getter();

        private readonly List<ComponentView> componentViews = new List<ComponentView>();

        /*[PropertySpace]
        [ShowInInspector]
        private List<JustSomeTest> someTest = new List<JustSomeTest>();

        [PropertySpace]
        [ShowInInspector]
        private List<JustSomeTest> someTestProp {
            get {
                return new List<JustSomeTest> {
                    new JustSomeTest {
                        publicShownStr = "test",
                        SomeInternalStuff = "123"
                    },
                    new JustSomeTest {
                        publicShownStr    = "test",
                        SomeInternalStuff = "123"
                    },
                    new JustSomeTest {
                        publicShownStr    = "test",
                        SomeInternalStuff = "123"
                    }
                };
            }
            set { }
        }*/

        [Serializable]
        private struct JustSomeTest {
            
            [PropertyTooltip("Hi!")]
            [ShowInInspector]
            internal string SomeInternalStuff { get; set; }

            private string notShown;

            public string publicShownStr;
            
            [ShowInInspector]
            private int privateInt;
        }

        //[DisableContextMenu]
        [PropertySpace]
        [ShowInInspector]
        //[HideReferenceObjectPickerAttribute]
        [ListDrawerSettings(AllowDrag = false, HideAddButton = true, HideRemoveButton = true)]
        private List<ComponentView> ComponentsOnEntity {
            get {
                this.componentViews.Clear();
                if (this.entity != null) {
                    foreach (var slotIndex in this.entity.componentsIds) {
                        var slot = this.entity.componentsIds.GetKeyByIndex(slotIndex);
                        var data = this.entity.componentsIds.GetValueByIndex(slotIndex);
                        var view = new ComponentView {
                            internalTypeDefinition = CommonTypeIdentifier.intTypeAssociation[slot],
                            id                     = data,
                            world                  = this.entity.world
                        };
                        this.componentViews.Add(view);
                    }
                }

                return this.componentViews;
            }
            set { }
        }

        [PropertyTooltip("$" + nameof(FullName))]
        [Serializable]
        private struct ComponentView {
            internal CommonTypeIdentifier.InternalTypeDefinition internalTypeDefinition;

            internal World world;

            internal bool   IsMarker => this.internalTypeDefinition.typeInfo.isMarker;
            internal string FullName => this.internalTypeDefinition.type.FullName;

            [ShowIf(nameof(IsMarker))]
            [HideLabel]
            //[DisplayAsString(false)]
            [ShowInInspector]
            internal string TypeName => this.internalTypeDefinition.type.Name;

            internal int id;

            //[DisableContextMenu]
            [HideIf(nameof(IsMarker), Condition = true)]
            //[LabelText("$" + nameof(TypeName))]
            [ShowInInspector]
            //[HideReferenceObjectPickerAttribute]
            public object Data {
                get {
                    if (this.internalTypeDefinition.typeInfo.isMarker || Application.isPlaying == false) {
                        return null;
                    }

                    return this.internalTypeDefinition.cacheGetComponentBoxed(this.world, this.id);
                }
                set {
                    if (this.internalTypeDefinition.typeInfo.isMarker || Application.isPlaying == false) {
                        return;
                    }

                    this.internalTypeDefinition.cacheSetComponentBoxed(this.world, this.id, value);
                }
            }
        }
    }

    //[HideReferenceObjectPicker]
    [Serializable]
    [InlineProperty]
    [HideLabel]
    [Title("Debug info", drawLine = true)]
    internal class EntityViewerWithHeader : EntityViewer {
    }
}
#endif