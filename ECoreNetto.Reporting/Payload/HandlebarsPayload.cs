// ------------------------------------------------------------------------------------------------
// <copyright file="HandlebarsPayload.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Reporting.Payload
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using ECoreNetto;

    /// <summary>
    /// represents the payload for the generators that require all <see cref="EEnum"/>,
    /// <see cref="EDataType"/> and <see cref="EClass"/>
    /// </summary>
    public class HandlebarsPayload
    {
        /// <summary>
        /// initializes an instance of the <see cref="HandlebarsPayload"/> class.
        /// </summary>
        /// <param name="rootPackage">
        /// The root <see cref="EPackage"/> of the ECore model
        /// </param>
        /// <param name="enums">
        /// the <see cref="EEnum"/>s in the ECore model
        /// </param>
        /// <param name="dataTypes">
        /// the <see cref="EDataType"/>s in the ECore model
        /// </param>
        /// <param name="classes">
        /// the <see cref="EClass"/>es in the ECore model
        /// </param>
        public HandlebarsPayload(EPackage rootPackage, IEnumerable<EEnum> enums, IEnumerable<EDataType> dataTypes, IEnumerable<EClass> classes)
        {
            this.RootPackage = rootPackage;
            this.Enums = enums.ToArray();
            this.DataTypes = dataTypes.ToArray();
            this.Classes = classes.ToArray();
        }

        /// <summary>
        /// Gets the root <see cref="EPackage"/>
        /// </summary>
        public EPackage RootPackage { get; private set; }

        /// <summary>
        /// Gets the array of <see cref="EEnum"/>
        /// </summary>
        public EEnum[] Enums { get; private set; }

        /// <summary>
        /// Gets the array of <see cref="EDataType"/>
        /// </summary>
        public EDataType[] DataTypes { get; private set; }

        /// <summary>
        /// Gets the array of <see cref="EClass"/>
        /// </summary>
        public EClass[] Classes { get; private set; }

        /// <summary>
        /// Gets the version of the reporting library
        /// </summary>
        public string? Version => Assembly.GetExecutingAssembly().GetName().Version?.ToString();
    }
}
