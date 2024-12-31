using System;
using System.Collections.Generic;
using System.Text;

namespace PropertyModels.ComponentModel.DataAnnotations
{
    /// <summary>
    /// 枚举类型允许值
    /// </summary>
    public class EnumPermitValuesAttribute:Attribute
    {

        public  int[] PermitValues { set; get; }

        public EnumPermitValuesAttribute()
        {
            
        }
        public EnumPermitValuesAttribute(int[] permitValues)
        {
            PermitValues = permitValues;
        }
    }
}
