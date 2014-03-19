// --------------------------------------------------------------------------------------------------------------------
// <copyright file="YamlFormatter.cs" company="Coeus Consulting Ltd.">
//   Coeus Consulting 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Yaml
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;

    /// <summary>
    /// The yaml formatter.
    /// </summary>
    public class YamlFormatter : IFormatter
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="T:System.Runtime.Serialization.SerializationBinder"/> that performs type lookups during deserialization.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Runtime.Serialization.SerializationBinder"/> that performs type lookups during deserialization.
        /// </returns>
        public SerializationBinder Binder { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="T:System.Runtime.Serialization.StreamingContext"/> used for serialization and deserialization.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Runtime.Serialization.StreamingContext"/> used for serialization and deserialization.
        /// </returns>
        public StreamingContext Context { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="T:System.Runtime.Serialization.SurrogateSelector"/> used by the current formatter.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Runtime.Serialization.SurrogateSelector"/> used by this formatter.
        /// </returns>
        public ISurrogateSelector SurrogateSelector { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Deserializes the data on the provided stream and reconstitutes the graph of objects.
        /// </summary>
        /// <returns>
        /// The top object of the deserialized graph.
        /// </returns>
        /// <param name="serializationStream">
        /// The stream that contains the data to deserialize. 
        /// </param>
        public object Deserialize(Stream serializationStream)
        {
            var sr = new StreamReader(serializationStream);

            // Get Type from serialized data.
            string line = sr.ReadLine();
            var delim = new[] { ':' };
            string[] sarr = line.Split(delim);
            string className = sarr[1];
            Type t = Type.GetType(className);

            // Create object of just found type name.
            object obj = FormatterServices.GetUninitializedObject(t);

            // Get type members.
            MemberInfo[] members = FormatterServices.GetSerializableMembers(obj.GetType(), this.Context);

            // Create data array for each member.
            var data = new object[members.Length];

            // Store serialized variable name -> value pairs.
            var sdict = new StringDictionary();
            while (sr.Peek() >= 0)
            {
                line = sr.ReadLine();
                sarr = line.Split(delim);

                // key = variable name, value = variable value.
                sdict[sarr[0].Trim()] = sarr[1].Trim();
            }

            sr.Close();

            // Store for each member its value, converted from string to its type.
            for (int i = 0; i < members.Length; ++i)
            {
                var fi = (FieldInfo)members[i];
                if (!sdict.ContainsKey(fi.Name))
                {
                    throw new SerializationException("Missing field value : " + fi.Name);
                }

                data[i] = Convert.ChangeType(sdict[fi.Name], fi.FieldType);
            }

            // Populate object members with theri values and return object.
            return FormatterServices.PopulateObjectMembers(obj, members, data);
        }

        /// <summary>
        /// Serializes an object, or graph of objects with the given root to the provided stream.
        /// </summary>
        /// <param name="serializationStream">
        /// The stream where the formatter puts the serialized data. This stream can reference a variety of backing stores (such as files, network, memory, and so on). 
        /// </param>
        /// <param name="graph">
        /// The object, or root of the object graph, to serialize. All child objects of this root object are automatically serialized. 
        /// </param>
        public void Serialize(Stream serializationStream, object graph)
        {
            // Get fields that are to be serialized.
            MemberInfo[] members = FormatterServices.GetSerializableMembers(graph.GetType(), this.Context);

            // Get fields data.
            object[] objs = FormatterServices.GetObjectData(graph, members);

            // Write class name and all fields & values to file
            var sw = new StreamWriter(serializationStream);

            // sw.WriteLine("@ClassName={0}", graph.GetType().FullName);
            for (int i = 0; i < objs.Length; ++i)
            {
                // Skip this field if it is marked NonSerialized.
                if (Attribute.IsDefined(members[i], typeof(NonSerializedAttribute)))
                {
                    continue;
                }

                var field = (FieldInfo)members[i];
                if (field.FieldType.IsPrimitive || field.FieldType == typeof(string))
                {
                    sw.WriteLine("{0}:{1}", members[i].Name, objs[i]);
                }
                else if (field.FieldType.IsEnum)
                {
                    sw.WriteLine("{0}:{1}", members[i].Name, objs[i]);

                }
                else if (field.FieldType.IsGenericType)
                {
                    foreach (Type interfaceType in field.FieldType.GetInterfaces())
                    {
                        if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IList<>))
                        {
                            sw.WriteLine("{0}:", members[i].Name);

                            foreach (var genericArgument in field.FieldType.GetGenericArguments())
                            {
                                if (genericArgument.IsPrimitive || genericArgument == typeof(string))
                                {
                                    sw.WriteLine("-{0}:{1}", genericArgument.Name, genericArgument.Name);
                                }
                                else if (genericArgument.IsEnum)
                                {
                                    sw.WriteLine("-{0}:{1}", members[i].Name, objs[i]);

                                }
                            }

                            break;
                        }
                    }
                }
            }

            sw.Close();
        }

        #endregion
    }
}