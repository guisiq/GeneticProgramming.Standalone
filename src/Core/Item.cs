using System;
using System.ComponentModel;
using System.Reflection;
using GeneticProgramming.Core;
using GeneticProgramming.Abstractions.Parameters; // For IParameterCollection

namespace GeneticProgramming.Core
{
    /// <summary>
    /// Base implementation of IItem
    /// </summary>
    [Item("Item", "Base class for all items")]
    public abstract class Item : IItem
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string name = string.Empty;
        private string description = string.Empty;
        private IParameterCollection? _parameters; // Added for IOperator

        public virtual string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public virtual string Description
        {
            get => description;
            set
            {
                if (description != value)
                {
                    description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public virtual string ItemName => GetType().Name;
        public virtual string ItemDescription => GetType().Name;
        public virtual Version ItemVersion => new Version(1, 0);
        public virtual string ItemImage => string.Empty;

        // Implementation for IOperator
        public IParameterCollection? Parameters 
        {
            get => _parameters;
            set
            {
                if (_parameters != value)
                {
                    _parameters = value;
                    OnPropertyChanged(nameof(Parameters));
                }
            }
        }

        protected Item()
        {
            var attr = GetType().GetCustomAttribute<ItemAttribute>();
            if (attr != null)
            {
                name = attr.Name;
                description = attr.Description;
            }
            _parameters = new ParameterCollection(); // Initialize with a default collection
        }

        protected Item(Item original, Cloner cloner)
        {
            // Register this clone early to handle circular references
            cloner.RegisterClonedObject(original, this);
            
            name = original.name;
            description = original.description;
            // Parameters are not cloned by default in this base class, 
            // derived classes should handle cloning if necessary or if Parameters are part of their state.
            // Consider if a deep clone of parameters is needed here or if it's specific to certain items.
            // For now, let's assign a new collection or clone if original.Parameters is not null.
            _parameters = original.Parameters != null ? cloner.Clone(original.Parameters) : new ParameterCollection();
        }

        public abstract IDeepCloneable Clone(Cloner cloner);

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
