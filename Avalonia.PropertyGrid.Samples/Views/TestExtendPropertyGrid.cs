using Avalonia.Controls;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Factories;
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
        }
    }

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

        public override Control HandleNewProperty(IPropertyGrid rootPropertyGrid, object target, PropertyDescriptor propertyDescriptor)
        {
            if(propertyDescriptor.PropertyType != typeof(SVector3))
            {
                return null;
            }

            Vector3View control = new Vector3View();

            return control;
        }

        public override bool HandlePropertyChanged(object target, PropertyDescriptor propertyDescriptor, Control control)
        {
            if(propertyDescriptor.PropertyType != typeof(SVector3))
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
                    SetAndRaise(vv, propertyDescriptor, target, model._vec);
                };

                return true;
            }

            return false;
        }
    }
}
