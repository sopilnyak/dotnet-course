using System.Collections;

static class Program
 {
     static void Main(string[] args)
     {
     }
}


public class BatchIterator<T> : IEnumerable
{
    private readonly IEnumerable<T> collection_;
    private int batchSize_;
    private readonly T[] batch;

    public BatchIterator(IEnumerable<T> collection, int batch_size)
    {
        collection_ = collection;
        batchSize_ = batch_size;
        batch = new T[batch_size];
    }

    public IEnumerator GetEnumerator()
    {
        List<T> nextbatch = new List<T>(batchSize_);
        foreach (T item in collection_)
        {
            nextbatch.Add(item);
            if (nextbatch.Count == batchSize_)
            {
                yield return nextbatch;
                nextbatch.Clear();
            }
        }
        if (nextbatch.Count > 0)
            yield return nextbatch;
    }
}









/*public class BatchIterator<T> : IEnumerator
{
    //private readonly IEnumerable<T> collection_;
    private IEnumerator IEnumerator_; 
    private int batch_size_;
    private readonly T[] batch_;
    private int position_ = -1;

    public BatchIterator(IEnumerable<T> collection, int batch_size)
    {
        //collection_ = collection;
        IEnumerator_ = collection.GetEnumerator();
        batch_size_ = batch_size;
        batch_ = new T[batch_size];
    }

    public bool MoveNext()
    {
        return IEnumerator_.MoveNext();
    }

    public void Reset()
    {
        IEnumerator_.Reset();
    }

    public object Current
    {
        get
        {
            batch_[0] = IEnumerator_.Current.get;
            if (!MoveNext())
                throw new ArgumentException();
            while ()
            return _activities[_position];
        }
    }
}*/
