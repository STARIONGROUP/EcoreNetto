// -------------------------------------------------------------------------------------------------
// <copyright file="EObject.cs" company="Starion Group S.A.">
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

namespace ECoreNetto
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;

    /// <summary>
    /// An <see cref="EObject"/> is the base of all modeled objects. All the method names start with "e" to distinguish the EMF methods from the client methods.
    /// It provides support for the behaviors and features common to all modeled objects.
    /// <see cref="EObject"/> is the equivalent of <see cref="System.Object"/>.
    /// </summary>
    public abstract class EObject : Notifier
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EObject"/> class.
        /// </summary>
        /// <param name="resource">
        /// The <see cref="ECoreNetto.Resource.Resource"/> containing all instantiated <see cref="EObject"/>
        /// </param>
        protected EObject(Resource.Resource resource)
        {
            this.EResource = resource;
            this.Attributes = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets the <see cref="ECoreNetto.Resource.Resource"/> containing all instantiated <see cref="EObject"/>
        /// </summary>
        /// <remarks>
        /// An object is contained in a <see cref="ECoreNetto.Resource.Resource"/> if it, or one of it's containers, appears in the contents of that <see cref="ECoreNetto.Resource.Resource"/>. 
        /// An object must be contained by a <see cref="ECoreNetto.Resource.Resource"/> in order to be serialized. 
        /// </remarks>
        public Resource.Resource EResource { get; private set; }

        /// <summary>
        /// Gets or sets the current top package name used during .
        /// </summary>
        internal static string TopPackageName { get; set; }

        /// <summary>
        /// Gets the identifier for this <see cref="EModelElement"/>
        /// </summary>
        public virtual string Identifier { get; set; }

        /// <summary>
        /// Gets the collection of xml-attributes of this object that are present
        /// in the ECore file. The xml-attributes are stored here as key-value pair
        /// key: attribute name
        /// value: attribute value
        /// </summary>
        protected Dictionary<string, string> Attributes { get; private set; }

        /// <summary>
        /// Returns the meta class.
        /// </summary>
        /// <remarks>
        /// The meta class defines the features available for reflective access.
        /// </remarks>
        /// <returns>
        /// The meta class.
        /// </returns>
        public EClass EClass()
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Gets the containing object, or null.
        /// </summary>
        /// <remarks>
        /// An object is contained by another object if it appears in the contents of that object.
        /// The object will be contained by a containment <see cref="EStructuralFeature"/> of the containing object.
        /// </remarks>
        /// <returns>
        /// The containing <see cref="EObject"/>, or null.
        /// </returns>
        public EObject EContainer { get; internal set; }

        /// <summary>
        /// Returns the particular <see cref="EStructuralFeature"/> of the container that actually holds the object, or null, if there is no container. 
        /// Because of support for wildcard content, this feature may be an attribute representing a <see cref="EStructuralFeature"/> map; 
        /// in this case the object is referenced by the containment <see cref="EStructuralFeature"/> of an entry in the map, i.e., the <see cref="EContainmentFeature"/>. 
        /// </summary>
        /// <returns>
        /// The <see cref="EStructuralFeature"/> that actually contains the object.
        /// </returns>
        public EStructuralFeature EContainingFeature()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the containment <see cref="EStructuralFeature"/> that properly contains the object, or null, if there is no container. 
        /// Because of support for wildcard content, this feature may not be a direct <see cref="EStructuralFeature"/> of the container's class, 
        /// but rather a <see cref="EStructuralFeature"/> of an entry in a <see cref="EStructuralFeature"/> map feature of the container's class. 
        /// </summary>
        /// <returns>
        /// The feature that properly contains the object.
        /// </returns>
        public EReference EContainmentFeature()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a list view of the content objects; it is not modifiable. 
        /// </summary>
        /// <remarks>
        /// This will be the list of <see cref="EObject"/>s determined by the contents of the containment <see cref="EStructuralFeature"/>s of this object's meta class.
        /// Objects can, indirectly, be removed and will change to reflect container changes. The implicit tree of contents is also directly available. 
        /// </remarks>
        /// <returns>
        /// A list view of the content objects.
        /// </returns>
        public IEnumerable<EObject> EContents()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a tree iterator that iterates over all the direct contents and indirect contents of this object.
        /// </summary>
        /// <returns>
        /// A tree iterator that iterates over all contents.
        /// </returns>
        public IEnumerable<EObject> EAllContents()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates whether this object is a proxy. 
        /// </summary>
        /// <remarks>
        /// A proxy is an object that is defined in a <see cref="Resource"/> that has not been loaded. An object may be a proxy either 
        /// because proxy resolution was disabled when the object was accessed (see eGet(EStructuralFeature,boolean)) 
        /// or because proxy resolution failed
        /// </remarks>
        /// <returns>
        /// true if this object is a proxy or false, otherwise.
        /// </returns>
        public bool EIsProxy()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a list view of the cross referenced objects; it is not modifiable. 
        /// </summary>
        /// <remarks>
        /// This will be the list of <see cref="EObject"/>s determined by the contents of the reference <see cref="EStructuralFeature"/>s of this object's meta class, 
        /// excluding containment <see cref="EStructuralFeature"/>s and their opposites. The cross reference list's iterator will be of type EContentsEList.FeatureIterator, 
        /// for efficient determination of the feature of each cross reference in the list,
        /// </remarks>
        /// <returns>
        /// A list view of the cross referenced objects. 
        /// </returns>
        public IEnumerable<EObject> ECrossReferences()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the value of the given <see cref="EStructuralFeature"/> of this object. 
        /// </summary>
        /// <param name="feature">
        /// The <see cref="EStructuralFeature"/> of the value to fetch.
        /// </param>
        /// <returns>
        /// The value of the given <see cref="EStructuralFeature"/> of this object.
        /// </returns>
        public object EGet(EStructuralFeature feature)
        {
            return this.EGet(feature, true);
        }

        /// <summary>
        /// Returns the value of the given <see cref="EStructuralFeature"/> of the object; the value is optionally resolved before it is returned.
        /// </summary>
        /// <remarks>
        /// If the <see cref="EStructuralFeature"/> is many-valued, the result will be an EList and each object in the list will be an instance of the <see cref="EStructuralFeature"/>'s type;
        /// the list's contents are not affected by resolve argument. Otherwise the result directly will be an instance of the feature's type; 
        /// if it is a proxy, it is resolved.
        /// </remarks>
        /// <param name="feature">
        /// The <see cref="EStructuralFeature"/> of the value to fetch.
        /// </param>
        /// <param name="resolve">
        /// Whether to resolve.
        /// </param>
        /// <returns>
        /// The value of the given <see cref="EStructuralFeature"/> of the object.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If the <see cref="EStructuralFeature"/> is not one the meta class's features and is also not affiliated with one of the meta class's features. 
        /// </exception>        
        public object EGet(EStructuralFeature feature, bool resolve)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the value of the given <see cref="EStructuralFeature"/> of the object to the new value. 
        /// </summary>
        /// <remarks>
        /// If the <see cref="EStructuralFeature"/> is many-valued, the new value must be an EList and each object in that list must be an instance of the <see cref="EStructuralFeature"/>'s type; 
        /// the existing contents are cleared and the contents of the new value are added. However, if the new value is the content list itself, 
        /// or is modified as a side effect of modifying the content list (i.e., if it is a view on the content list), the behavior is undefined 
        /// and will likely result in simply clearing the list. If the <see cref="EStructuralFeature"/> is single-valued, the new value directly must be an instance 
        /// of the <see cref="EStructuralFeature"/>'s type and it becomes the new value of the feature of the object. If the <see cref="EStructuralFeature"/> is unsettable, 
        /// the modeled state becomes set; otherwise, the feature may still not considered set if the new value is the same as the default.
        /// </remarks>
        /// <param name="feature">
        /// The <see cref="EStructuralFeature"/> of the value to set.
        /// </param>
        /// <param name="newValue">
        /// the value to set the <see cref="EStructuralFeature"/> to.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the <see cref="EStructuralFeature"/> is not one the meta class's <see cref="EStructuralFeature"/>, or it isn't changeable.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// If there is a type conflict.
        /// </exception>
        public void ESet(EStructuralFeature feature, object newValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns whether the feature of the object is considered to be set. 
        /// </summary>
        /// <remarks>
        /// If the <see cref="EStructuralFeature"/> is many-valued, the value must be an EList and the <see cref="EStructuralFeature"/> is considered set if the list is not empty.
        /// If the <see cref="EStructuralFeature"/> is unsettable, the modeled state is directly available and is used. 
        /// Otherwise, the unresolved value of the <see cref="EStructuralFeature"/> of the object is compared against the <see cref="EStructuralFeature"/>'s default value or the meta class's 
        /// default value, as appropriate; the <see cref="EStructuralFeature"/> is considered set if it's not the same as the default. 
        /// This property can affect serialization, since defaults are typically omitted in a compact serialization. 
        /// </remarks>
        /// <param name="feature">
        /// The <see cref="EStructuralFeature"/> in question.
        /// </param>
        /// <returns>
        /// Whether the <see cref="EStructuralFeature"/> of the object is set.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If the feature is not one the meta class's <see cref="EStructuralFeature"/>s.
        /// </exception>
        public bool EIsSet(EStructuralFeature feature)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unsets the <see cref="EStructuralFeature"/> of the object. 
        /// </summary>
        /// <remarks>
        /// If the <see cref="EStructuralFeature"/> is many-valued, the value must be an EList and that list is cleared. Otherwise, the value of the <see cref="EStructuralFeature"/> of the 
        /// object is set to the <see cref="EStructuralFeature"/>'s default value or the meta class's default value, as appropriate. If the <see cref="EStructuralFeature"/> is unsettable, 
        /// the modeled state becomes unset. In any case, the <see cref="EStructuralFeature"/> will no longer be considered set. 
        /// </remarks>
        /// <param name="feature">
        /// The <see cref="EStructuralFeature"/> in question.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the feature is not one the meta class's <see cref="EStructuralFeature"/>s, or it isn't changeable.
        /// </exception>
        public void EUnSet(EStructuralFeature feature)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Invokes the specified operation of the object. If the operation has parameters, then corresponding arguments must be supplied.
        /// There are no optional parameters in Ecore operations. 
        /// </summary>
        /// <remarks>
        /// If the operation is a void operation, then on successful execution, the result of this invocation is null. Otherwise, 
        /// if the operation is multi-valued, then an EList is returned (possibly empty). 
        /// If single-valued, then an instance of the operation's type is returned, or possibly null. 
        /// If the invoked operation fails with an exception, then it is re-thrown, wrapped in an InvocationTargetException. 
        /// </remarks>
        /// <param name="operation">
        /// The <see cref="EOperation"/> in question
        /// </param>
        /// <param name="arguments">
        /// The arguments for the <see cref="EOperation"/>.
        /// </param>
        /// <returns>
        /// The resulting <see cref="object"/> or null
        /// </returns>
        public object EInvoke(EOperation operation, IEnumerable<object> arguments)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Process the current node and its children of the current <see cref="XmlReader"/>
        /// </summary>
        /// <param name="element">
        /// The <see cref="XmlReader"/>
        /// </param>
        public virtual void ReadXml(XmlNode element)
        {
            this.SaveAttributes(element);
            this.ReadChildNodes(element);
        }

        /// <summary>
        /// Set the properties of this <see cref="EObject"/>
        /// </summary>
        internal abstract void SetProperties();

        /// <summary>
        /// Read the attributes of the current node to set the properties of this <see cref="EModelElement"/>
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        protected void SaveAttributes(XmlNode reader)
        {
            foreach (XmlAttribute readerAttribute in reader.Attributes)
            {
                var attributeValue = this.ProcessAttributeValue(readerAttribute.Name, readerAttribute.Value);
                this.Attributes.Add(readerAttribute.Name, attributeValue);
            }
        }
        
        /// <summary>
        /// Instantiate new <see cref="EObject"/> from the current node of the <see cref="XmlReader"/>
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/></param>
        protected virtual void DeserializeChildNode(XmlNode reader)
        {
        }
        
        /// <summary>
        /// Process the <see cref="XmlReader"/> to read the child nodes
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/></param>
        private void ReadChildNodes(XmlNode reader)
        {
            foreach (XmlNode readerChildNode in reader.ChildNodes)
            {
                this.DeserializeChildNode(readerChildNode);
            }
        }

        /// <summary>
        /// Process the attribute value and rewrite if required.
        /// </summary>
        /// <param name="attributeName">
        /// The attribute name.
        /// </param>
        /// <param name="attributeValue">
        /// The attribute value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ProcessAttributeValue(string attributeName, string attributeValue)
        {
            var referenceAttributes = new[] { "eType", "eSuperTypes", "eOpposite" };

            if (!referenceAttributes.Contains(attributeName))
            {
                // nothing to do
                return attributeValue;
            }

            // split the attribute value to support one or many value parts
            var attributeValueParts = attributeValue.Split(' ');

            // rewrite implicit reference attributes to point to the current top container.ecore
            for (var i = 0; i < attributeValueParts.Length; i++)
            {
                if (attributeValueParts[i].StartsWith("#//"))
                {
                    attributeValueParts[i] = $"{TopPackageName}.ecore{attributeValueParts[i]}";
                }
            }

            // rebuild the attribute value
            return string.Join(" ", attributeValueParts);
        }
    }
}
