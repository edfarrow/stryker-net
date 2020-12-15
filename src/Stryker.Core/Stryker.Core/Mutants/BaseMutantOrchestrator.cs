using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Stryker.Core.Options;

namespace Stryker.Core.Mutants
{
    public abstract class BaseMutantOrchestrator
    {
        internal readonly IStrykerOptions _options;

        internal bool MustInjectCoverageLogic =>
            _options != null && _options.Optimizations.HasFlag(OptimizationFlags.CoverageBasedTest) &&
            !_options.Optimizations.HasFlag(OptimizationFlags.CaptureCoveragePerTest);

        internal ICollection<Mutant> Mutants { get; set; }
        internal int MutantCount { get; set; }

        public BaseMutantOrchestrator(IStrykerOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Gets the stored mutants and resets the mutant list to an empty collection
        /// </summary>
        public IReadOnlyCollection<Mutant> GetLatestMutantBatch()
        {
            var tempMutants = Mutants;
            Mutants = new Collection<Mutant>();
            return (IReadOnlyCollection<Mutant>)tempMutants;
        }
    }
}
