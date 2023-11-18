using System;
using System.Collections.Generic;

namespace MonkiiLoader
{
    internal class MonkiiAction<T> where T : Delegate
    {
        internal readonly T del;
        internal readonly bool unsubscribeOnFirstInvocation;
        internal readonly int priority;

        internal readonly MonkiiAssembly MonkiiAssembly;

        private MonkiiAction(T singleDel, int priority, bool unsubscribeOnFirstInvocation)
        {
            del = singleDel;
            MonkiiAssembly = MonkiiAssembly.GetMonkiiAssemblyOfMember(del.Method, del.Target);
            this.priority = priority;
            this.unsubscribeOnFirstInvocation = unsubscribeOnFirstInvocation;
        }

        internal static List<MonkiiAction<T>> Get(T del, int priority = 0, bool unsubscribeOnFirstInvocation = false)
        {
            var mets = del.GetInvocationList();
            var result = new List<MonkiiAction<T>>();
            foreach (var met in mets)
            {
                if (met.Target != null && met.Target is MonkiiBase Monkii && !Monkii.Registered)
                    continue;

                result.Add(new MonkiiAction<T>((T)met, priority, unsubscribeOnFirstInvocation));
            }
            return result;
        }
    }
}
