using System;

namespace VersionOne.ServiceHost.Core.StartupValidation {
    public class ValidationSimpleStep : IValidationStep {
        private readonly ISimpleResolver resolver;
        private readonly ISimpleValidator validator;

        public ValidationSimpleStep(ISimpleValidator validator) : this(validator, null) {}

        public ValidationSimpleStep(ISimpleValidator validator, ISimpleResolver resolver) {
            this.validator = validator;
            this.resolver = resolver;
        }

        public void Run() {
            if(validator == null) {
                throw new InvalidOperationException("Cannot run the step without a validator.");
            }

            var isValid = validator.Validate();

            if(!isValid && (resolver == null || resolver != null && !resolver.Resolve())) {
                throw new ValidationException("Validation error during service initialization.");
            }
        }
    }
}