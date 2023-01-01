// -------------------------------------------------------------------------------------------------
// <copyright file="ContainerList.cs" company="RHEA System S.A.">
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
    using System;
    using System.Collections.Generic;
    using ECoreNetto.Utils;

    /// <summary>
    /// List type that represents a composite aggregation.
    /// </summary>
    /// <typeparam name="T">
    /// The type of <see cref="EObject"/> that the list contains
    /// </typeparam>
    public class ContainerList<T> : List<T> where T : EObject
    {
        /// <summary>
        /// backing field for the container of this <see cref="ContainerList{T}"/>
        /// </summary>
        private readonly EObject container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerList{T}"/> class.
        /// </summary>
        /// <param name="container">
        /// The <see cref="EObject"/> that contains the <see cref="ContainerList{T}"/>
        /// </param>
        public ContainerList(EObject container)
        {
            this.container = container;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerList{T}"/> class.
        /// </summary>
        /// <param name="containerList">
        /// The <see cref="ContainerList{T}"/> whose values are copied
        /// </param>
        /// <param name="container">
        /// The <see cref="EObject"/> that contains the <see cref="ContainerList{T}"/>
        /// </param>
        public ContainerList(IEnumerable<T> containerList, EObject container) 
            : base(containerList)
        {
            this.container = container;
        }

        /// <summary>
        /// Adds a new <see cref="EObject"/> in the <see cref="List{T}"/> and sets its <see cref="EObject.EContainer"/> property
        /// </summary>
        /// <param name="object">
        /// The new <see cref="EObject"/> to add
        /// </param>
        public new void Add(T @object)
        {
            @object.RemoveFromContainer();

            @object.EContainer = this.container;
            
            if (this.Contains(@object))
            {
                throw new InvalidOperationException($"The added item already exists {@object}.");
            }

            base.Add(@object);
        }

        /// <summary>
        /// Adds a <see cref="IEnumerable{T}"/> of <see cref="EObject"/> in the <see cref="List{T}"/> and sets their <see cref="EObject.EContainer"/> property
        /// </summary>
        /// <param name="objects">
        /// the new <see cref="EObject"/>s to add
        /// </param>
        public new void AddRange(IEnumerable<T> objects)
        {
            foreach (var @object in objects)
            {
                @object.EContainer = this.container;
                this.Add(@object);
            }
        }

        /// <summary>
        /// Gets or sets the value of the <see cref="EObject"/> associated with the specified index.
        /// </summary>
        /// <param name="index">the index</param>
        /// <returns>The <see cref="EObject"/> with the specified index (only for get).</returns>
        public new T this[int index]
        {
            get
            {
                if (index < 0 || index >= base.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"index is {index}, valid range is 0 to {this.Count - 1}.");
                }

                return base[index];
            }

            set
            {
                if (index < 0 || index >= base.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"index is {index}, valid range is 0 to {this.Count - 1}.");
                }

                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (this.Contains(value) && base[index] != value)
                {
                    throw new InvalidOperationException($"The added item already exists {value}.");
                }

                value.RemoveFromContainer();

                value.EContainer = this.container;
                base[index] = value;
            }
        }
    }
}
