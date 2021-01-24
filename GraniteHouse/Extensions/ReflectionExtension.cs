using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraniteHouse.Extensions
{
    public static class ReflectionExtension
    {
        public static String GetPropertyValue<T> (this T item ,String propertyName) 
        {
            return item.GetType().GetProperty(propertyName).GetValue(item, null).ToString();
        
        }


    }
}
