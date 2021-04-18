using System;
using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Exceptions;
using Stryker.Core.Mutators;

namespace Stryker.Core.Options.Inputs
{
    public class ExcludedMutationsInput : InputDefinition<IEnumerable<string>, IEnumerable<Mutator>>
    {
        public override IEnumerable<string> Default => Enumerable.Empty<string>();

        protected override string Description => @"The given mutators will be excluded for this mutation testrun.
    This argument takes a json array as value. Example: ['string', 'logical']";

        public IEnumerable<Mutator> Validate()
        {
            if (SuppliedInput is { })
            {
                var excludedMutators = new List<Mutator>();

                // Get all mutatorTypes and their descriptions
                var typeDescriptions = Enum.GetValues(typeof(Mutator))
                    .Cast<Mutator>()
                    .ToDictionary(x => x, x => x.GetDescription());

                foreach (var mutatorToExclude in SuppliedInput)
                {
                    // Find any mutatorType that matches the name passed by the user
                    var mutatorDescriptor = typeDescriptions.FirstOrDefault(
                        x => x.Value.ToString().ToLower().Contains(mutatorToExclude.ToLower()));
                    if (mutatorDescriptor.Value is { })
                    {
                        excludedMutators.Add(mutatorDescriptor.Key);
                    }
                    else
                    {
                        throw new StrykerInputException($"Invalid excluded mutator ({mutatorToExclude}).");
                    }
                }

               return excludedMutators;
            }
            return Enumerable.Empty<Mutator>();
        }
    }
}