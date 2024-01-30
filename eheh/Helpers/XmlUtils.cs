using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace eheh.Helpers
{
    public static class XmlUtils
    {
        private static readonly Dictionary<Type, XmlSerializer> Serializers = new Dictionary<Type, XmlSerializer>();

        public static void Serialize<T>(string fileName, T item)
        {
            using (StreamWriter textWriter = new StreamWriter(fileName))
            {
                if (!Serializers.ContainsKey(typeof(T)))
                {
                    Serializers.Add(typeof(T), new XmlSerializer(typeof(T)));
                }
                Serializers[typeof(T)].Serialize(textWriter, item);
            }
        }

        public static void Serialize<T>(Stream stream, T item)
        {
            using (StreamWriter textWriter = new StreamWriter(stream))
            {
                if (!Serializers.ContainsKey(typeof(T)))
                {
                    Serializers.Add(typeof(T), new XmlSerializer(typeof(T)));
                }
                Serializers[typeof(T)].Serialize(textWriter, item);
            }
        }

        public static void Serialize(string fileName, object item, Type valueType)
        {
            using (StreamWriter textWriter = new StreamWriter(fileName))
            {
                if (!Serializers.ContainsKey(valueType))
                {
                    Serializers.Add(valueType, new XmlSerializer(valueType));
                }
                Serializers[valueType].Serialize(textWriter, item);
            }
        }

        public static void Serialize(Stream stream, object item, Type valueType)
        {
            using (StreamWriter textWriter = new StreamWriter(stream))
            {
                if (!Serializers.ContainsKey(valueType))
                {
                    Serializers.Add(valueType, new XmlSerializer(valueType));
                }
                Serializers[valueType].Serialize(textWriter, item);
            }
        }

        public static T Deserialize<T>(string fileName)
        {
            using (StreamReader textReader = new StreamReader(fileName))
            {
                if (!Serializers.ContainsKey(typeof(T)))
                {
                    Serializers.Add(typeof(T), new XmlSerializer(typeof(T)));
                }
                return (T)Serializers[typeof(T)].Deserialize(textReader);
            }
        }

        public static T Deserialize<T>(Stream stream)
        {
            using (StreamReader textReader = new StreamReader(stream))
            {
                if (!Serializers.ContainsKey(typeof(T)))
                {
                    Serializers.Add(typeof(T), new XmlSerializer(typeof(T)));
                }
                return (T)Serializers[typeof(T)].Deserialize(textReader);
            }
        }

        public static T Deserialize<T>(StringReader reader)
        {
            if (!Serializers.ContainsKey(typeof(T)))
            {
                Serializers.Add(typeof(T), new XmlSerializer(typeof(T)));
            }
            return (T)Serializers[typeof(T)].Deserialize(reader);
        }

        public static object Deserialize(string fileName, Type valueType)
        {
            using (StreamReader textReader = new StreamReader(fileName))
            {
                if (!Serializers.ContainsKey(valueType))
                {
                    Serializers.Add(valueType, new XmlSerializer(valueType));
                }
                return Serializers[valueType].Deserialize(textReader);
            }
        }

        public static object Deserialize(Stream stream, Type valueType)
        {
            using (StreamReader textReader = new StreamReader(stream))
            {
                if (!Serializers.ContainsKey(valueType))
                {
                    Serializers.Add(valueType, new XmlSerializer(valueType));
                }
                return Serializers[valueType].Deserialize(textReader);
            }
        }
    }
}
