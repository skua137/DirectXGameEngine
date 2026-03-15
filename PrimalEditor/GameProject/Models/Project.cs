using PrimalEditor.Components;
using PrimalEditor.DLLWrappers;
using PrimalEditor.GameDev;
using PrimalEditor.Utilities;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Input;

namespace PrimalEditor.GameProject
{
    public enum BuildConfiguration
    {
        Debug,
        DebugEditor,
        Release,
        ReleaseEditor
    }

    [DataContract(Name ="Game")]
    public class Project : ViewModelBase
    {
        public static UndoRedo UndoRedo { get; } = new UndoRedo();
        public static string Extension { get; } = ".primal";
        [DataMember]
        public string Name { get; private set; } = "New Project";
        [DataMember]
        public string Path { get; private set; }

        public string Solution => $"{Path}{Name}.sln";
        public string FullPath => $@"{Path}{Name}{Extension}";

        private static readonly string[] _buildConfigurationNames = new string[] {
            "Debug",
            "DebugEditor",
            "Release",
            "ReleaseEditor"};

        [DataMember(Name = "Scenes")]

        private ObservableCollection<Scene> scenes = new ObservableCollection<Scene>(); 
        public ReadOnlyObservableCollection<Scene> Scenes { get; private set; }

        public ICommand AddSceneCommand { get; private set; }
        public ICommand RemoveSceneCommand { get; private set; }
        public ICommand UndoCommand { get; private set; }
        public ICommand RedoCommand { get; private set; }

        public ICommand SaveCommand { get; private set; }

        public ICommand BuildCommand { get; private set; }

        private int _buildConfig;

        public int BuildConfig
        {
            get { return _buildConfig; }
            set
            {
                if (_buildConfig != value)
                {
                    _buildConfig = value;
                    OnPropertyChanged(nameof(BuildConfig));
                }
            }
        }

        public BuildConfiguration StandAloneBuildConfig => BuildConfig == 0 ? BuildConfiguration.Debug : BuildConfiguration.Release;

        public BuildConfiguration DllBuildConfig => BuildConfig == 0 ? BuildConfiguration.DebugEditor : BuildConfiguration.ReleaseEditor;

        private Scene activeScene;
        public Scene ActiveScene
        {
            get { return activeScene; }
            set {
                if (activeScene != value)
                {
                    activeScene = value;
                    OnPropertyChanged(nameof(activeScene));
                }
            } 
        }
        
        public static Project Current => Application.Current.MainWindow.DataContext as Project;

        private string[] availableScripts;

        public string[] AvailableScripts
        {
            get { return availableScripts; }
            set
            {
                if (availableScripts != value)
                {
                    availableScripts = value;
                    OnPropertyChanged(nameof(availableScripts));
                }
            }
        }


        private void AddScene(string sceneName)
        {
            Debug.Assert(sceneName != null);
            scenes.Add(new Scene(this, sceneName));
        }

        private void RemoveScene(Scene scene)
        {
            Debug.Assert(scene != null);
            scenes.Remove(scene);
        }

        private static string GetConfigurationName(BuildConfiguration config) => _buildConfigurationNames[(int)config];

        public static Project Load(string file)
        {
            Debug.Assert(File.Exists(file));
            return Serializer.FromFile<Project>(file);
        }

        public static void Save(Project project)
        {
            Serializer.ToFile(project,project.FullPath);
            Logger.Log(MessageType.Info, $"Project saved to {project.FullPath}");
        }

        public void Unload() 
        {
            UnloadGameCodeDll();
            VisualStudio.CloseVisualStudio();
            UndoRedo.Reset();
        }

        [OnDeserialized]
        private async void OnDeserialized(StreamingContext context)
        {
            if (scenes != null)
            {
                Scenes = new ReadOnlyObservableCollection<Scene>(scenes);

                OnPropertyChanged(nameof(Scenes));
            }

            ActiveScene = Scenes.FirstOrDefault(x => x.IsActive);
            Debug.Assert(ActiveScene != null);
            await BuildGameCodeDll(false);

            SetCommands();
        }

        private void SetCommands()
        {
            AddSceneCommand = new RelayCommand<object>(x =>
            {
                AddScene($"New Scene {scenes.Count}");
                var newScene = scenes.Last();
                var sceneIndex = scenes.Count - 1;
                UndoRedo.Add(new UndoRedoAction(
                    () => RemoveScene(newScene),
                    () => scenes.Insert(sceneIndex, newScene),
                    $"Add {newScene.Name}"));
            });

            RemoveSceneCommand = new RelayCommand<Scene>(x =>
            {
                var sceneIndex = scenes.IndexOf(x);
                RemoveScene(x);

                UndoRedo.Add(new UndoRedoAction(
                    () => scenes.Insert(sceneIndex, x),
                    () => RemoveScene(x),
                    $"Remove {x.Name}"));
            }, x => !x.IsActive);

            UndoCommand = new RelayCommand<object>(x => UndoRedo.Undo(), x => UndoRedo.UndoList.Any());
            RedoCommand = new RelayCommand<object>(x => UndoRedo.Redo(), x => UndoRedo.RedoList.Any());

            SaveCommand = new RelayCommand<object>(x => Save(this));
            BuildCommand = new RelayCommand<bool>(async x => await BuildGameCodeDll(x), x => !VisualStudio.IsDebugging() && VisualStudio.BuildDone);

            OnPropertyChanged(nameof(AddSceneCommand));
            OnPropertyChanged(nameof(RemoveSceneCommand));
            OnPropertyChanged(nameof(UndoCommand));
            OnPropertyChanged(nameof(RedoCommand));
            OnPropertyChanged(nameof(SaveCommand));
            OnPropertyChanged(nameof(BuildCommand));
        }

        private async Task BuildGameCodeDll(bool showWindow = true)
        {
            try
            {
                UnloadGameCodeDll();

                // Build the game code dll
                await Task.Run (() => VisualStudio.BuildSolution(this, GetConfigurationName(DllBuildConfig), showWindow));
                if (VisualStudio.BuildSucceeded)
                {
                    // if successful -> load the game code dll again
                    LoadGameCodeDll();
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        private void LoadGameCodeDll()
        {
            var configName = GetConfigurationName(DllBuildConfig);
            var dll = $@"{Path}x64\{configName}\{Name}.dll";
            AvailableScripts = null;
            if (File.Exists(dll) && EngineAPI.LoadGameCodeDll(dll) != 0)
            {
                AvailableScripts = EngineAPI.GetScriptNames();
                ActiveScene.GameEntities.Where(x => x.GetComponent<Script>() != null).ToList().ForEach(x => x.IsActive = true);
                Logger.Log(MessageType.Info, "Game code DLL loaded successfully.");
            }
            else
                Logger.Log(MessageType.Info, "Failed to load game code DLL file. Try to build the project first.");
        }

        private void UnloadGameCodeDll()
        {
            ActiveScene.GameEntities.Where(x => x.GetComponent<Script>() != null).ToList().ForEach(x => x.IsActive = false);
            if (EngineAPI.UnloadGameCodeDll() != 0)
            {
                Logger.Log(MessageType.Info, "Game code DLL unloaded successfully.");
                AvailableScripts = null;
            }
        }

        public Project(string name, string path)
        {
            Name = name;
            Path = path;



            OnDeserialized(new StreamingContext());
        }
    }
}
