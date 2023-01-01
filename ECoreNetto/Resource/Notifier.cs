// -------------------------------------------------------------------------------------------------
// <copyright file="Notifier.cs" company="RHEA System S.A.">
//
//   Copyright 2017-2023 RHEA System S.A.
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

namespace ECoreNetto
{
    /// <summary>
    /// A source of notification delivery. Since all modeled objects will be notifiers, the method names start with "e" to distinguish the EMF methods from the client's methods.
    /// </summary>
    /// <remarks>
    /// This is the abstract super class of all classes in the EMF framework
    /// </remarks>
    public abstract class Notifier
    {
    }
}
