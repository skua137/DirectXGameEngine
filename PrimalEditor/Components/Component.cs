using PrimalEditor.GameProject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PrimalEditor.Components
{
    public interface IMSComponent { }

    public abstract class MSComponent<T> : ViewModelBase, IMSComponent where T :Component
    {
        private bool _enableUpdates = true;

        public List<T> SelectedComponents { get; }

        protected abstract bool UpdateComponents(string propertyName);

        protected abstract bool UpdateMSComponent();

        public void Refresh()
        {
            _enableUpdates = false;
            UpdateMSComponent();
            _enableUpdates = true;
        }

        public MSComponent(MSEntity msEntity)
        {
            Debug.Assert(msEntity.SelectedEntities?.Any() == true);
            SelectedComponents = msEntity.SelectedEntities.Select(x=>x.GetComponent<T>()).ToList();
            PropertyChanged += (s, e) => { if (_enableUpdates) UpdateComponents(e.PropertyName); };
        }
    }

    [DataContract]
    public abstract class Component : ViewModelBase
    {
        public abstract IMSComponent GetMultiSelectionComponent(MSEntity mSEntity);
        [DataMember]
        public GameEntity Owner { get; private set; }   

        public Component(GameEntity owner)
        {
            Debug.Assert(owner != null);
            Owner = owner;
        }   
    }
}
