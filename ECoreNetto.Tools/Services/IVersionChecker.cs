// ------------------------------------------------------------------------------------------------
// <copyright file="IVersionChecker.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Tools.Services
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A service that checks whether a newer version is available
    /// </summary>
    public interface IVersionChecker
    {
        /// <summary>
        /// Checks for the lastest release
        /// </summary>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/> used to cancel the operation
        /// </param>
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
