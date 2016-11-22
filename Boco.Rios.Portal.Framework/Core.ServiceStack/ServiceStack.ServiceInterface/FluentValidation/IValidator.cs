#region License
// Copyright (c) Jeremy Skinner (http://www.jeremyskinner.co.uk)
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at http://www.codeplex.com/FluentValidation
#endregion

namespace ServiceStack.FluentValidation
{
    using System;
    using System.Collections.Generic;
    using Internal;
    using Results;

    /// <summary>
    /// Defines a validator for a particualr type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IValidator<in T> : IValidator {
        /// <summary>
        /// Validates the specified instance.
        /// </summary>
        /// <param name="instance">The instance to validate</param>
        /// <returns>A ValidationResult object containing any validation failures.</returns>
        ValidationResult Validate(T instance);

        /// <summary>
        /// Sets the cascade mode for all rules within this validator.
        /// </summary>
        CascadeMode CascadeMode { get; set; }
    }

    /// <summary>
    /// Defines a validator for a particular type.
    /// </summary>
    public interface IValidator : IEnumerable<IValidationRule> {
        /// <summary>
        /// Validates the specified instance
        /// </summary>
        /// <param name="instance"></param>
        /// <returns>A ValidationResult containing any validation failures</returns>
        ValidationResult Validate(object instance);


        /// <summary>
        /// Validates the specified instance.
        /// </summary>
        /// <param name="context">A ValidationContext</param>
        /// <returns>A ValidationResult object containy any validation failures.</returns>
        ValidationResult Validate(ValidationContext context);

        /// <summary>
        /// Creates a hook to access various meta data properties
        /// </summary>
        /// <returns>A IValidatorDescriptor object which contains methods to access metadata</returns>
        IValidatorDescriptor CreateDescriptor();

        /// <summary>
        /// Checks to see whether the validator can validate objects of the specified type
        /// </summary>
        bool CanValidateInstancesOfType(Type type);
    }
}