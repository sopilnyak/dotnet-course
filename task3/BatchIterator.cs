/*
    Задача 1 (5 баллов)

    1. Реализовать класс BatchIterator<T>, который позволяет итерироваться по произвольной коллекции IEnumerable<T>
    батчами фиксированного размера. При этом класс не должен хранить все создаваемые им батчи, их нужно создавать отложенно.
    Предлагается самостоятельно продумать методы класса так, чтобы им было удобно пользоваться.
    2. Написать с помощью NUnit тесты на класс BatchIterator<T>.
    
    Пример сценария использования: допустим, у нас есть большой массив файлов, которые необходимо прочитать, скопировать
    в память и что-то с ними сделать. Все файлы одновременно в память не влезут, а читать их по одному (например, из сети)
    тоже неоптимально. Мы знаем, что одновременно в память влезут BatchSize файлов и хотим использовать класс BatchIterator<T>
    для удобной обработки этих файлов.
    В реализации вам пригодится ключевое слово yield.
*/

using System.Collections.Generic;
using System.Collections;

public class BatchIterator<T> : IEnumerable<IEnumerable<T>>
{
    class Batch : IEnumerable<T>
    {
        List<T> values;
        IEnumerator<T> inner;
        int targetSize;
        public Batch(T first_element, IEnumerator<T> inner, int size)
        {
            this.values = new List<T>();
            this.values.Add(first_element);
            this.inner = inner;
            this.targetSize = size;
        }

        public IEnumerator<T> GetEnumerator()
        {
            int currentSize = 0;
            while (currentSize < this.targetSize)
            {
                if (currentSize < this.values.Count)
                {
                    yield return this.values[currentSize];
                }
                else if (this.inner.MoveNext())
                {
                    this.values.Add(this.inner.Current);
                    yield return this.values[currentSize];
                }

                currentSize += 1;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    int batchSize;
    IEnumerable<T> inner;
    public BatchIterator(IEnumerable<T> inner, int batchSize)
    {
        if (batchSize <= 0)
        {
            throw new ArgumentOutOfRangeException("batchSize must be positive");
        }
        this.inner = inner;
        this.batchSize = batchSize;
    }


    public IEnumerator<IEnumerable<T>> GetEnumerator()
    {
        var iter = this.inner.GetEnumerator();
        while (iter.MoveNext())
        {
            var next = iter.Current;
            yield return new Batch(next, iter, this.batchSize);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}