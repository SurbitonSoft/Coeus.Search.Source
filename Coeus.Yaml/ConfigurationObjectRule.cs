// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationObjectRule.cs" company="Coeus Consulting Ltd.">
//   Coeus Consulting 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Yaml
{
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// The configuration object rule.
    /// </summary>
    public class ConfigurationObjectRule
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationObjectRule"/> class.
        /// </summary>
        /// <param name="objectName">
        /// The object name.
        /// </param>
        /// <param name="objectType">
        /// The object type.
        /// </param>
        /// <param name="objectDefaultValue">
        /// The object default value.
        /// </param>
        /// <param name="objectEnumValues">
        /// The object enum values.
        /// </param>
        /// <param name="childObjects">
        /// The child objects.
        /// </param>
        /// <param name="userValueRequired">
        /// The user Value Required.
        /// </param>
        public ConfigurationObjectRule(
            string objectName, 
            ConfigurationObjectType objectType, 
            string objectDefaultValue, 
            string[] objectEnumValues, 
            List<ConfigurationObjectRule> childObjects, 
            bool userValueRequired)
        {
            this.UserValueRequired = userValueRequired;
            this.ObjectEnumValues = objectEnumValues;
            this.ObjectDefaultValue = objectDefaultValue;
            this.ObjectType = objectType;
            this.ObjectName = objectName;
            this.ChildObjects = childObjects;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationObjectRule"/> class.
        /// </summary>
        /// <param name="objectName">
        /// The object Name.
        /// </param>
        /// <param name="objectType">
        /// The object Type.
        /// </param>
        /// <param name="objectDefaultValue">
        /// The object Default Value.
        /// </param>
        /// <param name="objectEnumValues">
        /// The object Enum Values.
        /// </param>
        /// <param name="userValueRequired">
        /// The user Value Required.
        /// </param>
        public ConfigurationObjectRule(
            string objectName, 
            ConfigurationObjectType objectType, 
            string objectDefaultValue, 
            string[] objectEnumValues, 
            bool userValueRequired)
        {
            if (objectType != ConfigurationObjectType.Complex)
            {
                throw new InvalidEnumArgumentException("objectType", (int)objectType, typeof(ConfigurationObjectType));
            }

            this.UserValueRequired = userValueRequired;
            this.ObjectEnumValues = objectEnumValues;
            this.ObjectDefaultValue = objectDefaultValue;
            this.ObjectType = objectType;
            this.ObjectName = objectName;
            this.ChildObjects = new List<ConfigurationObjectRule>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the child objects.
        /// </summary>
        public List<ConfigurationObjectRule> ChildObjects { get; private set; }

        /// <summary>
        /// Gets the object default value.
        /// </summary>
        public string ObjectDefaultValue { get; private set; }

        /// <summary>
        /// Gets the object enum values.
        /// </summary>
        public string[] ObjectEnumValues { get; private set; }

        /// <summary>
        /// Gets the object name.
        /// </summary>
        public string ObjectName { get; private set; }

        /// <summary>
        /// Gets or sets the object type.
        /// </summary>
        public ConfigurationObjectType ObjectType { get; set; }

        /// <summary>
        /// Gets a value indicating whether user value required.
        /// </summary>
        public bool UserValueRequired { get; private set; }

        #endregion
    }
}