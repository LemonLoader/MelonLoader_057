using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MonkiiLoader
{
    public abstract class MonkiiTypeBase<T> : MonkiiBase where T : MonkiiTypeBase<T>
    {
        /// <summary>
        /// List of registered <typeparamref name="T"/>s.
        /// </summary>
        new public static ReadOnlyCollection<T> RegisteredMonkiis => _registeredMonkiis.AsReadOnly();
        new internal static List<T> _registeredMonkiis = new();

        /// <summary>
        /// A Human-Readable Name for <typeparamref name="T"/>.
        /// </summary>
        public static string TypeName { get; protected internal set; }

        static MonkiiTypeBase()
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle); // To make sure that the type initializer of T was triggered.
        }

        public sealed override string MonkiiTypeName => TypeName;

        protected private override bool RegisterInternal()
        {
            if (!base.RegisterInternal())
                return false;
            _registeredMonkiis.Add((T)this);
            return true;
        }

        protected private override void UnregisterInternal()
        {
            _registeredMonkiis.Remove((T)this);
        }

        public static void ExecuteAll(LemonAction<T> func, bool unregisterOnFail = false, string unregistrationReason = null)
            => ExecuteList(func, _registeredMonkiis, unregisterOnFail, unregistrationReason);
    }
}
