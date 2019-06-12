﻿// <auto-generated />

using System;
using System.Reflection;
using System.Resources;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Cosmos.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static class CosmosStrings
    {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("Microsoft.EntityFrameworkCore.Cosmos.Properties.CosmosStrings", typeof(CosmosStrings).GetTypeInfo().Assembly);

        /// <summary>
        ///     Cosmos-specific methods can only be used when the context is using the Cosmos provider.
        /// </summary>
        public static string CosmosNotInUse
            => GetString("CosmosNotInUse");

        /// <summary>
        ///     The entity of type '{entityType}' is mapped as a part of the document mapped to '{missingEntityType}', but there is no tracked entity of this type with the corresponding key value. Consider using 'DbContextOptionsBuilder.EnableSensitiveDataLogging' to see the key values.
        /// </summary>
        public static string OrphanedNestedDocument([CanBeNull] object entityType, [CanBeNull] object missingEntityType)
            => string.Format(
                GetString("OrphanedNestedDocument", nameof(entityType), nameof(missingEntityType)),
                entityType, missingEntityType);

        /// <summary>
        ///     The entity of type '{entityType}' is mapped as a part of the document mapped to '{missingEntityType}', but there is no tracked entity of this type with the key value '{keyValue}'.
        /// </summary>
        public static string OrphanedNestedDocumentSensitive([CanBeNull] object entityType, [CanBeNull] object missingEntityType, [CanBeNull] object keyValue)
            => string.Format(
                GetString("OrphanedNestedDocumentSensitive", nameof(entityType), nameof(missingEntityType), nameof(keyValue)),
                entityType, missingEntityType, keyValue);

        /// <summary>
        ///     No matching discriminator values where found for this instance of '{entityType}'.
        /// </summary>
        public static string UnableToDiscriminate([CanBeNull] object entityType)
            => string.Format(
                GetString("UnableToDiscriminate", nameof(entityType)),
                entityType);

        private static string GetString(string name, params string[] formatterNames)
        {
            var value = _resourceManager.GetString(name);
            for (var i = 0; i < formatterNames.Length; i++)
            {
                value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
            }

            return value;
        }
    }
}

