using System.Windows;
using System.Windows.Controls;

namespace PrimalEditor.GameProject
{
    /// <summary>
    /// Interaction logic for OpenProjectView.xaml
    /// </summary>
    public partial class OpenProjectView : UserControl
    {
        public OpenProjectView()
        {
            InitializeComponent();
            this.DataContext = new OpenProjectViewModel();

            Loaded += OpenProjectView_Loaded;
        }

        private void OpenProjectView_Loaded(object sender, RoutedEventArgs e)
        {
            var item = projectsListbox.ItemContainerGenerator.ContainerFromIndex(projectsListbox.SelectedIndex) as ListBoxItem;
            item?.Focus();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenSelectedProject();
        }

        private void OpenSelectedProject()
        {
            var project = OpenProjectViewModel.Open(projectsListbox.SelectedItem as ProjectData);
            bool dialogResult = false;
            var win = Window.GetWindow(this);
            if (project != null)
            {
                dialogResult = true; 
                win.DataContext = project;
            }
            win.DialogResult = dialogResult;
            win.Close();
        }

        private void ListBoxItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenSelectedProject();
        }
    }
}
