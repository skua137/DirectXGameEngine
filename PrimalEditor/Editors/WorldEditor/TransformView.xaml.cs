using PrimalEditor.Components;
using PrimalEditor.GameProject;
using PrimalEditor.Utilities;
using System.Diagnostics;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PrimalEditor.Editors
{
    /// <summary>
    /// Interaction logic for TransformView.xaml
    /// </summary>
    public partial class TransformView : UserControl
    {
        private Action _undoAction = null;
        private bool _propertyChanged = false;
        public TransformView()
        {
            InitializeComponent();
            Loaded += TransformView_Loaded;
        }

        private void TransformView_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= TransformView_Loaded;
            (DataContext as MSTransform).PropertyChanged += (s, e) => _propertyChanged = true;
        }


        private Action GetAction(
            Func<Transform, (Transform transform, Vector3)> selector, 
            Action <(Transform transform, Vector3)> forEachAction)
        {
            if (!(DataContext is MSTransform vm))
            {
                _undoAction = null;
                _propertyChanged = false;
                return null;
            }
            var selection = vm.SelectedComponents.Select(transform => (transform, transform.Position)).ToList();
            return new Action(() =>
            {
                selection.ForEach(item => item.transform.Position = item.Position);
                (GameEntityView.Instance.DataContext as MSEntity)?.GetMSComponent<MSTransform>().Refresh();
            });
        }

        private Action GetPositionAction() => GetAction((x) => (x, x.Position), (x) => x.transform.Position = x.Item2);
        private Action GetRotationAction() => GetAction((x) => (x, x.Rotation), (x) => x.transform.Rotation = x.Item2);
        private Action GetScaleAction() => GetAction((x) => (x, x.Scale), (x) => x.transform.Scale = x.Item2);

        private void RecordActions(Action redoAction, string name)
        {
            if (_propertyChanged)
            {
                Debug.Assert(_undoAction != null);
                _propertyChanged = false;
                Project.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction, name));
            }
        }
        private void VectorBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _propertyChanged = false;
            _undoAction = GetPositionAction();
        }

        private void VectorBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RecordActions(GetPositionAction(), "Position changed.");
        }

        private void VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_propertyChanged && _undoAction != null)
            {
                VectorBox_PreviewMouseLeftButtonUp(sender, null);
            }
        }

        private void VectorBox_PreviewMouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            _propertyChanged = false;
            _undoAction = GetRotationAction();
        }

        private void VectorBox_PreviewMouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            RecordActions(GetRotationAction(), "Rotation changed.");
        }

        private void VectorBox_PreviewMouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            _propertyChanged = false;
            _undoAction = GetScaleAction();
        }

        private void VectorBox_PreviewMouseLeftButtonUp_2(object sender, MouseButtonEventArgs e)
        {
            RecordActions(GetScaleAction(), "Scale changed.");
        }

        private void VectorBox_LostKeyboardFocus_1(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_propertyChanged && _undoAction != null)
            {
                VectorBox_PreviewMouseLeftButtonUp_1(sender, null);
            }
        }

        private void VectorBox_LostKeyboardFocus_2(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_propertyChanged && _undoAction != null)
            {
                VectorBox_PreviewMouseLeftButtonUp_2(sender, null);
            }
        }
    }
}
