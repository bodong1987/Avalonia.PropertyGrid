using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Factories;
using Avalonia.PropertyGrid.Controls.Factories.Builtins;
using Avalonia.PropertyGrid.Model.Collections;
using Avalonia.PropertyGrid.Model.ComponentModel;
using Avalonia.PropertyGrid.Samples.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Samples.Views
{
    public class TestExtendPropertyGrid : Controls.PropertyGrid
    {
        static TestExtendPropertyGrid()
        {
            FactoryTemplates.AddFactory(new Vector3CellEditFactory());
            FactoryTemplates.AddFactory(new CountryInfoCellEditFactory());
        }
    }

    #region SVector3
    public class SVector3ViewModel : MiniReactiveObject
    {
        public SVector3 _vec;

        public float X
        {
            get
            {
                return _vec.x;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _vec.x, value);
            }
        }

        public float Y
        {
            get
            {
                return _vec.y;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _vec.y, value);
            }
        }

        public float Z
        {
            get
            {
                return _vec.z;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _vec.z, value);
            }
        }
    }

    class Vector3CellEditFactory : AbstractCellEditFactory
    {
        public override bool Accept(object accessToken)
        {
            return accessToken is TestExtendPropertyGrid;
        }

        public override Control HandleNewProperty(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;

            if (propertyDescriptor.PropertyType != typeof(SVector3))
            {
                return null;
            }

            Vector3View control = new Vector3View();

            return control;
        }

        public override bool HandlePropertyChanged(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;
            var control = context.CellEdit;

            if (propertyDescriptor.PropertyType != typeof(SVector3))
            {
                return false;
            }

            ValidateProperty(control, propertyDescriptor, target);

            if (control is Vector3View vv)
            {
                SVector3 vec = (SVector3)propertyDescriptor.GetValue(target);

                var model = new SVector3ViewModel() { _vec = vec };
                vv.DataContext = model;

                model.PropertyChanged += (s, e) =>
                {
                    SetAndRaise(context, context.CellEdit, model._vec);
                };

                return true;
            }

            return false;
        }
    }
    #endregion

    #region Countries
    internal class CountryInfoCellEditFactory : SelectableListCellEditFactory
    {
        public override int ImportPriority => base.ImportPriority + 100;

        public override bool Accept(object accessToken)
        {
            return accessToken is TestExtendPropertyGrid;
        }

        public override Control HandleNewProperty(PropertyCellContext context)
        {
            if(context.Property.PropertyType != typeof(SelectableList<CountryInfo>))
            {
                return null;
            }

            Control control = base.HandleNewProperty(context);

            if(control is ComboBox cb)
            {
                cb.ItemTemplate = new FuncDataTemplate<CountryInfo>((value, namescope) =>
                {
                    return new CountryView();
                });
            }

            return control;
        }
    }
    #endregion
}
