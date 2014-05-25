HyperNEAT experiments
=====================

This repository contains a copy of [SharpNEAT 2.2](http://sharpneat.sourceforge.net/) with a few classification and clustering experiments.

The experiments are in a separate project named `SharpNeatExperiments`, which contains :

* In the `Classification` folder, all the classification experiments.
   * `Iris`, `PIMA` and `Voices` are using datasets without geometrical representations of the inputs.
   * `OCR` is an Optical Character Recognition experiment with a 2D matrix of pixels for each sample, thus letting HyperNEAT take advantage of each pixel's position.
* In the `Clustering` folder, all the clustering experiments.
    * `Old` contains the first attempts to do clustering with HyperNEAT. It doesn't work and has not been updated. It can just be ignored.
    * `MapClustering` is the clustering experiment with geolocated data. `ColombiaExperimentHyperNeat` is an instance of this experiment using rescaled data of some area of Colombia.
    * `WindowMapClustering` is the same experiment with a different network topology. Instead of passing all samples as input, networks are activated with one sample at a time, with a window (comparable to a filter in image processing) that defines which neighboring samples to pass with it. This way, the number of inputs and outputs is reduced, thus giving the possibility to compare performances with NEAT and to try setting a hidden layer. But with HyperNEAT, we can't take advantage of the samples' position in the hole area anymore.

Domain views are available for most experiments.

Note that this is prototyping code which clearly needs some refactorisation. I tried to keep it as understandable as possible, though.
