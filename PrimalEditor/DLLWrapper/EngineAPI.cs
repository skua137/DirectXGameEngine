using EnvDTE;
using PrimalEditor.Components;
using PrimalEditor.EngineAPIStructs;
using PrimalEditor.Utilities;
using System.Numerics;
using System.Runtime.InteropServices;

namespace PrimalEditor.EngineAPIStructs
{
    [StructLayout(LayoutKind.Sequential)]
    class TransformComponent
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale = new Vector3(1,1,1);
    }

    [StructLayout(LayoutKind.Sequential)]
    class ScriptComponent
    {
        public IntPtr ScriptCreator;
    }

    [StructLayout(LayoutKind.Sequential)]
    class GameEntityDescriptor
    {
        public TransformComponent Transform = new TransformComponent();
        public ScriptComponent Script = new ScriptComponent();
    }
}

namespace PrimalEditor.DLLWrappers
{
    static class EngineAPI
    {
        private const string _engineDllName = "EngineDLL.dll";

        [DllImport(_engineDllName, CharSet = CharSet.Ansi)]
        public static extern int LoadGameCodeDll(string dllPath);

        [DllImport(_engineDllName)]
        public static extern int UnloadGameCodeDll();

        [DllImport(_engineDllName)]
        public static extern IntPtr GetScriptCreator(string name);

        [DllImport(_engineDllName)]
        [return: MarshalAs(UnmanagedType.SafeArray)]
        public static extern string[] GetScriptNames();

        internal static class EntityAPI
        {
            [DllImport(_engineDllName)]
            private static extern int CreateGameEntity(GameEntityDescriptor desc);

            public static int CreateGameEntity(GameEntity entity)
            {
                GameEntityDescriptor descriptor = new GameEntityDescriptor();

                //transform component
                {
                    var c = entity.GetComponent<PrimalEditor.Components.Transform>();
                    descriptor.Transform.Position = c.Position;
                    descriptor.Transform.Rotation = c.Rotation;
                    descriptor.Transform.Scale = c.Scale;
                }
                //script component
                {
                    var c = entity.GetComponent<Script>();
                    if (c!=null && GameProject.Project.Current!= null)
                    {
                        if (GameProject.Project.Current.AvailableScripts.Contains(c.Name))
                        {
                            descriptor.Script.ScriptCreator = GetScriptCreator(c.Name);
                        }
                        else
                        {
                            Logger.Log(MessageType.Error, $"Unable to find the script with name {c.Name}. Game entity will be created without script component!");
                        }
                    }
                }

                return CreateGameEntity(descriptor);
            }

            [DllImport(_engineDllName)]
            private static extern void RemoveGameEntity(int id);

            public static void RemoveGameEntity(GameEntity entity)
            {
                RemoveGameEntity(entity.EntityId);
            }
        }
    }
}
