using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WixSharpSetup
{
    public class SourceFile
    {
        [Flags]
        public enum Action
        {
            None = 0,
            CopyToFtp = 1,
            ShortCut = 2,
            Install32 = 4,
            Install64 = 8,
            Install = 16,
            Version = 32
        }
        public string Filename { get; private set; }
        public string FullPath => System.IO.Path.GetFullPath(Filename);
	    public string Name => System.IO.Path.GetFileName(Filename);
        public Action Actions { get; private set; }

        public SourceFile(string filename, Action action)
        {
            Filename = filename;
            Actions = action;
        }

        public SourceFile(string filename, IEnumerable<Action> actions)
        {
            Actions = actions.Aggregate((a1, a2) => a1 | a2);
        }

        public bool HaveAction(Action action)
        {
            return (Actions & action) == action;
        }

        public static List<SourceFile> NewList(params SourceFile[] files)
        {
            return new List<SourceFile>(files);
        }
    }
}
