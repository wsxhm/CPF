using System;
using System.Collections.Generic;
using System.Text;

namespace CPF
{
    public interface IBehaviorObject
    {
        void BehaviorTo(CpfObject cpfObject);
        void DetachFrom(CpfObject cpfObject);
    }

    public abstract class Behavior : CpfObject, IBehaviorObject
    {
        protected Behavior() : this(typeof(CpfObject))
        {
        }
        internal Behavior(Type associatedType) => AssociatedType = associatedType ?? throw new ArgumentNullException(nameof(associatedType));

        protected Type AssociatedType { get; }

        void IBehaviorObject.BehaviorTo(CpfObject cpfObject)
        {
            if (cpfObject == null)
                throw new ArgumentNullException(nameof(cpfObject));
            if (!AssociatedType.IsInstanceOfType(cpfObject))
                throw new InvalidOperationException("object not an instance of AssociatedType");
            OnBehaviorTo(cpfObject);
        }

        void IBehaviorObject.DetachFrom(CpfObject cpfObject) => OnDetachingFrom(cpfObject);

        protected virtual void OnBehaviorTo(CpfObject cpfObject)
        {

        }

        protected virtual void OnDetachingFrom(CpfObject cpfObject)
        {

        }
    }

    public abstract class Behavior<T> : Behavior where T : CpfObject
    {
        protected Behavior() : base(typeof(T))
        {
        }

        protected override void OnBehaviorTo(CpfObject cpfObject)
        {
            base.OnBehaviorTo(cpfObject);
            OnBehaviorTo((T)cpfObject);
        }

        protected virtual void OnBehaviorTo(T cpfObject)
        {
        }

        protected override void OnDetachingFrom(CpfObject cpfObject)
        {
            OnDetachingFrom((T)cpfObject);
            base.OnDetachingFrom(cpfObject);
        }

        protected virtual void OnDetachingFrom(T cpfObject)
        {
        }
    }
}
