using PrimalEditor.GameDev;
using PrimalEditor.GameProject;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace PrimalEditor.Editors
{
    /// <summary>
    /// Interaction logic for WorldEditorView.xaml
    /// </summary>
    public partial class WorldEditorView : UserControl
    {
        public WorldEditorView()
        {
            InitializeComponent();
            Loaded += WorldEditorView_Loaded;
        }

        private void WorldEditorView_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= WorldEditorView_Loaded;
            Focus();
        }

        private void OnNewScript_Button_Click(object sender, RoutedEventArgs e)
        {
            new NewScriptDialog().ShowDialog();
        }

        private void OnBuild_Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
