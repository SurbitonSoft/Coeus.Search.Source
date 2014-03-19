// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingBase.cs" company="Coeus Consulting Ltd.">
//   Coeus Consulting Ltd. 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Configuration
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// The entity base.
    /// </summary>
    /// <typeparam name="T">
    /// Any class type to serialize/deserialize
    /// </typeparam>
    public class SettingBase<T>
    {
        #region Static Fields

        /// <summary>
        /// The serializer.
        /// </summary>
        private static XmlSerializer serializer;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the serializer.
        /// </summary>
        private static XmlSerializer Serializer
        {
            get
            {
                //XmlRootAttribute xRoot = new XmlRootAttribute();
                //xRoot.ElementName = "Analyzer";
                //xRoot.Namespace = "http://SearcherSchemas.com/analyzers";
                //xRoot.IsNullable = false;
                return serializer ?? (serializer = new XmlSerializer(typeof(T)));
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Deserializes workflow markup into an EntityBase object
        /// </summary>
        /// <param name="xml">
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
        public static bool Deserialize(string xml, out T obj, out Exception exception)
        {
            exception = null;
            obj = default(T);
            try
            {
                obj = Deserialize(xml);
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
        /// <param name="xml">
        /// The xml.
        /// </param>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool Deserialize(string xml, out T obj)
        {
            Exception exception = null;
            return Deserialize(xml, out obj, out exception);
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
            StringReader stringReader = null;
            try
            {
                stringReader = new StringReader(xml);
                return (T)Serializer.Deserialize(XmlReader.Create(stringReader));
            }
            finally
            {
                if (stringReader != null)
                {
                    stringReader.Dispose();
                }
            }
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
        /// The validate xsd from file.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="schemaName">
        /// The schema name.
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

        /// <summary>
        /// Serializes current EntityBase object into file
        /// </summary>
        /// <param name="fileName">
        /// full path of outupt xml file
        /// </param>
        /// <param name="exception">
        /// output Exception value if failed
        /// </param>
        /// <returns>
        /// true if can serialize and save into file; otherwise, false
        /// </returns>
        public virtual bool SaveToFile(string fileName, out Exception exception)
        {
            exception = null;
            try
            {
                this.SaveToFile(fileName);
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
        public virtual void SaveToFile(string fileName)
        {
            StreamWriter streamWriter = null;
            try
            {
                string xmlString = this.Serialize();
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
        /// <returns>string XML value</returns>
        public virtual string Serialize()
        {
            StreamReader streamReader = null;
            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream();
                Serializer.Serialize(memoryStream, this);
                memoryStream.Seek(0, SeekOrigin.Begin);
                streamReader = new StreamReader(memoryStream);
                return streamReader.ReadToEnd();
            }
            finally
            {
                if (streamReader != null)
                {
                    streamReader.Dispose();
                }

                if (memoryStream != null)
                {
                    memoryStream.Dispose();
                }
            }
        }

        #endregion
    }
}