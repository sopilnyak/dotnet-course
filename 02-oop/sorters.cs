namespace sorters; 

public interface ISorter<T> {
    public void Sort(T[] arr);
}

public class BubbleSort<T> : ISorter<T> where T: IComparable {
    public void Sort(T[] arr) {
        var swapped = true;

        while (swapped) {
            swapped = false;
            for (var i = 0; i < arr.Length - 1; ++i) {
                if (arr[i].CompareTo(arr[i + 1]) <= 0) continue;
                (arr[i], arr[i + 1]) = (arr[i + 1], arr[i]);
                swapped = true;
            }
        }
    }
}

public class QuickSort<T> : ISorter<T> where T: IComparable {
    public void Sort(T[] arr) {
        QuickSortRecursion(arr, 0, arr.Length - 1);
    }

    private int Partition(T[] arr, int startIdx, int endIdx) {
        T pivot = arr[endIdx];

        var realPivotIdx = startIdx - 1;
            
        for (var i = startIdx; i <= endIdx - 1; ++i) {
            if (arr[i].CompareTo(pivot) < 0) {
                ++realPivotIdx;
                (arr[realPivotIdx], arr[i]) = (arr[i], arr[realPivotIdx]);
            }
        }

        (arr[realPivotIdx + 1], arr[endIdx]) = (arr[endIdx], arr[realPivotIdx + 1]);
        return realPivotIdx + 1;
    }
        
    private void QuickSortRecursion(T[] arr, int startIdx, int endIdx) {
        if (startIdx >= endIdx) {
            return;
        }
        int newParts = Partition(arr, startIdx, endIdx);
        QuickSortRecursion(arr, startIdx, newParts - 1);
        QuickSortRecursion(arr, newParts + 1, endIdx);
    }
}
