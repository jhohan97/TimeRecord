using System;
using System.Collections.Generic;
using System.Text;

namespace TimeRecord.Tests.Helpers
{
    public class NullScope : IDisposable
    {
        public static NullScope instance { get; } = new NullScope();

        public void Dispose() {}

        private NullScope() { }
    }
}
