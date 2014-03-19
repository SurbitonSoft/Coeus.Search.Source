// --------------------------------------------------------------------------------------------------------------------
// <copyright file="YamlParser.cs" company="Coeus Consulting Ltd.">
//   Coeus Consulting 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Yaml
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// The yaml parser.
    /// </summary>
    public class YamlParser
    {
        #region Enums

        /// <summary>
        /// The yaml object type.
        /// </summary>
        public enum YamlObjectType
        {
            /// <summary>
            /// The object is an array
            /// </summary>
            Array, 

            /// <summary>
            /// The object is simple key value pair.
            /// </summary>
            Simple, 

            /// <summary>
            /// The object is a comment.
            /// </summary>
            Comment
        }

        /// <summary>
        /// The line type.
        /// </summary>
        private enum LineType
        {
            /// <summary>
            /// The object start.
            /// </summary>
            ObjectStart, 

            /// <summary>
            /// The object line item.
            /// </summary>
            ObjectLineItem, 

            /// <summary>
            /// The line item.
            /// </summary>
            LineItem
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The read yaml.
        /// </summary>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <returns>
        /// The System.Collections.Generic.List`1[T -&gt; Coeus.Yaml.YamlObject].
        /// </returns>
        public static List<YamlObject> ReadYaml(string filePath)
        {
            var yamlObjects = new List<YamlObject>();
            if (File.Exists(filePath))
            {
                using (var sr = new StreamReader(filePath))
                {
                    var yamlObject = new YamlObject();
                    int i = 0;
                    while (sr.Peek() >= 0 && !sr.EndOfStream)
                    {
                        i++;
                        string line = sr.ReadLine();

                        // Ignore blank lines
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }

                        // This is a comment
                        if (line.Trim().StartsWith("#", StringComparison.OrdinalIgnoreCase))
                        {
                            yamlObjects.Add(new YamlObject { ObjectType = YamlObjectType.Comment, Value = line });
                            continue;
                        }

                        // If the line ends with : then it is surely an object but does not start with a -
                        if (line.Trim().EndsWith(":", StringComparison.OrdinalIgnoreCase)
                            && !line.Trim().StartsWith("-", StringComparison.OrdinalIgnoreCase))
                        {
                            KeyValuePair<string, string> keyValue = ParseYamlLine(line.Trim(), LineType.ObjectStart, i);
                            yamlObject = new YamlObject
                                {
                                    ObjectType = YamlObjectType.Array, 
                                    Key = keyValue.Key, 
                                    SubObjects = new List<YamlObject>()
                                };

                            yamlObjects.Add(yamlObject);
                        }
                        else if (line.Trim().StartsWith("-", StringComparison.OrdinalIgnoreCase))
                        {
                            // If the line starts with a - then it is a list item
                            if (yamlObject.ObjectType == YamlObjectType.Array)
                            {
                                KeyValuePair<string, string> keyValue = ParseYamlLine(
                                    line.Trim(), LineType.ObjectLineItem, i);
                                yamlObject.SubObjects.Add(
                                    new YamlObject
                                        {
                                            Key = keyValue.Key, 
                                            Value = keyValue.Value, 
                                            ObjectType = YamlObjectType.Simple
                                        });
                            }
                            else
                            {
                                throw new MalformedInputException(
                                    "No top level item defined for the list item.", i, line);
                            }
                        }
                        else
                        {
                            // This is a normal key value pair
                            KeyValuePair<string, string> keyValue = ParseYamlLine(line.Trim(), LineType.LineItem, i);
                            yamlObjects.Add(
                                new YamlObject
                                    {
                                       ObjectType = YamlObjectType.Simple, Key = keyValue.Key, Value = keyValue.Value 
                                    });
                        }
                    }
                }
            }
            else
            {
                throw new FileNotFoundException(filePath);
            }

            return yamlObjects;
        }

        /// <summary>
        /// The write yaml.
        /// </summary>
        /// <param name="yamlObjects">
        /// The yaml objects.
        /// </param>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <returns>
        /// The System.Boolean.
        /// </returns>
        public static bool WriteYaml(List<YamlObject> yamlObjects, string filePath)
        {
            using (var sr = new StreamWriter(filePath, false))
            {
                foreach (YamlObject yamlObject in yamlObjects)
                {
                    switch (yamlObject.ObjectType)
                    {
                        case YamlObjectType.Array:
                            sr.WriteLine(string.Concat(yamlObject.Key, ":"));
                            foreach (YamlObject property in yamlObject.SubObjects)
                            {
                                sr.WriteLine(string.Concat("\t", property.Key, ":", property.Value));
                            }

                            break;
                        case YamlObjectType.Simple:
                            sr.WriteLine(string.Concat(yamlObject.Key, ":", yamlObject.Value));
                            break;
                        case YamlObjectType.Comment:
                            sr.WriteLine(yamlObject.Value);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            return true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The parse yaml line.
        /// </summary>
        /// <param name="line">
        /// The line.
        /// </param>
        /// <param name="lineType">
        /// The line type.
        /// </param>
        /// <param name="lineNumber">
        /// The line Number.
        /// </param>
        /// <returns>
        /// The System.Collections.Generic.KeyValuePair`2[TKey -&gt; System.String, TValue -&gt; System.String].
        /// </returns>
        private static KeyValuePair<string, string> ParseYamlLine(string line, LineType lineType, int lineNumber)
        {
            string key, value;

            switch (lineType)
            {
                case LineType.ObjectStart:
                    key = line.Remove(line.LastIndexOf(":", StringComparison.Ordinal));
                    value = string.Empty;
                    break;
                case LineType.ObjectLineItem:

                    // Remove leading -
                    string temp = line.Replace("-", string.Empty);

                    if (temp.IndexOf(":", StringComparison.Ordinal) == -1)
                    {
                        throw new MalformedInputException(
                            "The line data is not in correct format. Expected ':'", lineNumber, line);
                    }

                    key = temp.Remove(temp.IndexOf(":", StringComparison.Ordinal));
                    value = temp.Substring(temp.IndexOf(":", StringComparison.Ordinal) + 1);
                    break;
                case LineType.LineItem:

                    if (line.IndexOf(":", StringComparison.Ordinal) == -1)
                    {
                        throw new MalformedInputException(
                            "The line data is not in correct format. Expected ':'", lineNumber, line);
                    }

                    key = line.Remove(line.IndexOf(":", StringComparison.Ordinal));
                    value = line.Substring(line.IndexOf(":", StringComparison.Ordinal) + 1);

                    break;
                default:
                    throw new ArgumentOutOfRangeException("lineType");
            }

            return new KeyValuePair<string, string>(key, value);
        }

        #endregion
    }
}