// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace AnuChandy.Fluent.Service.Model.Core
{
    /// <summary>
    /// Type that supports validating it's current state.
    /// </summary>
    public interface ISupportsValidate
    {
        /// <summary>
        /// Validates the current state and throw exception if state is invalid.
        /// </summary>
        /// <param name="propertyName">User facing name of a property of this type which is composed in another type</param>
        void Validate(String propertyName);
    }
}
