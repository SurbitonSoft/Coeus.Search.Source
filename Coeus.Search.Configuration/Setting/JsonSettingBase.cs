// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonSettingBase.cs" company="Coeus Application Services">
//   Coeus Application Services 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Configuration.Setting
{
    using System;
    using System.IO;
    using System.Xml;

    using Newtonsoft.Json;

    using Formatting = Newtonsoft.Json.Formatting;

    /// <summary>
    /// The entity base.
    /// </summary>
    /// <typeparam name="T">
    /// Any class type to serialize/deserialize
    /// </typeparam>
    public static class JsonSettingBase<T>
    {
        #region Static Fields

        /// <summary>
        /// The json serializer settings.
        /// </summary>
        private static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, 
                NullValueHandling = NullValueHandling.Ignore
            };

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Deserializes workflow markup into an EntityBase object
        /// </summary>
        /// <param name="jsv">
        /// string workflow markup to deserialize
        /// </param>
        /// <param name="obj">
        /// Output EntityBase object
        /// </param>
        /// <param name="exception">
        /// output Exception value if deserialize failed
        /// </param>
        /// <returns>
        /// true if this XmlSerializer can deserialize the object; otherwise, false
        /// </returns>
        public static bool Deserialize(string jsv, out T obj, out Exception exception)
        {
            exception = null;
            obj = default(T);
            try
            {
                obj = Deserialize(jsv);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        /// <summary>
        /// The deserialize.
        /// </summary>
        /// <param name="jsv">
        /// The xml.
        /// </param>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool Deserialize(string jsv, out T obj)
        {
            Exception exception = null;
            return Deserialize(jsv, out obj, out exception);
        }

        /// <summary>
        /// The deserialize.
        /// </summary>
        /// <param name="xml">
        /// The xml.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T Deserialize(string xml)
        {
            return JsonConvert.DeserializeObject<T>(xml, jsonSerializerSettings);
        }

        /// <summary>
        /// Deserializes xml markup from file into an EntityBase object
        /// </summary>
        /// <param name="fileName">
        /// string xml file to load and deserialize
        /// </param>
        /// <param name="obj">
        /// Output EntityBase object
        /// </param>
        /// <param name="exception">
        /// output Exception value if deserialize failed
        /// </param>
        /// <returns>
        /// true if this XmlSerializer can deserialize the object; otherwise, false
        /// </returns>
        public static bool LoadFromFile(string fileName, out T obj, out Exception exception)
        {
            exception = null;
            obj = default(T);
            try
            {
                obj = LoadFromFile(fileName);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        /// <summary>
        /// The load from file.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool LoadFromFile(string fileName, out T obj)
        {
            Exception exception = null;
            return LoadFromFile(fileName, out obj, out exception);
        }

        /// <summary>
        /// The load from file.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T LoadFromFile(string fileName)
        {
            FileStream file = null;
            StreamReader sr = null;
            try
            {
                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                sr = new StreamReader(file);
                string xmlString = sr.ReadToEnd();
                sr.Close();
                file.Close();
                return Deserialize(xmlString);
            }
            finally
            {
                if (file != null)
                {
                    file.Dispose();
                }

                if (sr != null)
                {
                    sr.Dispose();
                }
            }
        }

        /// <summary>
        /// Serializes current EntityBase object into file
        /// </summary>
        /// <param name="fileName">
        /// full path of outupt xml file
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="exception">
        /// output Exception value if failed
        /// </param>
        /// <returns>
        /// true if can serialize and save into file; otherwise, false
        /// </returns>
        public static bool SaveToFile(string fileName, object item, out Exception exception)
        {
            exception = null;
            try
            {
                SaveToFile(fileName, item);
                return true;
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }

        /// <summary>
        /// The save to file.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        public static void SaveToFile(string fileName, object item)
        {
            StreamWriter streamWriter = null;
            try
            {
                string xmlString = JsonConvert.SerializeObject(item, Formatting.Indented, jsonSerializerSettings);
                var xmlFile = new FileInfo(fileName);
                streamWriter = xmlFile.CreateText();
                streamWriter.WriteLine(xmlString);
                streamWriter.Close();
            }
            finally
            {
                if (streamWriter != null)
                {
                    streamWriter.Dispose();
                }
            }
        }

        /// <summary>
        /// Serializes current EntityBase object into an XML document
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// string XML value
        /// </returns>
        public static string Serialize(object item)
        {
            return JsonConvert.SerializeObject(item, Formatting.Indented, jsonSerializerSettings);
        }

        /// <summary>
        /// The validate xsd from file.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool ValidateXsdFromFile(string fileName)
        {
            var r = new XmlTextReader(fileName);
            var xmlReaderSettings = new XmlReaderSettings { ValidationType = ValidationType.Schema };
           // xmlReaderSettings.Schemas.Add(SchemaCollection.Cache);

            XmlReader validationReader = XmlReader.Create(r, xmlReaderSettings);
            while (validationReader.Read())
            {
                validationReader.Close();
            }

            return true;
        }

        #endregion
    }
}