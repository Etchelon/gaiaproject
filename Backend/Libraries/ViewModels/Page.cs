using System.Collections.Generic;

namespace GaiaProject.ViewModels
{
    public class Page<T>
    {
        public T[] Items { get; set; }
        public bool HasMore { get; set; }
    }
}