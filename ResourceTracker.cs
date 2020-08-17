using System;
using System.Collections.Generic;

namespace LearnOpenGL
{
    public class ResourceTracker : Disposable
    {
        private List<Disposable> items = new List<Disposable>();

        public ResourceTracker()
        {
        }

        public T Add<T>(T item) where T : Disposable
        {
            if (item != null)
            {
                this.items.Add(item);
            }
            return item;
        }

        protected override void CleanupDisposableObjects()
        {
            // Dispose in reverse order
            for (int i = this.items.Count - 1; i >= 0; i--)
            {
                if (this.items[i] != null)
                {
                    this.items[i].Dispose();
                    this.items[i] = null;
                }
            }
            this.items.Clear();
        }

        protected override void CleanupUnmanagedResources()
        {
            // Nothing.
        }
    }
}
