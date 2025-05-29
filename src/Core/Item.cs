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
        public Guid Id { get; } // Add this line

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
            Id = Guid.NewGuid(); // Add this line to initialize Id
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
            
            Id = original.Id; // Add this line to copy Id during cloning
            name = original.name;
            description = original.description;
            _parameters = original.Parameters != null ? cloner.Clone(original.Parameters) : new ParameterCollection();
        }

        // Tornando Clone virtual e adicionando a lógica padrão.
        // As classes derivadas não precisarão mais verificar ClonedObjectRegistered diretamente em Clone.
        public virtual IDeepCloneable Clone(Cloner cloner)
        {
            if (cloner.ClonedObjectRegistered(this))
            {
                return cloner.GetClone(this)!;
            }
            // A instância criada por CreateCloneInstance será registrada
            // automaticamente quando seu construtor chamar o construtor base Item(original, cloner).
            return CreateCloneInstance(cloner);
        }

        /// <summary>
        /// Creates a new instance of the derived type for cloning.
        /// This method is called by the base Clone method after ensuring the object
        /// hasn't already been cloned by the provided Cloner instance.
        /// The constructor of the derived class, typically a copy constructor,
        /// is responsible for calling the base Item(original, cloner) constructor,
        /// which will handle the registration of the new clone.
        /// </summary>
        /// <param name="cloner">The cloner to use for the cloning process.</param>
        /// <returns>A new instance of the derived type.</returns>
        protected abstract IDeepCloneable CreateCloneInstance(Cloner cloner);

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
