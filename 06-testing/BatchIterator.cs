namespace Testing
{

    using System.Collections.Generic;
    public class BatchIterator<T>
    {
        public static IEnumerable<T[]> GetNextBatch(IEnumerable<T> data, int batchSize)
        {
            List<T> lst = new List<T>();
            int len = 0;
            foreach (var elem in data)
            {
                lst.Add(elem);
                len++;
                if (len == batchSize)
                {
                    yield return lst.ToArray();
                    lst = new List<T>();
                    len = 0;
                }
            }
            if (len > 0)
            {
                yield return lst.ToArray();
            }
        }
    }
}
