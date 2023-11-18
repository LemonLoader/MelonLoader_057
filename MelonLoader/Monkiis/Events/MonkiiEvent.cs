using System;
using System.Collections.Generic;
using System.Reflection;

namespace MonkiiLoader
{
    public abstract class MonkiiEventBase<T> where T : Delegate
    {
        private readonly List<MonkiiAction<T>> actions = new();
        private MonkiiAction<T>[] cachedActionsArray = new MonkiiAction<T>[0];
        public readonly bool oneTimeUse;

        public bool Disposed { get; private set; }

        public MonkiiEventBase(bool oneTimeUse = false)
        {
            this.oneTimeUse = oneTimeUse;
        }

        public bool CheckIfSubscribed(MethodInfo method, object obj = null)
        {
            lock (actions)
            {
                return actions.Exists(x => x.del.Method == method && (obj == null || x.del.Target == obj));
            }
        }

        public void Subscribe(T action, int priority = 0, bool unsubscribeOnFirstInvocation = false)
        {
            if (Disposed)
                return;

            lock (actions)
            {
                var acts = MonkiiAction<T>.Get(action, priority, unsubscribeOnFirstInvocation);
                foreach (var a in acts)
                {
                    if (CheckIfSubscribed(a.del.Method, a.del.Target))
                        continue;

                    if (a.MonkiiAssembly != null)
                    {
                        MonkiiDebug.Msg($"MonkiiAssembly '{a.MonkiiAssembly.Assembly.GetName().Name}' subscribed with {a.del.Method.Name}");
                        a.MonkiiAssembly.OnUnregister.Subscribe(() => Unsubscribe(a.del), unsubscribeOnFirstInvocation: true);
                    }

                    for (var b = 0; b < actions.Count; b++)
                    {
                        var act = actions[b];
                        if (a.priority < act.priority)
                        {
                            actions.Insert(b, a);
                            UpdateEnumerator();
                            return;
                        }
                    }

                    actions.Add(a);
                    UpdateEnumerator();
                }
            }
        }

        public void Unsubscribe(T action)
        {
            foreach (var inv in action.GetInvocationList())
            {
                Unsubscribe(inv.Method, inv.Target);
            }
        }

        public void UnsubscribeAll()
        {
            lock (actions)
                actions.Clear();

            UpdateEnumerator();
        }

        public void Unsubscribe(MethodInfo method, object obj = null)
        {
            if (Disposed)
                return;

            lock (actions)
            {
                if (method.IsStatic)
                    obj = null;

                var any = false;
                for (var a = 0; a < actions.Count; a++)
                {
                    var act = actions[a];
                    if (act.del.Method != method || (obj != null && act.del.Target != obj))
                        continue;

                    any = true;
                    actions.RemoveAt(a);
                    if (act.MonkiiAssembly != null)
                        MonkiiDebug.Msg($"MonkiiAssembly '{act.MonkiiAssembly.Assembly.GetName().Name}' unsubscribed with {act.del.Method.Name}");
                }

                if (any)
                    UpdateEnumerator();
            }
        }

        private void UpdateEnumerator()
        {
            cachedActionsArray = actions.ToArray();
        }

        public class MonkiiEventSubscriber
        {
            public T del;
            public bool unsubscribeOnFirstInvocation;
            public int priority;
            public MonkiiAssembly MonkiiAssembly;
        }
        public MonkiiEventSubscriber[] GetSubscribers()
        {
            List<MonkiiEventSubscriber> allSubs = new List<MonkiiEventSubscriber>();
            foreach (var act in actions)
                allSubs.Add(new MonkiiEventSubscriber
                {
                    del = act.del,
                    unsubscribeOnFirstInvocation = act.unsubscribeOnFirstInvocation,
                    priority = act.priority,
                    MonkiiAssembly = act.MonkiiAssembly
                });
            return allSubs.ToArray();
        }

        protected void Invoke(Action<T> delegateInvoker)
        {
            if (Disposed)
                return;

            var actionsArray = cachedActionsArray;
            for (var a = 0; a < actionsArray.Length; a++)
            {
                var del = actionsArray[a];
                try { delegateInvoker(del.del); }
                catch (Exception ex)
                {
                    if (del.MonkiiAssembly == null || del.MonkiiAssembly.LoadedMonkiis.Count == 0)
                        MonkiiLogger.Error(ex.ToString());
                    else
                        del.MonkiiAssembly.LoadedMonkiis[0].LoggerInstance.Error(ex.ToString());
                }

                if (del.unsubscribeOnFirstInvocation)
                    Unsubscribe(del.del);
            }

            if (oneTimeUse)
                Dispose();
        }

        public void Dispose()
        {
            UnsubscribeAll();
            Disposed = true;
        }
    }

    #region Param Children
    public class MonkiiEvent : MonkiiEventBase<LemonAction>
    {
        public MonkiiEvent(bool oneTimeUse = false) : base(oneTimeUse) { }

        public void Invoke()
        {
            Invoke(x => x());
        }
    }
    public class MonkiiEvent<T1> : MonkiiEventBase<LemonAction<T1>>
    {
        public MonkiiEvent(bool oneTimeUse = false) : base(oneTimeUse) { }

        public void Invoke(T1 arg1)
        {
            Invoke(x => x(arg1));
        }
    }
    public class MonkiiEvent<T1, T2> : MonkiiEventBase<LemonAction<T1, T2>>
    {
        public MonkiiEvent(bool oneTimeUse = false) : base(oneTimeUse) { }

        public void Invoke(T1 arg1, T2 arg2)
        {
            Invoke(x => x(arg1, arg2));
        }
    }
    public class MonkiiEvent<T1, T2, T3> : MonkiiEventBase<LemonAction<T1, T2, T3>>
    {
        public MonkiiEvent(bool oneTimeUse = false) : base(oneTimeUse) { }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3)
        {
            Invoke(x => x(arg1, arg2, arg3));
        }
    }
    public class MonkiiEvent<T1, T2, T3, T4> : MonkiiEventBase<LemonAction<T1, T2, T3, T4>>
    {
        public MonkiiEvent(bool oneTimeUse = false) : base(oneTimeUse) { }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Invoke(x => x(arg1, arg2, arg3, arg4));
        }
    }
    public class MonkiiEvent<T1, T2, T3, T4, T5> : MonkiiEventBase<LemonAction<T1, T2, T3, T4, T5>>
    {
        public MonkiiEvent(bool oneTimeUse = false) : base(oneTimeUse) { }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            Invoke(x => x(arg1, arg2, arg3, arg4, arg5));
        }
    }
    public class MonkiiEvent<T1, T2, T3, T4, T5, T6> : MonkiiEventBase<LemonAction<T1, T2, T3, T4, T5, T6>>
    {
        public MonkiiEvent(bool oneTimeUse = false) : base(oneTimeUse) { }
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            Invoke(x => x(arg1, arg2, arg3, arg4, arg5, arg6));
        }
    }
    public class MonkiiEvent<T1, T2, T3, T4, T5, T6, T7> : MonkiiEventBase<LemonAction<T1, T2, T3, T4, T5, T6, T7>>
    {
        public MonkiiEvent(bool oneTimeUse = false) : base(oneTimeUse) { }
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            Invoke(x => x(arg1, arg2, arg3, arg4, arg5, arg6, arg7));
        }
    }
    public class MonkiiEvent<T1, T2, T3, T4, T5, T6, T7, T8> : MonkiiEventBase<LemonAction<T1, T2, T3, T4, T5, T6, T7, T8>>
    {
        public MonkiiEvent(bool oneTimeUse = false) : base(oneTimeUse) { }
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            Invoke(x => x(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8));
        }
    }
    #endregion
}
