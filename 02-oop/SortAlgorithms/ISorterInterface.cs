using System;
using System.Diagnostics;
using System.IO;

namespace Homework1
{
    interface ISorter {
        void Sort<T>(T[] items) where T : IComparable;
    }
}