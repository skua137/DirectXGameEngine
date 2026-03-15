using PrimalEditor.Components;
using PrimalEditor.Utilities;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows.Input;

namespace PrimalEditor.GameProject
{
    [DataContract]
    public class Scene : ViewModelBase
    {
        public Scene(Project project, string name)
        {
            Debug.Assert(project != null);
            Project = project;
            Name = name;
            OnDeserialized(new StreamingContext());
        }

        private string name;
        [DataMember]
		public string Name
		{
			get { return name; }
			set {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
              
            }
		}

        [DataMember]
        public Project Project { get; set; }


        private bool isActive;
        [DataMember]
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                if (isActive != value)
                {
                    isActive = value;
                    OnPropertyChanged(nameof(IsActive));
                }

            }
        }
        public ICommand AddGameEntityCommand { get; private set; }
        public ICommand RemoveGameEntityCommand { get; private set; }

        [DataMember(Name = "GameEntities")]
        private ObservableCollection<GameEntity> gameEntities = new ObservableCollection<GameEntity>();

        public ReadOnlyObservableCollection<GameEntity> GameEntities { get; private set; }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (gameEntities != null)
            {
                GameEntities = new ReadOnlyObservableCollection<GameEntity>(gameEntities);

                OnPropertyChanged(nameof(GameEntities));
            }

            foreach (GameEntity entity in gameEntities)
            {
                entity.IsActive = IsActive;
            }


            AddGameEntityCommand = new RelayCommand<GameEntity>(x =>
            {
                AddGameEntity(x);
                var entityIndex = gameEntities.Count - 1;
                Project.UndoRedo.Add(new UndoRedoAction(
                    () => RemoveGameEntity(x),
                    () => AddGameEntity(x, entityIndex),
                    $"Add {x.Name} to {Name}"));
            });

            RemoveGameEntityCommand = new RelayCommand<GameEntity>(x =>
            {
                var entityIndex = gameEntities.IndexOf(x);
                RemoveGameEntity(x);

                Project.UndoRedo.Add(new UndoRedoAction(
                    () => AddGameEntity(x, entityIndex),
                    () => RemoveGameEntity(x),
                    $"Remove {x.Name}"));
            });

        }

        private void AddGameEntity(GameEntity gameEntity, int index = -1)
        {
            Debug.Assert(gameEntity != null && !gameEntities.Contains(gameEntity));
            gameEntity.IsActive = IsActive;
            if (index == -1)
            {
                gameEntities.Add(gameEntity);
            }
            else
            {
                gameEntities.Insert(index, gameEntity);
            }
        }

        private void RemoveGameEntity(GameEntity gameEntity)
        {
            Debug.Assert(gameEntity != null && gameEntities.Contains(gameEntity));
            gameEntity.IsActive = false;
            gameEntities.Remove(gameEntity);
        }
    }
}
