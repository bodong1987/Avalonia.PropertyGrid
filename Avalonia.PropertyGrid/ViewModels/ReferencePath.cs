using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.PropertyGrid.ViewModels
{
    public class ReferencePath
    {
        public readonly Stack<string> Paths = new Stack<string>();

        public int Count => Paths.Count;

        public void BeginScope(string scope)
        {
            Paths.Push(scope);
        }

        public void EndScope()
        {
            Paths.Pop();
        }

        public override string ToString()
        {
            return string.Join(", ", Paths.ToArray());
        }
    }
}
