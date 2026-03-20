using PrimalEditor.Utilities;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace PrimalEditor.GameProject
{
    public class NewProjectViewModel : ViewModelBase
    {
        private string name, projectPath;
        private readonly string _templatePath = @"ProjectTemplates\";
        public NewProjectViewModel() 
        {
            try
            {
                name = "NewProject";
                projectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\PrimalProjects\";
                //projectPath = $@"C:\PrimalProjects\";
                var templateFiles = Directory.GetFiles(_templatePath, "template.xml", SearchOption.AllDirectories);
                Debug.Assert(templateFiles.Any());
                foreach (var templateFile in templateFiles)
                {
                    var template = Serializer.FromFile<ProjectTemplate>(templateFile);
                    template.TemplatePath = Path.GetDirectoryName(templateFile);
                    template.ProjectFilePath = Path.GetFullPath(Path.Combine(template.TemplatePath, template.ProjectFile));
    
                    _projectTemplates.Add(template);
                }
                selectedProjectTemplate = _projectTemplates[0];
                ValidateProjectPath();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Failed to read project templates");
                throw;
            }
        }



        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    ValidateProjectPath();
                    OnPropertyChanged("Name");
                }
            }
        }

        public string ProjectPath
        {
            get { return projectPath; }
            set
            {
                if (projectPath != value)
                {
                    projectPath = value;
                    ValidateProjectPath();
                    OnPropertyChanged(nameof(ProjectPath));
                }
            }
        }

        private bool _isValid;

        public bool IsValid
        {
            get { return _isValid; }
            set 
            {
                if (_isValid != value)
                {
                    _isValid = value;
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        private string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set 
            { 
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged(nameof(ErrorMessage));
                }
            }
        }

        private ProjectTemplate selectedProjectTemplate;

        public ProjectTemplate SelectedProjectTemplate
        {
            get { return selectedProjectTemplate; }
            set { selectedProjectTemplate = value; OnPropertyChanged(nameof(selectedProjectTemplate)); }
        }



        public ObservableCollection<ProjectTemplate> _projectTemplates = new ObservableCollection<ProjectTemplate>();

        public ObservableCollection<ProjectTemplate> ProjectTemplates => _projectTemplates;

        private bool ValidateProjectPath()
        {
            var path = ProjectPath;
            if (!Path.EndsInDirectorySeparator(path))
            {
                path += @"\";
            }
            path += $@"{Name}\";

            IsValid = false;
            if (string.IsNullOrWhiteSpace(Name.Trim()))
            {
                ErrorMessage = "Type in a project name.\n";
            }
            else if (Name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                ErrorMessage = "Invalid character(s) in a project name.\n";
            }
            else if (ProjectPath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                ErrorMessage = "Invalid character(s) in a project path.\n";
            }
            else if (Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any())
            {
                ErrorMessage = "Selected project folder already exists and is not empty.";
            }
            else
            {
                ErrorMessage = string.Empty;
                IsValid = true;
            }
            return IsValid;
        }

        public string CreateProject(ProjectTemplate template)
        {
            ValidateProjectPath();
            if (!IsValid)
            {
                return string.Empty;
            }

            if (!Path.EndsInDirectorySeparator(ProjectPath))
            {
                ProjectPath += @"\";
            }
            var path = $@"{ProjectPath}{Name}\";
            try
            {
                if(!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                foreach (var folder in template.Folders)
                {
                    Directory.CreateDirectory(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), folder))); 
                }
                var dirInfo = new DirectoryInfo(path + @".Primal\");
                dirInfo.Attributes |= FileAttributes.Hidden;
                // copy icon
                // copy screenshot

                var projectXml = File.ReadAllText(template.ProjectFilePath);
                projectXml = String.Format(projectXml, Name, path);
                var projectPath = Path.GetFullPath(Path.Combine(path, $"{Name}{Project.Extension}"));
                File.WriteAllText(projectPath, projectXml);

                CreateMSVCSolution(template, path);

                return path;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Failed to create {template.ProjectType}");
                throw;
            }
        }

        private void CreateMSVCSolution(ProjectTemplate template, string path)
        {
            Debug.Assert(File.Exists(Path.Combine(template.TemplatePath, "MSVCSolution")));
            Debug.Assert(File.Exists(Path.Combine(template.TemplatePath, "MSVCProject")));

            var projpath = $@"{ProjectPath}{Name}\";
            var engineAPIPath = Path.Combine(MainWindow.PrimalPath, @"Engine\EngineAPI\");
            Debug.Assert(Directory.Exists(engineAPIPath));
            var _0 = Name;
            var _1 = "{" + Guid.NewGuid().ToString().ToUpper() + "}";

            var solution = File.ReadAllText(Path.Combine(template.TemplatePath, "MSVCSolution"));
            solution = string.Format(solution, _0, _1, "{" + Guid.NewGuid().ToString().ToUpper() + "}");
            File.WriteAllText(Path.GetFullPath(Path.Combine(projpath, $"{_0}.sln")), solution);

            var _2 = engineAPIPath;
            var _3 = MainWindow.PrimalPath;

            var project = File.ReadAllText(Path.Combine(template.TemplatePath, "MSVCProject"));
            project = string.Format(project, _0, _1, _2, _3);
            File.WriteAllText(Path.GetFullPath(Path.Combine(projpath, $@"GameCode\{_0}.vcxproj")), project);
        }
    }
}
