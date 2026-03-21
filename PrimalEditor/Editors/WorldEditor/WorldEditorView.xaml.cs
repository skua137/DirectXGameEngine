using PrimalEditor.GameDev;
using PrimalEditor.GameProject;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;

namespace PrimalEditor.Editors
{
    public class NullableStringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value == 0 ? "Debug" : "Release";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var comboBoxItem = value as ComboBoxItem;
            if (comboBoxItem != null)
            {
                var tempString = comboBoxItem.Content.ToString();
                if (String.IsNullOrEmpty(tempString))
                {
                    return 0;
                }
                else
                {
                    if (tempString != "Debug")
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            else
                return 0;
        }
    }

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
