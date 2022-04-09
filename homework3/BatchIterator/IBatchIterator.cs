using System.Collections.Generic;

namespace Homework3.BatchIterator
{
    // для dependency injection
    public interface IBatchIterator<T>
    {
        public uint BatchSize { get; set; }
        public IEnumerable<T> DataToBatch { get; set; }

        public IEnumerator<T[]> GetEnumerator();
    }
}