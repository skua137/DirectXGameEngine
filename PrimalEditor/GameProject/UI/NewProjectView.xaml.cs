using System.Windows;
using System.Windows.Controls;

namespace PrimalEditor.GameProject
{
    /// <summary>
    /// Interaction logic for NewProjectView.xaml
    /// </summary>
    public partial class NewProjectView : UserControl
    {
        public NewProjectView()
        {
            InitializeComponent();
            this.DataContext = new NewProjectViewModel();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = DataContext as NewProjectViewModel;
            var projectPath = vm.CreateProject(TemplateListbox.SelectedItem as ProjectTemplate);
            bool dialogResult = false;
            var win = Window.GetWindow(this);
            if (!String.IsNullOrEmpty(projectPath))
            {
                dialogResult = true;
                win.DataContext = OpenProjectViewModel.Open(new ProjectData() { ProjectName = vm.Name, ProjectPath = projectPath });
            }
            win.DialogResult = dialogResult;
            win.Close();
        }
    }
}
