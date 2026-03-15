using PrimalEditor.Components;
using PrimalEditor.GameProject;
using PrimalEditor.Utilities;
using System.Windows;
using System.Windows.Controls;

namespace PrimalEditor.Editors
{
    /// <summary>
    /// Interaction logic for ProjectLayoutView.xaml
    /// </summary>
    public partial class ProjectLayoutView : UserControl
    {
        public ProjectLayoutView()
        {
            InitializeComponent();
        }

        private void OnAddGameEntityButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var vm = btn.DataContext as Scene;
            vm.AddGameEntityCommand.Execute(new GameEntity(vm) { Name = "Empty Game Entity" });
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (e.AddedItems.Count > 0)
            //{
            //    GameEntityView.Instance.DataContext = (sender as ListBox).SelectedItems[0];
            //}

            var listbox = sender as ListBox;


            var newSelection = listbox.SelectedItems.Cast<GameEntity>().ToList();
            var prevSelection = newSelection.Except(e.AddedItems.Cast<GameEntity>()).Concat(e.RemovedItems.Cast<GameEntity>()).ToList();

            Project.UndoRedo.Add(new UndoRedoAction(
                () =>
                {
                    listbox.UnselectAll();
                    prevSelection.ForEach(item => (listbox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem).IsSelected = true);
                },
                () => 
                {
                    listbox.UnselectAll();
                    newSelection.ForEach(item => (listbox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem).IsSelected = true);
                },
                "Selection Changed"
                ));

            MSGameEntity msGameEntity = null;
            if (newSelection.Any())
            {
                msGameEntity = new MSGameEntity(newSelection);
            }
            GameEntityView.Instance.DataContext = msGameEntity;
        }
    }
}
