using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Core;
using SharpNeat.Phenomes;
using System.Diagnostics;
using SharpNeat.Utility;

namespace SharpNeat.Experiments.Common
{
    public delegate void OutputProcessor(int i, double output);

    public interface IViewableEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        /// <summary>
        /// Convenience method for getting the output of a network
        /// </summary>
        /// <param name="box">NEAT network to activate</param>
        /// <param name="inputs">Inputs to be fed into the network</param>
        /// <param name="fun">Delegate that defines what to do with the output</param>
        void Test(IBlackBox box, IList<double> inputs, OutputProcessor fun);
    }
}
