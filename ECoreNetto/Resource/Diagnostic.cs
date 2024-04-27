// -------------------------------------------------------------------------------------------------
// <copyright file="Diagnostic.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2024 Starion Group S.A.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Resource
{
    /// <summary>
    /// A noteworthy issue in a document.
    /// </summary>
    public class Diagnostic
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Diagnostic"/> class.
        /// </summary>
        /// <param name="column">
        /// The column location of the issue within the source
        /// </param>
        /// <param name="line">
        /// The line location of the issue within the source..
        /// </param>
        /// <param name="location">
        /// The source location of the issue.
        /// </param>
        /// <param name="message">
        /// The translated message describing the issue.
        /// </param>
        internal Diagnostic(int column, int line, string location, string message)
        {
            this.Column = column;
            this.Line = line;
            this.Location = location;
            this.Message = message;
        }

        /// <summary>
        /// Gets the column location of the issue within the source
        /// </summary>
        /// <remarks>
        ///  Column 1 is the first column.
        /// </remarks>
        public int Column { get; private set; }

        /// <summary>
        /// Gets the line location of the issue within the source.
        /// </summary>
        /// <remarks>
        /// Line 1 is the first line.
        /// </remarks>
        public int Line { get; private set; }

        /// <summary>
        /// Gets the source location of the issue.
        /// </summary>
        /// <remarks>
        /// This will typically be just the URI of the resource containing this diagnostic.
        /// </remarks>
        public string Location { get; private set; }

        /// <summary>
        /// Gets a translated message describing the issue.
        /// </summary>
        public string Message { get; private set; }
    }
}
