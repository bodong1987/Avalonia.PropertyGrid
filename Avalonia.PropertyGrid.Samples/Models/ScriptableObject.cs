using PropertyModels.ComponentModel;
using PropertyModels.ComponentModel.DataAnnotations;
using PropertyModels.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Samples.Models
{
    /// <summary>
    /// Class ScriptableObject.
    /// </summary>
    public class ScriptableObject : ReactiveObject
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ScriptableObject"/> is require.
        /// </summary>
        /// <value><c>true</c> if require; otherwise, <c>false</c>.</value>
        public bool Require { get; set; }

        /// <summary>
        /// Gets or sets the type of the value.
        /// </summary>
        /// <value>The type of the value.</value>
        [DependsOnProperty(nameof(Value))]
        public Type ValueType { get; set; }

        object ValueCore;
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value
        {
            get => ValueCore;
            set
            {
                if (value == null)
                {
                    ValueCore = null;
                    ValueType = null;
                    RaisePropertyChanged(nameof(Value));
                    return;
                }

                if (value is IList list)
                {
                    if (list.Count != 0)
                    {
                        Type type = null;
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (list[i] != null)
                            {
                                type = list[i].GetType();
                                break;
                            }
                        }

                        if (type != null)
                        {
                            ValueCore = Activator.CreateInstance(typeof(BindingList<>).MakeGenericType(type));

                            for (int i = 0; i < list.Count; ++i)
                            {
                                (ValueCore as IBindingList).Add(list[i]);
                            }

                            ValueType = ValueCore.GetType();

                            RaisePropertyChanged(nameof(Value));

                            return;
                        }
                    }

                    ValueCore = new BindingList<string>();
                    ValueType = ValueCore.GetType();
                }
                else
                {
                    ValueCore = value;
                    ValueType = ValueCore.GetType();
                }


                RaisePropertyChanged(nameof(Value));
            }
        }

        public Attribute[] ExtraAttributes
        {
            get;
            set;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return (DisplayName.IsNotNullOrEmpty() ? DisplayName : Name) + "=" + (Value?.ToString() ?? "None");
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <returns>Attribute[].</returns>
        public Attribute[] GetAttributes()
        {
            DisplayNameAttribute displayNameAttribute = new DisplayNameAttribute(DisplayName.IsNotNullOrEmpty() ? DisplayName : Name);
            DescriptionAttribute descAttribute = new DescriptionAttribute(Description.IsNotNullOrEmpty() ? Description : displayNameAttribute.DisplayName);

            if(ExtraAttributes != null)
            {
                return ExtraAttributes.Concat(new Attribute[] { descAttribute, displayNameAttribute }).ToArray();
            }
            else
            {
                return new Attribute[] { descAttribute, displayNameAttribute };
            }
        }
    }
}
