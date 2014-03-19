namespace Coeus.Search.Sdk.Helpers
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public class XsdHelpers<T>
    {
        /// <summary>
        /// Deserialize the xml to entity
        /// </summary>
        /// <param name="filename">Path to the file to read from</param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void DeserializeObjectFromFile(string filename, out T entity)
        {
            // Create an instance of the XmlSerializer specifying type and namespace.
            var serializer = new XmlSerializer(typeof(T));

            try
            {
                using (TextReader reader = new StreamReader(filename))
                {
                    entity = (T)serializer.Deserialize(reader);
                    reader.Close();
                    return;
                }

                /*
                // A FileStream is needed to read the XML document.
                using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {

                    using (var reader = XmlReader.Create(fs))
                    {
                        entity = (T)serializer.Deserialize(reader);
                        fs.Close();
                        reader.Close();
                        return;
                    }
                }
                 */
            }
            catch (FileNotFoundException ex)
            {
                //Logger.Fatal("Configuration file not found at the configured location.", ex);
                throw;
            }
            catch (Exception ex)
            {
                //Logger.Fatal("Error processing configuration file.", ex);
            }
            entity = default(T);
        }


        /// <summary>
        /// Serialize the entity object to xml
        /// </summary>
        /// <param name="filename">Path to the file</param>
        /// <param name="entity"></param>
        public bool SerializeObjectToFile(string filename, T entity)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter writer = new StreamWriter(fs, new UTF8Encoding()))
                {
                    serializer.Serialize(writer, entity);
                    fs.Close();
                    writer.Close();
                    return true;
                }
            }
        }

        /// <summary>
        /// Deserialize the xml to entity
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void DeserializeObjectFromString(string xml, out T entity)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var stringReader = new StringReader(xml))
            {
                try
                {
                    entity = ((T)(serializer.Deserialize(XmlReader.Create(stringReader))));
                    return;
                }
                catch (Exception ex) { }
            }
            entity = default(T);
        }


        /// <summary>
        /// Serializes current EntityBase object into an XML document
        /// </summary>
        // <returns>string XML value</returns>
        public string Serialize(T entity)
        {
            var serializer = new XmlSerializer(typeof(T));
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    serializer.Serialize(memoryStream, entity);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    using (var streamReader = new StreamReader(memoryStream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }
    }
}
