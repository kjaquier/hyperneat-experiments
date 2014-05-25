using SharpNeat.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SharpNeat.Experiments.Common
{
    public static class Utils
    {
        /// <summary>
        /// Python-style list slicing
        /// Source : http://extensionmethod.net/csharp/ienumerable-t/slice-t-int-start-int-end
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static IEnumerable<T> Slice<T>(this IEnumerable<T> collection, int start, int end)
        {
            int index = 0;
            int count = 0;

            if (collection == null)
                throw new ArgumentNullException("collection");

            // Optimise item count for ICollection interfaces.
            if (collection is ICollection<T>)
                count = ((ICollection<T>)collection).Count;
            else if (collection is ICollection)
                count = ((ICollection)collection).Count;
            else
                count = collection.Count();

            // Get start/end indexes, negative numbers start at the end of the collection.
            if (start < 0)
                start += count;

            if (end < 0)
                end += count;

            foreach (var item in collection)
            {
                if (index >= end)
                    yield break;

                if (index >= start)
                    yield return item;

                ++index;
            }
        }

        /// <summary>
        /// Get the index of the highest value of a sequence.
        /// Source : http://stackoverflow.com/questions/462699/how-do-i-get-the-index-of-the-highest-value-in-an-array-using-linq
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public static int MaxIndex<T>(this IEnumerable<T> sequence)
            where T : IComparable<T>
        {
            int maxIndex = -1;
            T maxValue = default(T); // Immediately overwritten anyway

            int index = 0;
            foreach (T value in sequence)
            {
                if (value.CompareTo(maxValue) > 0 || maxIndex == -1)
                {
                    maxIndex = index;
                    maxValue = value;
                }
                index++;
            }
            return maxIndex;
        }


        public static IEnumerable<T> Flatten<T>(this IList<IList<T>> list)
        {
            foreach (var row in list)
            {
                foreach (var item in row)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Create a new array with an new element append at the end
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="elem"></param>
        /// <returns></returns>
        public static T[] Extend<T>(this T[] array, T elem)
        {
            var n = array.Count();
            T[] newArray = new T[n + 1];
            array.CopyTo(newArray, 0);
            newArray[n] = elem;
            return newArray;
        }

        public static int Median(this IList<int> seq)
        {
            return seq[seq.Count() / 2];
        }

        public static double Median(this IList<double> seq)
        {
            return seq[seq.Count() / 2];
        }

        public static double Mean(this IEnumerable<int> seq)
        {
            return seq.Sum() / seq.Count();
        }

        public static double Mean(this IEnumerable<double> seq)
        {
            return seq.Sum() / seq.Count();
        }

        public static void Shuffle(this IList list, FastRandom rng)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var j = rng.Next(list.Count);
                var tmp = list[i];
                list[i] = list[j];
                list[j] = tmp;
            }
        }

        public static IEnumerable<T> Shuffled<T>(this IEnumerable<T> collection, FastRandom rng)
        {
            var list = collection.ToList();
            for (var i = 0; i < list.Count; i++)
            {
                var j = rng.Next(list.Count);
                var tmp = list[i];
                list[i] = list[j];
                list[j] = tmp;
            }
            return list;
        }

        /// <summary>
        /// Creates a 2D matrix with the given number of columns and fill
        /// it with the enumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="nbColumns"></param>
        /// <returns></returns>
        public static T[][] ToMatrix<T>(this IEnumerable<T> collection, int nbColumns)
        {
            Debug.Assert(nbColumns > 0, "Nb columns must be positive!");

            var mat = new List<T[]>();
            var col = 0;
            List<T> row = new List<T>();

            foreach (var item in collection)
            {
                row.Add(item);
                col = (col + 1) % nbColumns;
                if (col == 0)
                {
                    mat.Add(row.ToArray());
                    row = new List<T>();
                }
            }

            // If there's still a remaning empty line, we must add it
            if (row.Count() > 0)
            {
                mat.Add(row.ToArray());
            }

            return mat.ToArray<T[]>();

        }
    }
}
