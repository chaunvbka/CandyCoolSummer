#pragma warning disable IDE0130


namespace Texell.CoreModule.Model
{
    using System;

    public abstract class BaseModel : IDisposable
    {
        protected bool _disposed = false;

        public abstract void Dispose();
    }
}