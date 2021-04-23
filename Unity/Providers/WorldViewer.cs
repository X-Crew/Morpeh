using Frigg;

namespace Morpeh {
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    
    [HideMonoScript]
    public class WorldViewer : MonoBehaviour {
        //[DisableContextMenu]
        [PropertySpace]
        [ShowInInspector]
        [Order(-1)]
        //[HideReferenceObjectPickerAttribute]
        [ListDrawerSettings(AllowDrag = false, HideAddButton = true, HideRemoveButton = true)]
        private List<EntityView> Entities {
            get {
                if (Application.isPlaying) {
                    if (World.Default.entitiesCount != this.entityViews.Count) {
                        this.entityViews.Clear();
                        for (int i = 0, length = World.Default.entitiesLength; i < length; i++) {
                            var entity = World.Default.entities[i];
                            if (entity != null) {
                                var view = new EntityView {ID = entity.internalID, entityViewer = {getter = () => entity}};
                                this.entityViews.Add(view);
                            }
                        }
                    }
                }

                return this.entityViews;
            }
            set { }
        }

        private readonly List<EntityView> entityViews = new List<EntityView>();

        //[DisableContextMenu]
        [Serializable]
        protected internal class EntityView {
            [Readonly]
            public int ID;
            
            [ShowInInspector]
            internal Editor.EntityViewer entityViewer = new Editor.EntityViewer();
        }
    }
}