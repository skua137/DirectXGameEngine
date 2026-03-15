using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PrimalEditor.GameProject
{
    /// <summary>
    /// Interaction logic for ProjectBrowserDialog.xaml
    /// </summary>
    public partial class ProjectBrowserDialog : Window
    {
        private CubicEase _easing = new CubicEase() { EasingMode = EasingMode.EaseInOut};
        public ProjectBrowserDialog()
        {
            InitializeComponent();
            btnOpenProject.IsChecked = true;
            Loaded += ProjectBrowserDialog_Loaded;
        }

        private void ProjectBrowserDialog_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= ProjectBrowserDialog_Loaded;
            if (!OpenProjectViewModel.Projects.Any())
            {
                btnOpenProject.IsEnabled = false;
                openProjectView.Visibility = Visibility.Hidden;
                btnOpenProject_Click(btnNewProject, new RoutedEventArgs());
                btnNewProject_Click(btnNewProject, new RoutedEventArgs());
            }
        }

        private void btnOpenProject_Click(object sender, RoutedEventArgs e)
        {
            if (btnNewProject.IsChecked == true)
            {
                btnNewProject.IsChecked = false;
                AnimateToOpenProject();
                openProjectView.IsEnabled = true;
                newProjectView.IsEnabled = false;
            }
            btnOpenProject.IsChecked = true;
        }

        private void btnNewProject_Click(object sender, RoutedEventArgs e)
        {
            if (btnOpenProject.IsChecked == true)
            {
                btnOpenProject.IsChecked = false;
                AnimateToCreateProject();
                newProjectView.IsEnabled = true;
                openProjectView.IsEnabled = false;
            }
            btnNewProject.IsChecked = true;
        }

        private void AnimateToCreateProject()
        {
            var highlightAnimation = new DoubleAnimation(200, 400, new Duration(TimeSpan.FromSeconds(0.2)));
            highlightAnimation.Completed += (s, e) =>
            {
                var animation = new ThicknessAnimation(new Thickness(0), new Thickness(-1600,0,0,0), new Duration(TimeSpan.FromSeconds(0.5)));
                animation.EasingFunction = _easing;
                BrowserContent.BeginAnimation(MarginProperty, animation);
            };
            highlightRect.BeginAnimation(Canvas.LeftProperty, highlightAnimation);
        }

        private void AnimateToOpenProject()
        {
            var highlightAnimation = new DoubleAnimation(400, 200, new Duration(TimeSpan.FromSeconds(0.2)));
            highlightAnimation.Completed += (s, e) =>
            {
                var animation = new ThicknessAnimation(new Thickness(-1600, 0, 0, 0), new Thickness(0), new Duration(TimeSpan.FromSeconds(0.5)));
                animation.EasingFunction = _easing;
                BrowserContent.BeginAnimation(MarginProperty, animation);
            };
            highlightRect.BeginAnimation(Canvas.LeftProperty, highlightAnimation);
        }
               
    }
}
