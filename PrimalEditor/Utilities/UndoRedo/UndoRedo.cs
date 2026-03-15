using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimalEditor.Utilities
{
    public interface IUndoRedo
    {
        string Name { get; }
        void Undo();
        void Redo();
    }

    public class UndoRedoAction : IUndoRedo
    {
        private Action undoAction;
        private Action redoAction;
        public string Name { get; }

        public void Redo() => redoAction();

        public void Undo() => undoAction();

        public UndoRedoAction(string name)
        {
            Name = name;
        }

        public UndoRedoAction(Action undo, Action redo, string name)
        {
            Debug.Assert(undo != null && redo != null);
            undoAction = undo;
            redoAction = redo;
            Name = name;
        }

        public UndoRedoAction(string property, object instance, object undoValue, object redoValue, string name)
            : this(
                () => instance.GetType().GetProperty(property).SetValue(instance, undoValue),
                () => instance.GetType().GetProperty(property).SetValue(instance, redoValue),
                name)
        {

        }
    }

    public class UndoRedo
    {
        private bool enableAdd = true;
        private readonly ObservableCollection<IUndoRedo> redoList = new ObservableCollection<IUndoRedo>();
        private readonly ObservableCollection<IUndoRedo> undoList = new ObservableCollection<IUndoRedo>();

        public ReadOnlyObservableCollection<IUndoRedo> RedoList { get; }
        public ReadOnlyObservableCollection<IUndoRedo> UndoList { get; }

        public void Reset()
        {
            redoList.Clear(); 
            undoList.Clear(); 
        }

        public UndoRedo()
        {
            RedoList = new ReadOnlyObservableCollection<IUndoRedo>(redoList);
            UndoList = new ReadOnlyObservableCollection<IUndoRedo>(undoList);
        }

        public void Undo()
        {
            if (undoList.Any())
            {
                var cmd = undoList.Last();
                undoList.RemoveAt(undoList.Count - 1);
                enableAdd = false;
                cmd.Undo();
                enableAdd = true;
                redoList.Insert(0,cmd);
            }
        }

        public void Redo()
        {
            if (redoList.Any())
            {
                var cmd = redoList.First();
                redoList.RemoveAt(0);
                enableAdd = false;
                cmd.Redo();
                enableAdd = true;
                undoList.Add(cmd);
            }
        }

        public void Add(IUndoRedo cmd)
        {
            if (enableAdd)
            {
                undoList.Add(cmd);
                redoList.Clear();
            }
        }
    }
}
