using System.Collections.Generic;

namespace Homework3.BatchIterator
{
    // Если бы не задание, я бы сделал BatchIterator функцией, а не классом.
    // Поэтому тут есть странный generic, проставленный у класса, 
    // а не у самого метода Iterate.

    // Для dependency injection
    public interface IBatchIterator<T>
    {
        public int BatchSize { get; set; }

        public IEnumerable<T[]> Iterate(IEnumerable<T> data_to_batch);
    }
}