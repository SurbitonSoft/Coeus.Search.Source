// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuleValidator.cs" company="Coeus Consulting Ltd.">
//   Coeus Consulting 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Yaml
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// The rule validator.
    /// </summary>
    public class RuleValidator
    {
        #region Public Methods and Operators

        /// <summary>
        /// The validate rule.
        /// </summary>
        /// <param name="rules">
        /// The rules.
        /// </param>
        /// <param name="configurationItems">
        /// The configuration Items.
        /// </param>
        /// <returns>
        /// The System.Boolean.
        /// </returns>
        public static bool ValidateRule(List<ConfigurationObjectRule> rules, ref List<YamlObject> configurationItems)
        {
            foreach (ConfigurationObjectRule rule in rules.Where(a => a.ObjectType != ConfigurationObjectType.Complex))
            {
                YamlObject item =
                    configurationItems.FirstOrDefault(
                        a => string.Equals(a.Key, rule.ObjectName, StringComparison.OrdinalIgnoreCase));
                if (item != null)
                {
                    if (string.IsNullOrWhiteSpace(item.Value) && item.ObjectType != YamlParser.YamlObjectType.Array)
                    {
                        Debug.Fail("The passed in input values should never be null for a non array item.");
                    }

                    DataTypeValidation(rule, item.Value);
                }
                else if (rule.ObjectType == ConfigurationObjectType.Complex && rule.UserValueRequired)
                {
                    throw new ConfigurationParameterMissingException(
                        "Missing required configuration setting: " + rule.ObjectName);
                }
                else if (!rule.UserValueRequired && !string.IsNullOrWhiteSpace(rule.ObjectDefaultValue)
                         && rule.ObjectType != ConfigurationObjectType.Complex)
                {
                    configurationItems.Add(
                        new YamlObject
                            {
                                Key = rule.ObjectName, 
                                Value = rule.ObjectDefaultValue, 
                                ObjectType = YamlParser.YamlObjectType.Simple
                            });
                }
                else
                {
                    throw new ConfigurationParameterMissingException(
                        "Missing required configuration setting: " + rule.ObjectName);
                }
            }

            // Complex type validation
            foreach (ConfigurationObjectRule rule in
                rules.Where(a => a.ObjectType == ConfigurationObjectType.Complex && a.UserValueRequired))
            {
                IEnumerable<YamlObject> arrays =
                    configurationItems.Where(a => a.ObjectType == YamlParser.YamlObjectType.Array);
                List<YamlObject> yamlObjects = arrays as List<YamlObject> ?? arrays.ToList();

                if (!yamlObjects.Any())
                {
                    throw new ConfigurationParameterMissingException(
                        "Missing required configuration setting: " + rule.ObjectName);
                }

                foreach (YamlObject array in yamlObjects)
                {
                    ComplexRuleValidation(rule, array);
                }
            }

            return true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The complex rule validation.
        /// </summary>
        /// <param name="rule">
        /// The rule.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        private static void ComplexRuleValidation(ConfigurationObjectRule rule, YamlObject item)
        {
            foreach (ConfigurationObjectRule configurationObjectRule in rule.ChildObjects)
            {
                YamlObject property =
                    item.SubObjects.FirstOrDefault(
                        a =>
                        string.Equals(a.Key, configurationObjectRule.ObjectName, StringComparison.OrdinalIgnoreCase));
                if (property != null)
                {
                    DataTypeValidation(configurationObjectRule, property.Value);
                    continue;
                }

                if (configurationObjectRule.ObjectType == ConfigurationObjectType.Complex
                    && configurationObjectRule.UserValueRequired)
                {
                    throw new ConfigurationParameterMissingException(
                        "Missing required configuration setting: " + configurationObjectRule.ObjectName);
                }

                if (!configurationObjectRule.UserValueRequired
                    && !string.IsNullOrWhiteSpace(configurationObjectRule.ObjectDefaultValue)
                    && configurationObjectRule.ObjectType != ConfigurationObjectType.Complex)
                {
                    item.SubObjects.Add(
                        new YamlObject
                            {
                                Key = configurationObjectRule.ObjectName, 
                                Value = configurationObjectRule.ObjectDefaultValue, 
                                ObjectType = YamlParser.YamlObjectType.Simple
                            });
                }
                else
                {
                    throw new ConfigurationParameterMissingException(
                        "Missing required configuration setting: " + configurationObjectRule.ObjectName);
                }
            }
        }

        /// <summary>
        /// The data type validation.
        /// </summary>
        /// <param name="rule">
        /// The rule.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        private static void DataTypeValidation(ConfigurationObjectRule rule, string value)
        {
            switch (rule.ObjectType)
            {
                case ConfigurationObjectType.Integer:
                    int i;
                    if (!int.TryParse(value, out i))
                    {
                        throw new InvalidCastException("The input value of " + value + "is not a valid integer.");
                    }

                    break;
                case ConfigurationObjectType.String:
                    break;
                case ConfigurationObjectType.Float:
                    decimal decimalInput;
                    if (!decimal.TryParse(value, out decimalInput))
                    {
                        throw new InvalidCastException("The input value of " + value + "is not a valid decimal.");
                    }

                    break;
                case ConfigurationObjectType.Enum:
                    if (!rule.ObjectEnumValues.Contains(value, StringComparer.OrdinalIgnoreCase))
                    {
                        throw new InvalidCastException(
                            "The input value of " + value + "is not a in the valid enum list.");
                    }

                    break;
                case ConfigurationObjectType.Bool:
                    bool boolInput;
                    if (!bool.TryParse(value, out boolInput))
                    {
                        throw new InvalidCastException("The input value of " + value + "is not a valid boolean.");
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}