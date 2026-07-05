// ------------------------------------------------------------------------------------------------
// <copyright file="ConstraintInfo.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Extensions
{
    /// <summary>
    /// Represents a named constraint (rule) declared on an <see cref="EClass"/>, together with its
    /// specification body and language, as read from the Ecore constraint/OCL annotations.
    /// </summary>
    public class ConstraintInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintInfo"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the constraint.
        /// </param>
        /// <param name="body">
        /// The specification body (e.g. the OCL expression), or an empty string when none is declared.
        /// </param>
        /// <param name="language">
        /// The specification language (e.g. <c>OCL</c>), or an empty string when unknown.
        /// </param>
        public ConstraintInfo(string name, string body, string language)
        {
            this.Name = name;
            this.Body = body;
            this.Language = language;
        }

        /// <summary>
        /// Gets the name of the constraint.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the specification body of the constraint (e.g. the OCL expression).
        /// </summary>
        public string Body { get; }

        /// <summary>
        /// Gets the specification language of the constraint (e.g. <c>OCL</c>).
        /// </summary>
        public string Language { get; }
    }
}
