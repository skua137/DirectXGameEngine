using PrimalEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace PrimalEditor.GameProject
{
    [DataContract]
    public class ProjectData
    {
        [DataMember]
        public string ProjectName { get; set; }
        [DataMember]
        public string ProjectPath { get; set; }
        [DataMember]
        public DateTime Date { get; set; }

        public string FullPath { get => $"{ProjectPath}{ProjectName}{Project.Extension}";}
    }

    [DataContract]
    public class ProjectDataList
    {
        [DataMember]
        public List<ProjectData> Projects { get; set; }
    }

    public class OpenProjectViewModel
    {
        private static readonly string _applicationDataPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\PrimalEditor\";
        private static readonly string _projectDataPath;
        private static readonly ObservableCollection<ProjectData> projects = new ObservableCollection<ProjectData>();   

        public static ReadOnlyObservableCollection<ProjectData> Projects { get; }

        static OpenProjectViewModel()
        {
            try
            {
                if (!Directory.Exists(_applicationDataPath))
                {
                    Directory.CreateDirectory(_applicationDataPath);
                }
                _projectDataPath = $@"{_applicationDataPath}ProjectData.xml";
                Projects = new ReadOnlyObservableCollection<ProjectData>(projects);
                ReadProjectData();
            }   
            catch (Exception ex)
            { 
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Failed to read project data");
                throw;
            }
        }

        private static void ReadProjectData()
        {
            if (File.Exists(_projectDataPath))
            {
                var projectList = Serializer.FromFile<ProjectDataList>(_projectDataPath).Projects.OrderByDescending(x=> x.Date);
                projects.Clear();
                foreach (var project in projectList)
                {
                    if (File.Exists(project.FullPath))
                    {
                        projects.Add(project);
                    }
                }
            }
        }

        private static void WriteProjectData()
        {
            var projectList = projects.OrderBy(x=>x.Date).ToList();
            Serializer.ToFile(new ProjectDataList { Projects = projectList }, _projectDataPath);
        }

        public static Project Open(ProjectData projectData)
        {
            ReadProjectData();
            var project = projects.FirstOrDefault(x => x.FullPath.Equals(projectData.FullPath));
            if (project != null)
            {
                project.Date = DateTime.Now;
            }
            else
            {
                project = projectData;
                project.Date = DateTime.Now;
                projects.Add(project);
            }
            WriteProjectData();

            return Project.Load(project.FullPath);
        }

    }
}
