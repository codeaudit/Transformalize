//-------------------------------------------------------------------------------
// <copyright file="WeakPropertyValue.cs" company="Ninject Project Contributors">
//   Copyright (c) 2009-2013 Ninject Project Contributors
//   Authors: Remo Gloor (remo.gloor@gmail.com)
//           
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   you may not use this file except in compliance with one of the Licenses.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//   or
//       http://www.microsoft.com/opensource/licenses.mspx
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------

using System;

namespace Transformalize.Libs.Ninject.Parameters
{
    /// <summary>
    ///     Overrides the injected value of a property.
    ///     Keeps a weak reference to the value.
    /// </summary>
    public class WeakPropertyValue : Parameter, IPropertyValue
    {
        private readonly WeakReference weakReference;

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeakPropertyValue" /> class.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="value">The value to inject into the property.</param>
        public WeakPropertyValue(string name, object value)
            : base(name, (object) null, false)
        {
            weakReference = new WeakReference(value);
            ValueCallback = (ctx, target) => weakReference.Target;
        }
    }
}