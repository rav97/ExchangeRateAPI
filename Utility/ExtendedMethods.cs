using ExchangeRateAPI.Models;
using ExchangeRateAPI.Models.ECB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ExchangeRateAPI.Utility
{
    public static class ExtendedMethods
    {
        public static object GetDefaultValue(this Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);

            return null;
        }

        public static void RewriteObject<T>(this T destination, T source) where T : class
        {
            var objectType = source.GetType();
            var propertiesList = objectType.GetProperties();

            foreach(var property in propertiesList)
            {
                if(!property.PropertyType.FullName.Contains("Database"))
                {
                    var value = property.GetValue(source, null);
                    property.SetValue(destination, value);
                }
            }
        }

        public static void UpdateObject<T>(this T destination, T source) where T : class
        {
            var objectType = source.GetType();
            var propertiesList = objectType.GetProperties();

            foreach (var property in propertiesList)
            {
                if (!property.PropertyType.FullName.Contains("Database"))
                {
                    var value = property.GetValue(source, null);
                    var defaultValue = property.PropertyType.GetDefaultValue();

                    if(!Equals(value, defaultValue))
                        property.SetValue(destination, value);
                }
            }
        }

        public static string XmlSerializeToString(this object objectInstance)
        {
            var serializer = new XmlSerializer(objectInstance.GetType());
            var sb = new StringBuilder();

            using (TextWriter writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, objectInstance);
            }

            return sb.ToString();
        }

        public static T XmlDeserializeFromString<T>(this string objectData)
        {
            return (T)XmlDeserializeFromString(objectData, typeof(T));
        }

        public static object XmlDeserializeFromString(this string objectData, Type type)
        {
            var serializer = new XmlSerializer(type);
            object result;

            using (TextReader reader = new StringReader(objectData))
            {
                result = serializer.Deserialize(reader);
            }

            return result;
        }

        public static string ToBase64(this string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }

        public static string FromBase64(this string encoded)
        {
            var bytes = Convert.FromBase64String(encoded);
            return Encoding.UTF8.GetString(bytes);
        }

    }
}
