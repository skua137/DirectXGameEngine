using System.Runtime.Serialization;

namespace PrimalEditor.GameProject
{
    [DataContract]
    public class ProjectTemplate
    {
        public ProjectTemplate(string projectType, string projectFile, List<string> folders)
        {
            ProjectType = projectType;
            ProjectFile = projectFile;
            Folders = folders;
        }

        [DataMember]
        public string ProjectType { get; private set; }
        [DataMember]
        public string ProjectFile { get; private set; }
        [DataMember]
        public List<string> Folders { get; private set; }

        public string ProjectFilePath { get; set; }
        public string TemplatePath { get; set; }
    }
}
