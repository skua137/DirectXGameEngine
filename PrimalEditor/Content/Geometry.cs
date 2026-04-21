using PrimalEditor.GameProject;
using PrimalEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimalEditor.Content
{
    public enum PrimitiveMeshType
    {
        Plane,
        Cube,
        UvSphere,
        IcoSphere,
        Cylinder,
        Capsule
    }

    public class Mesh : ViewModelBase
    {
        private int vertexSize;

        public int VertexSize
        {
            get { return vertexSize; }
            set
            {
                if (vertexSize != value)
                {
                    vertexSize = value;
                    OnPropertyChanged(nameof(VertexSize));
                }
            }
        }

        private int vertexCount;

        public int VertexCount
        {
            get { return vertexCount; }
            set
            {
                if (vertexCount != value)
                {
                    vertexCount = value;
                    OnPropertyChanged(nameof(VertexCount));
                }
            }
        }

        private int indexSize;
        public int IndexSize
        {
            get { return indexSize; }
            set
            {
                if (indexSize != value)
                {
                    indexSize = value;
                    OnPropertyChanged(nameof(IndexSize));
                }
            }
        }

        private int indexCount;

        public int IndexCount
        {
            get { return indexCount; }
            set
            {
                if (indexCount != value)
                {
                    indexCount = value;
                    OnPropertyChanged(nameof(IndexCount));
                }
            }
        }

        public byte[] Vertices { get; set; }
        public byte[] Indices { get; set; }     
    }

    public class MeshLOD : ViewModelBase
    {
        private string name;

        private float _lodThreshold;

        public float LodThreshold
        {
            get { return _lodThreshold; }
            set
            {
                if (_lodThreshold != value)
                {
                    _lodThreshold = value;
                    OnPropertyChanged(nameof(LodThreshold));
                }
            }
        }


        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public ObservableCollection<Mesh> Meshes { get; } = new ObservableCollection<Mesh>();
    }

    public class LODGroup : ViewModelBase
    {
        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }


        public ObservableCollection<MeshLOD> LODs { get; } = new ObservableCollection<MeshLOD>();
    }

    public class Geometry : Asset
    {
        private readonly List<LODGroup> _lodGroups = new List<LODGroup>();

        public LODGroup GetLODGroup(int lodGroup=0)
        {
            Debug.Assert(lodGroup >=0 && lodGroup < _lodGroups.Count);
            return _lodGroups.Any() ? _lodGroups[lodGroup] : null; 
        }

        public Geometry() : base(AssetType.Mesh)
        {
        }

        public void FromRawData(byte[] data)
        {
            Debug.Assert(data?.Length > 0);
            _lodGroups.Clear();

            using var reader = new BinaryReader(new MemoryStream(data));
            //skip scene name 
            var s = reader.ReadInt32();
            reader.BaseStream.Position += s;
            var numLodGroups = reader.ReadInt32();
            Debug.Assert(numLodGroups > 0);

            for (int i = 0; i < numLodGroups; i++)
            {
                // get log group name
                s = reader.ReadInt32();
                string lodGroupName;
                if(s>0)
                {
                    var nameBytes = reader.ReadBytes(s);
                    lodGroupName = Encoding.UTF8.GetString(nameBytes);
                }
                else
                {
                    lodGroupName = $"lod_{ContentHelper.GetRandomString()}";
                }

                // get number of meshes
                var numMeshes = reader.ReadInt32();
                Debug.Assert(numMeshes > 0);
                var lods = ReadMeshLODs(numMeshes, reader);

                var lodGroup =new LODGroup() { Name = lodGroupName };
                lods.ForEach(lodItem => lodGroup.LODs.Add(lodItem));
                _lodGroups.Add(lodGroup);
            }
        }

        private List<MeshLOD> ReadMeshLODs(int numMeshes, BinaryReader reader)
        {
            var lodIds = new List<int>();
            var lodList = new List<MeshLOD>();

            for (int i = 0; i < numMeshes; i++)
            {
                ReadMeshes(reader, lodIds, lodList);
            }
            return lodList;
        }

        private void ReadMeshes(BinaryReader reader, List<int> lodIds, List<MeshLOD> lodList)
        {
            //get mesh name
            var s = reader.ReadInt32();
            string meshName;
            string lodGroupName;
            if (s > 0)
            {
                var nameBytes = reader.ReadBytes(s);
                meshName = Encoding.UTF8.GetString(nameBytes);
            }
            else
            {
                meshName = $"lod_{ContentHelper.GetRandomString()}";
            }
            var mesh = new Mesh();
            var lodId = reader.ReadInt32();
            mesh.VertexSize = reader.ReadInt32();
            mesh.VertexCount = reader.ReadInt32();
            mesh.IndexSize = reader.ReadInt32();
            mesh.IndexCount = reader.ReadInt32();
            var lodThreshold = reader.ReadInt32();

            var vertexBufferSize = mesh.VertexSize * mesh.VertexCount;
            var indexBufferSize = mesh.IndexSize * mesh.IndexCount;

            mesh.Vertices = reader.ReadBytes(vertexBufferSize);
            mesh.Indices = reader.ReadBytes(indexBufferSize);

            MeshLOD lod;
            if (ID.IsValid(lodId) && lodIds.Contains(lodId))
            {
                lod = lodList[lodIds.IndexOf(lodId)];
                Debug.Assert(lod != null);
            }
            else
            {
                lodIds.Add(lodId);
                lod = new MeshLOD();
                lodList.Add(lod);
            }
            lod.Meshes.Add(mesh);
        }
    }
}
