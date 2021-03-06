using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TV_Lab
{
    public sealed class ThreadRandom : Random
    {
        public ThreadRandom(int _val) : base(_val) { }
        public ThreadRandom() : base() { }

        private object _lock = new object();

        public override int Next() { lock (_lock) return base.Next(); }

        public override int Next(int maxValue) { lock (_lock) return base.Next(maxValue); }

        public override int Next(int minValue, int maxValue) { lock (_lock) return base.Next(minValue, maxValue); }

        public override void NextBytes(byte[] buffer) { lock (_lock) base.NextBytes(buffer); }

        public override double NextDouble() { lock (_lock) return base.NextDouble(); }
    }
}
