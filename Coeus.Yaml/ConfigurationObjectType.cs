// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationObjectType.cs" company="Coeus Consulting Ltd.">
//   Coeus Consulting 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Yaml
{
    /// <summary>
    /// The configuration object type.
    /// </summary>
    public enum ConfigurationObjectType
    {
        /// <summary>
        /// The integer.
        /// </summary>
        Integer, 

        /// <summary>
        /// The string.
        /// </summary>
        String, 

        /// <summary>
        /// The float.
        /// </summary>
        Float, 

        /// <summary>
        /// The enum.
        /// </summary>
        Enum, 

        /// <summary>
        /// The boolean type
        /// </summary>
        Bool, 

        /// <summary>
        /// The complex.
        /// </summary>
        Complex
    }
}