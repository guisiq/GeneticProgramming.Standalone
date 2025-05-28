using System;
using System.ComponentModel;
using System.Reflection;
using GeneticProgramming.Core;

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

        protected Item()
        {
            var attr = GetType().GetCustomAttribute<ItemAttribute>();
            if (attr != null)
            {
                name = attr.Name;
                description = attr.Description;
            }
        }

        protected Item(Item original, Cloner cloner)
        {
            name = original.name;
            description = original.description;
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
