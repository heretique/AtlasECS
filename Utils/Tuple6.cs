// ----------------------------------------------------------------------------
// Tuple structs for use in .NET Not-Quite-3.5 (e.g. Unity3D).
//
// Used Chapter 3 in http://functional-programming.net/ as a starting point.
//
// Note: .NET 4.0 Tuples are immutable classes so they're *slightly* different.
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Atlas
{
    /// <summary>
    /// Represents a functional tuple that can be used to store
    /// two values of different types inside one object.
    /// </summary>
    /// <typeparam name="T1">The type of the first element</typeparam>
    /// <typeparam name="T2">The type of the second element</typeparam>
    /// <typeparam name="T3">The type of the third element</typeparam>
    /// <typeparam name="T4">The type of the fourth element</typeparam>
    /// <typeparam name="T5">The type of the fifth element</typeparam>
    /// <typeparam name="T6">The type of the sixth element</typeparam>
    public sealed class Tuple<T1, T2, T3, T4, T5, T6>
    {
        private readonly T1 item1;
        private readonly T2 item2;
        private readonly T3 item3;
        private readonly T4 item4;
        private readonly T5 item5;
        private readonly T6 item6;

        /// <summary>
        /// Retyurns the first element of the tuple
        /// </summary>
        public T1 Item1
        {
            get { return item1; }
        }

        /// <summary>
        /// Returns the second element of the tuple
        /// </summary>
        public T2 Item2
        {
            get { return item2; }
        }

        /// <summary>
        /// Returns the second element of the tuple
        /// </summary>
        public T3 Item3
        {
            get { return item3; }
        }

        /// <summary>
        /// Returns the second element of the tuple
        /// </summary>
        public T4 Item4
        {
            get { return item4; }
        }

        /// <summary>
        /// Returns the fifth element of the tuple
        /// </summary>
        public T5 Item5
        {
            get { return item5; }
        }


        /// <summary>
        /// Returns the sixth element of the tuple
        /// </summary>
        public T6 Item6
        {
            get { return item6; }
        }

        /// <summary>
        /// Create a new tuple value
        /// </summary>
        /// <param name="item1">First element of the tuple</param>
        /// <param name="second">Second element of the tuple</param>
        /// <param name="third">Third element of the tuple</param>
        /// <param name="fourth">Fourth element of the tuple</param>
        /// <param name="fifth">Fifth element of the tuple</param>
        /// <param name="sixth">Sixth element of the tuple</param>
        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
        {
            this.item1 = item1;
            this.item2 = item2;
            this.item3 = item3;
            this.item4 = item4;
            this.item5 = item5;
            this.item6 = item6;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + (item1 == null ? 0 : item1.GetHashCode());
            hash = hash * 23 + (item2 == null ? 0 : item2.GetHashCode());
            hash = hash * 23 + (item3 == null ? 0 : item3.GetHashCode());
            hash = hash * 23 + (item4 == null ? 0 : item4.GetHashCode());
            hash = hash * 23 + (item5 == null ? 0 : item5.GetHashCode());
            hash = hash * 23 + (item6 == null ? 0 : item5.GetHashCode());
            return hash;
        }

        public override bool Equals(object o)
        {
            if (o.GetType() != typeof(Tuple<T1, T2, T3, T4, T5, T6>)) {
                return false;
            }

            var other = (Tuple<T1, T2, T3, T4, T5, T6>)o;

            return this == other;
        }

        public static bool operator==(Tuple<T1, T2, T3, T4, T5, T6> a, Tuple<T1, T2, T3, T4, T5, T6> b)
        {
            if (object.ReferenceEquals(a, null)) {
                return object.ReferenceEquals(b, null);
            }
            if (a.item1 == null && b.item1 != null) return false;
            if (a.item2 == null && b.item2 != null) return false;
            if (a.item3 == null && b.item3 != null) return false;
            if (a.item4 == null && b.item4 != null) return false;
            if (a.item5 == null && b.item5 != null) return false;
            if (a.item6 == null && b.item6 != null) return false;
            return
                a.item1.Equals(b.item1) &&
                a.item2.Equals(b.item2) &&
                a.item3.Equals(b.item3) &&
                a.item4.Equals(b.item4) &&
                a.item5.Equals(b.item5) &&
                a.item6.Equals(b.item6);
        }

        public static bool operator!=(Tuple<T1, T2, T3, T4, T5, T6> a, Tuple<T1, T2, T3, T4, T5, T6> b)
        {
            return !(a == b);
        }
    }
}
