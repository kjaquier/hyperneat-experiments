HyperNEAT experiments
=====================

Content
-------

This repository contains a copy of [SharpNEAT 2.2](http://sharpneat.sourceforge.net/) with a few classification and clustering experiments.

The experiments are in a separate project named `SharpNeatExperiments`, which contains :

* In the `Classification` folder, all the classification experiments :
   * `Iris`, `PIMA` and `Voices` are using datasets without geometrical representations of the inputs. The goals is just to associate each sample with the right label, which is given with the dataset. I'm using [specificity, sensibility, accuracy](http://en.wikipedia.org/wiki/Confusion_matrix) and the mean square error for the fitness function.
   * `OCR` is an Optical Character Recognition experiment with a 2D matrix of pixels for each sample, thus letting HyperNEAT take advantage of each pixel's position. Apart from the positions of the input neurons, it has the same settings as the other classification experiments.
* In the `Clustering` folder, all the clustering experiments :
    * `Old` contains the first attempts to do clustering with HyperNEAT. It doesn't work and has not been updated. It can just be ignored.
    * `MapClustering` is a soft-clustering experiment (with fixed number of clusters) using geolocated data. `ColombiaExperimentHyperNeat` is an instance of this experiment based on rescaled data of some area of Colombia.
    * `WindowMapClustering` is the same experiment with a different network topology. Instead of passing all samples as input, networks are activated with one sample at a time, with a window (comparable to a filter in image processing) that defines which neighboring samples to pass with it. This way, the number of inputs and outputs is reduced, thus giving the possibility to compare performances with NEAT and to try setting a hidden layer. But with HyperNEAT, we can't take advantage of the samples' position in the hole area anymore.

More details on MapClustering and WindowMapClustering
-------

The goal is to visualize similar areas on a map, based on several features. Here is how the experiment is implemented :

- 3D matrix as input layer containing all the samples. X and Y axis for geographic positions, Z for feature index (for ex : -0.1, -0.2, etc.)
- 3D matrix as output layers. Same positions on X and Y as for the input layer, while Z axis is for cluster index (for ex : +0.1, +0.2, etc). Each level on the Z axis makes an output layer.
- Connections go from the input layer to each output layer.
- On both layers (and every axis), neurons positions are normalized between -1 and +1.
- Input values are normalized between -1 and +1, and rescaled so that the average is 0 and the standard deviation is 1.
- Fitness function is `activity + d_inter / (1+d_intra)` (I also tried other variations, but these are the three metrics I'm using).

`activity` is the average difference between the highest and the lowest activation level of each cluster.
I compute the center of a cluster with a weighted arithmetic mean of all the samples, using the activation level of each sample (given by the output of the network) as a weight.
So, `d_inter` is the average distance between the centers of the clusters, while the `d_intra` of a single cluster is the weighted distance between each sample and the center. The final `d_intra` is the average `d_intra`.

The idea is that HyperNEAT should be able to take advantage of the patterns that can be seen at the scale of the hole map, but as a consequence this experiment has a high number of inputs and outputs. So NEAT is unable to find solutions.

`WindowMapClustering` is the same experiment with a different network topology. Instead of passing all samples as input, networks are activated with one sample at a time, with a window (comparable to a filter in image processing) that defines which neighboring samples to pass with it. This way, the number of inputs and outputs is reduced, thus giving the possibility to compare performances with NEAT, and also to try setting a hidden layer. But HyperNEAT can't take advantage of the samples' position in the hole area anymore.

Remarks
-------

* Domain views are available for most experiments.
* Compared to the original source code of SharpNEAT, the `SharpNeat.Domains.XmlUtils` class has been modified.
* This is prototyping code which clearly needs some refactorisation. I tried to keep it as understandable as possible, though.
