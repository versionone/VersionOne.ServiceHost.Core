using System;

namespace VersionOne.ServiceHost.Core.StartupValidation {
    public class ValidationStep : IValidationStep {
        private readonly IResolver resolver;
        private readonly IValidator validator;

        public ValidationStep(IValidator validator, IResolver resolver) {
            this.validator = validator;
            this.resolver = resolver;
        }

        public void Run() {
            if(validator == null) {
                throw new InvalidOperationException("Cannot run the step without a validator");
            }
            var isValid = validator.Validate();

            if(!isValid && (resolver == null || resolver != null && !resolver.Resolve())) {
                throw new ValidationException("Validation error during service initialization");
            }
        }
    }
}