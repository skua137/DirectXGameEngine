using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;

namespace PrimalEditor.Utilities
{
    public enum MessageType
    {
        Info = 0x01,
        Warning = 0x02,
        Error = 0x04
    }

    public class LogMessage
    {
        public DateTime Time { get; }
        public MessageType MessageType { get; }
        public string Message { get; }
        public string File { get; }
        public string Caller { get; }
        public int Line { get; }
        public string MetaData => $"{File}: {Caller} ({Line})";

        public LogMessage(MessageType messageType, string message, string file, string caller, int line)
        {
            Time = DateTime.Now;
            MessageType = messageType;
            Message = message;
            File = System.IO.Path.GetFileName(file);
            Caller = caller;
            Line = line;
        }
    }

    public class Logger
    {
        private static int messageFilter = (int)(MessageType.Info | MessageType.Warning | MessageType.Error);   
        private static readonly ObservableCollection<LogMessage> messages = new ObservableCollection<LogMessage>();

        public static ReadOnlyObservableCollection<LogMessage> Messages { get; } = new ReadOnlyObservableCollection<LogMessage>(messages);

        public static CollectionViewSource FilteredMessages { get; } = new CollectionViewSource() { Source = messages };

        public static async void Log(
            MessageType type, string mssg,
            [CallerFilePath] string file="", [CallerMemberName] string caller="",
            [CallerLineNumber] int line =0)            
        {
            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                messages.Add(new LogMessage(type, mssg, file, caller, line));
            }));
        }

        public static async void Clear()
        {
            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                messages.Clear();
            }));
        }

        public static void SetMessageFilter(int mask)
        {
            messageFilter = mask;
            FilteredMessages.View.Refresh();
        }

        static Logger()
        {
            FilteredMessages.Filter += (s, e) => {
                var type = (int)(e.Item as LogMessage).MessageType;
                e.Accepted = (type & messageFilter) != 0;
            };
        }
    }
}
