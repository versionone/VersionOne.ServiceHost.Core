using System;

namespace VersionOne.ServiceHost.Core.StartupValidation {
    // TODO relax message severity in validators when using this kind of step. Application can start, thus it is not errors.
    public class LooseValidationSimpleStep: IValidationStep {
        [HasDependencies]
        public ISimpleResolver Resolver { get; set; }

        [HasDependencies]
        public ISimpleValidator Validator { get; set; }

        public LooseValidationSimpleStep(ISimpleValidator validator, ISimpleResolver resolver) {
            Validator = validator;
            Resolver = resolver;
        }

        public void Run() {
            if(Validator == null) {
                throw new InvalidOperationException("Cannot run the step without a validator.");
            }

            var isValid = Validator.Validate();

            if(!isValid && Resolver != null){
                Resolver.Resolve();
            }
        }
    }
}