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
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PrimalEditor.Editors
{
    /// <summary>
    /// Interaction logic for GeometryView.xaml
    /// </summary>
    public partial class GeometryView : UserControl
    {
        private Point _clickedPosition;
        private bool _captureLeft;
        private bool _capturedRight;

        public GeometryView()
        {
            InitializeComponent();
            DataContextChanged += (s, e) => SetGeometry();
        }

        private void SetGeometry(int index = -1)
        {
            if (!(DataContext is MeshRenderer vm)) return;

            if(vm.Meshes.Any() && viewport.Children.Count ==2)
            {
                viewport.Children.RemoveAt(1);
            }

            var meshIndex = 0;
            var modelGroup = new Model3DGroup();
            foreach (var mesh in vm.Meshes)
            {
                if (index != -1 && meshIndex != index)
                {
                    ++meshIndex;
                    continue;
                }

                var mesh3D = new MeshGeometry3D()
                {
                    Positions = mesh.Positions,
                    Normals = mesh.Normals,
                    TriangleIndices = mesh.Indices,
                    TextureCoordinates = mesh.UVs
                };
                var diffuse = new DiffuseMaterial(mesh.Diffuse);
                var specular = new SpecularMaterial(mesh.Specular, 50);
                var matGroup = new MaterialGroup();
                matGroup.Children.Add(diffuse);
                matGroup.Children.Add(specular);

                var model = new GeometryModel3D(mesh3D, matGroup);

                modelGroup.Children.Add(model);

                var binding = new Binding(nameof(mesh.Diffuse)) { Source = mesh };
                BindingOperations.SetBinding(diffuse, DiffuseMaterial.BrushProperty, binding);

                if (meshIndex == index) break;
            }
            var visual = new ModelVisual3D() { Content = modelGroup };
            viewport.Children.Add(visual);
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //_clickedPosition = e.GetPosition(this);
            //_captureLeft = true;
            //Mouse.Capture(sender as UIElement);
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            //if (!_captureLeft && !_capturedRight) return;

            //var pos = e.GetPosition(this);
            //var d = pos - _clickedPosition;

            //if (_captureLeft && !_capturedRight)
            //{
            //    MoveCamera(d.X, d.Y, 0);
            //}
            //else if (!_captureLeft && _capturedRight)
            //{
            //    var vm = DataContext as MeshRenderer;
            //    var cp = vm.CameraDirection;
            //    var yoffset = d.Y * 0.001 * Math.Sqrt(cp.X * cp.X + cp.Z * cp.Z);
            //    vm.CameraTarget = new Point3D(vm.CameraTarget.X, vm.CameraTarget.Y + yoffset, vm.CameraTarget.Z);
            //}
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //_captureLeft = false;
            //if (!_capturedRight) Mouse.Capture(null);
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {

        }

        private void Grid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
