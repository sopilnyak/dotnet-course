using System;

namespace HW1
{
    public interface IArraySort<T> where T : IComparable<T>
    {
        void Sort();
    }
}