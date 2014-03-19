// --------------------------------------------------------------------------------------------------------------------
// <copyright file="YamlObject.cs" company="">
//   
// </copyright>
// <summary>
//   The yaml object.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Coeus.Yaml
{
    using System.Collections.Generic;

    /// <summary>
    /// The yaml object.
    /// </summary>
    public class YamlObject
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether is object.
        /// </summary>
        public YamlParser.YamlObjectType ObjectType { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        public List<YamlObject> SubObjects { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value { get; set; }

        #endregion
    }
}