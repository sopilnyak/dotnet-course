namespace hw3.Task1;

public static class BatchIterator<T> {
    public static IEnumerable<T[]> NextBatch(IEnumerable<T> data, int batch_size, bool give_remain = true) {
        T[] batch = Array.Empty<T>();
        int cur_idx = 0;

        foreach (var item in data) {
            if (batch.Length == 0) {
                Array.Resize(ref batch, batch_size);
            }

            batch[cur_idx++] = item;

            if (batch.Length == batch_size) {
                yield return batch;

                batch = Array.Empty<T>();
                cur_idx = 0;
            }
        }

        if (cur_idx != 0 && give_remain) {
            Array.Resize(ref batch, cur_idx);
            yield return batch;
        }
    }
}
