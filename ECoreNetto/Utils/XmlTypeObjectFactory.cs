// ------------------------------------------------------------------------------------------------
// <copyright file="XmlTypeObjectFactory.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Utils
{
    using System.Collections.Generic;

    using ECoreNetto.Resource;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Instantiates the built-in <c>XMLType</c> data types (the Ecore mapping of the XML Schema simple
    /// types) so that references into the <c>http://www.eclipse.org/emf/2003/XMLType</c> namespace resolve
    /// without a backing file, mirroring how EMF's <c>EPackage.Registry</c> makes them resolvable.
    /// </summary>
    internal static class XmlTypeObjectFactory
    {
        /// <summary>
        /// The namespace URI under which the <c>XMLType</c> data types are referenced.
        /// </summary>
        public const string NamespaceUri = "http://www.eclipse.org/emf/2003/XMLType";

        /// <summary>
        /// The names of the <c>XMLType</c> data types, taken verbatim from EMF's
        /// <c>org.eclipse.emf.ecore.xml.type.impl.XMLTypePackageImpl</c>.
        /// </summary>
        private static readonly string[] DataTypeNames =
        {
            "AnySimpleType", "AnyURI", "Base64Binary", "Boolean", "BooleanObject", "Byte", "ByteObject",
            "Date", "DateTime", "Decimal", "Double", "DoubleObject", "Duration", "ENTITIES", "ENTITIESBase",
            "ENTITY", "Float", "FloatObject", "GDay", "GMonth", "GMonthDay", "GYear", "GYearMonth",
            "HexBinary", "ID", "IDREF", "IDREFS", "IDREFSBase", "Int", "Integer", "IntObject", "Language",
            "Long", "LongObject", "Name", "NCName", "NegativeInteger", "NMTOKEN", "NMTOKENS", "NMTOKENSBase",
            "NonNegativeInteger", "NonPositiveInteger", "NormalizedString", "NOTATION", "PositiveInteger",
            "QName", "Short", "ShortObject", "String", "Time", "Token", "UnsignedByte", "UnsignedByteObject",
            "UnsignedInt", "UnsignedIntObject", "UnsignedLong", "UnsignedShort", "UnsignedShortObject"
        };

        /// <summary>
        /// Creates the <c>XMLType</c> data types, each keyed by its fully-qualified reference
        /// (<c>http://www.eclipse.org/emf/2003/XMLType#//&lt;Name&gt;</c>).
        /// </summary>
        /// <param name="resource">
        /// The <see cref="Resource"/> that owns the created data types.
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging.
        /// </param>
        /// <returns>
        /// The keyed <c>XMLType</c> data types.
        /// </returns>
        public static IEnumerable<KeyValuePair<string, EObject>> CreateDataTypes(Resource resource, ILoggerFactory? loggerFactory)
        {
            foreach (var name in DataTypeNames)
            {
                yield return new KeyValuePair<string, EObject>(
                    $"{NamespaceUri}#//{name}",
                    new EDataType(resource, loggerFactory) { Name = name });
            }
        }
    }
}
