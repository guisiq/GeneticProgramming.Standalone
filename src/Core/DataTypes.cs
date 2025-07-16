using System;
using GeneticProgramming.Core;

namespace GeneticProgramming.Data
{
    /// <summary>
    /// Base class for value types
    /// </summary>
    public abstract class ValueTypeValue<T> : Item where T : struct
    {
        private T value;

        public virtual T Value
        {
            get => value;
            set
            {
                if (!this.value.Equals(value))
                {
                    this.value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        protected ValueTypeValue() : base() { }

        protected ValueTypeValue(T value) : base()
        {
            this.value = value;
        }

        protected ValueTypeValue(ValueTypeValue<T> original, Cloner cloner) : base(original, cloner)
        {
            value = original.value;
        }

        public override string ToString()
        {
            return value.ToString() ?? string.Empty;
        }
    }

    /// <summary>
    /// Wrapper for integer values
    /// </summary>
    [Item("IntValue", "Represents an integer value")]
    public class IntValue : ValueTypeValue<int>
    {
        public IntValue() : base() { }
        public IntValue(int value) : base(value) { }
        protected IntValue(IntValue original, Cloner cloner) : base(original, cloner) { }

        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new IntValue(this, cloner);
        }

        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new IntValue(this, cloner);
        }
    }

    /// <summary>
    /// Wrapper for double values
    /// </summary>
    [Item("DoubleValue", "Represents a double value")]
    public class DoubleValue : ValueTypeValue<double>
    {
        public DoubleValue() : base() { }
        public DoubleValue(double value) : base(value) { }
        protected DoubleValue(DoubleValue original, Cloner cloner) : base(original, cloner) { }

        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new DoubleValue(this, cloner);
        }

        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new DoubleValue(this, cloner);
        }
    }

    /// <summary>
    /// Wrapper for boolean values
    /// </summary>
    [Item("BoolValue", "Represents a boolean value")]
    public class BoolValue : ValueTypeValue<bool>
    {
        public BoolValue() : base() { }
        public BoolValue(bool value) : base(value) { }
        protected BoolValue(BoolValue original, Cloner cloner) : base(original, cloner) { }

        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new BoolValue(this, cloner);
        }

        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new BoolValue(this, cloner);
        }
    }

    /// <summary>
    /// Wrapper for string values
    /// </summary>
    [Item("StringValue", "Represents a string value")]
    public class StringValue : Item
    {
        private string value = string.Empty;

        public virtual string Value
        {
            get => value;
            set
            {
                if (this.value != value)
                {
                    this.value = value ?? string.Empty;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        public StringValue() : base() { }
        public StringValue(string value) : base() { this.value = value ?? string.Empty; }
        protected StringValue(StringValue original, Cloner cloner) : base(original, cloner)
        {
            value = original.value;
        }

        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new StringValue(this, cloner);
        }

        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new StringValue(this, cloner);
        }

        public override string ToString()
        {
            return value;
        }
    }
}
