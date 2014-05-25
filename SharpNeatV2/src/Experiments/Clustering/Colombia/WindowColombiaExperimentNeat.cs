
using SharpNeat.Experiments.Common;

namespace SharpNeat.Experiments.Clustering.Colombia
{
    /// <summary>
    /// Visualizing similar areas in Colombia with climatic data
    /// from http://www.worldclim.org/
    /// </summary>
    class WindowColombiaExperimentNeat : WindowColombiaExperimentHyperNeat
    {
        protected override Phenotype Phenotype
        {
            get { return Phenotype.Neat; }
        }
    }
}
