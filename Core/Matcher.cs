using System;
using UnityEngine;

namespace Atlas
{
    public class Requires<T> : IMatcher where T : Component
    {
        public bool Matches(GameObject entity)
        {
            return entity.GetComponent<T>() != null;
        }
    }

    public class RequiresNone: IMatcher
    {
        public bool Matches(GameObject entity)
        {
            return false;
        }
    }

    public class Accepts<T> : IMatcher where T : Component
    {
        public bool Matches(GameObject entity)
        {
            return true;
        }
    }

    public class Rejects<T> : IMatcher where T : Component
    {
        public bool Matches(GameObject entity)
        {
            return entity.GetComponent<T>() == null;
        }
    }

    public class Matcher<T1> : IMatcher where T1 : IMatcher
    {
        T1 matcher1;

        public Matcher()
        {
            matcher1 = Activator.CreateInstance<T1>();
        }

        public bool Matches(GameObject entity)
        {
            return matcher1.Matches(entity);
        }
    }

    public class Matcher<T1, T2> : IMatcher
        where T1 : IMatcher
        where T2 : IMatcher
    {
        T1 matcher1;
        T2 matcher2;

        public Matcher()
        {
            matcher1 = Activator.CreateInstance<T1>();
            matcher2 = Activator.CreateInstance<T2>();
        }

        public bool Matches(GameObject entity)
        {
            return matcher1.Matches(entity) 
                && matcher2.Matches(entity);
        }
    }

    public class Matcher<T1, T2, T3> : IMatcher
        where T1 : IMatcher
        where T2 : IMatcher
        where T3 : IMatcher
    {
        T1 matcher1;
        T2 matcher2;
        T3 matcher3;

        public Matcher()
        {
            matcher1 = Activator.CreateInstance<T1>();
            matcher2 = Activator.CreateInstance<T2>();
            matcher3 = Activator.CreateInstance<T3>();
        }

        public bool Matches(GameObject entity)
        {
            return matcher1.Matches(entity) 
                && matcher2.Matches(entity) 
                && matcher3.Matches(entity);
        }
    }

    public class Matcher<T1, T2, T3, T4> : IMatcher
        where T1 : IMatcher
        where T2 : IMatcher
        where T3 : IMatcher
        where T4 : IMatcher
    {
        T1 matcher1;
        T2 matcher2;
        T3 matcher3;
        T4 matcher4;

        public Matcher()
        {
            matcher1 = Activator.CreateInstance<T1>();
            matcher2 = Activator.CreateInstance<T2>();
            matcher3 = Activator.CreateInstance<T3>();
            matcher4 = Activator.CreateInstance<T4>();
        }

        public bool Matches(GameObject entity)
        {
            return matcher1.Matches(entity) 
                && matcher2.Matches(entity) 
                && matcher3.Matches(entity)
                && matcher4.Matches(entity);
        }
    }

    public class Matcher<T1, T2, T3, T4, T5> : IMatcher
        where T1 : IMatcher
        where T2 : IMatcher
        where T3 : IMatcher
        where T4 : IMatcher
        where T5 : IMatcher
    {
        T1 matcher1;
        T2 matcher2;
        T3 matcher3;
        T4 matcher4;
        T5 matcher5;

        public Matcher()
        {
            matcher1 = Activator.CreateInstance<T1>();
            matcher2 = Activator.CreateInstance<T2>();
            matcher3 = Activator.CreateInstance<T3>();
            matcher4 = Activator.CreateInstance<T4>();
            matcher5 = Activator.CreateInstance<T5>();
        }

        public bool Matches(GameObject entity)
        {
            return matcher1.Matches(entity)
                && matcher2.Matches(entity)
                && matcher3.Matches(entity)
                && matcher4.Matches(entity)
                && matcher5.Matches(entity);
        }
    }
}
