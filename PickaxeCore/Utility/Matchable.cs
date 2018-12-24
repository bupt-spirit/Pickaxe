using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PickaxeCore.Utility.Matchable
{
    public abstract class Matchable<A, B> {
        // private ctor ensures no external classes can inherit
        private Matchable() { }

        public delegate T DoIfMatchA<T>(ref A item);
        public delegate T DoIfMatchB<T>(ref B item);
        public delegate void DoIfMatchA(ref A item);
        public delegate void DoIfMatchB(ref B item);
        public abstract T Match<T>(DoIfMatchA<T> doIfMatchA, DoIfMatchB<T> doIfMatchB);
        public abstract void Match(DoIfMatchA doIfMatchA, DoIfMatchB doIfMatchB);

        public class TypeA : Matchable<A, B>
        {
            private A item;
            public TypeA(A item) { this.item = item; }
            public override T Match<T>(DoIfMatchA<T> doIfMatchA, DoIfMatchB<T> doIfMatchB) =>
                doIfMatchA(ref item);
            public override void Match(DoIfMatchA doIfMatchA, DoIfMatchB doIfMatchB) =>
                doIfMatchA(ref item);
        }

        public class TypeB : Matchable<A, B>
        {
            private B item;
            public TypeB(B item) { this.item = item; }
            public override T Match<T>(DoIfMatchA<T> doIfMatchA, DoIfMatchB<T> doIfMatchB) =>
                doIfMatchB(ref item);
            public override void Match(DoIfMatchA doIfMatchA, DoIfMatchB doIfMatchB) =>
                doIfMatchB(ref item);
        }
    }
}
