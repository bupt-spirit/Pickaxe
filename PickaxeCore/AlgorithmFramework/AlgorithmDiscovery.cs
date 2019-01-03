using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Pickaxe.AlgorithmFramework
{
    public class AlgorithmDiscovery
    {
        #region Fields

        private ObservableCollection<IAlgorithm> _algorithms;
        private ObservableCollection<IAlgorithm> _preprocessAlgorithms;
        private ObservableCollection<IAlgorithm> _classifyAlgorithms;
        private ObservableCollection<IAlgorithm> _clusterAlgorithms;
        private ObservableCollection<IAlgorithm> _associateAlgorithms;

        #endregion

        #region Properties

        public ObservableCollection<IAlgorithm> Algorithms
        {
            get => _algorithms ?? (
                _algorithms = new ObservableCollection<IAlgorithm>(
                    AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetTypes())
                        .Where((type) => type.IsClass && !type.IsAbstract)
                        .Where((type) => type.GetInterfaces().Contains(typeof(IAlgorithm)))
                        .Select((type) => (IAlgorithm)Activator.CreateInstance(type))
                        .OrderBy((algorithm) => algorithm.Name)
                ));
        }

        public ObservableCollection<IAlgorithm> PreprocessAlgorithms
        {
            get => _preprocessAlgorithms ?? (
                _preprocessAlgorithms = new ObservableCollection<IAlgorithm>(
                    Algorithms.Where((algorithm) => algorithm.Type == AlgorithmType.Preprocess)
                ));
        }

        public ObservableCollection<IAlgorithm> ClassifyAlgorithms
        {
            get => _classifyAlgorithms ?? (
                _classifyAlgorithms = new ObservableCollection<IAlgorithm>(
                    Algorithms.Where((algorithm) => algorithm.Type == AlgorithmType.Classify)
                ));
        }

        public ObservableCollection<IAlgorithm> ClusterAlgorithms
        {
            get => _clusterAlgorithms ?? (
                _clusterAlgorithms = new ObservableCollection<IAlgorithm>(
                    Algorithms.Where((algorithm) => algorithm.Type == AlgorithmType.Cluster)
                ));
        }

        public ObservableCollection<IAlgorithm> AssociateAlgorithms
        {
            get => _associateAlgorithms ?? (
                _associateAlgorithms = new ObservableCollection<IAlgorithm>(
                    Algorithms.Where((algorithm) => algorithm.Type == AlgorithmType.Associate)
                ));
        }

        #endregion

        #region Constructor

        public AlgorithmDiscovery()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            foreach (var dll in Directory.GetFiles(path, "*.dll"))
            {
                try
                {
                    var loadedAssembly = Assembly.LoadFile(dll);
                }
                catch (FileLoadException)
                {
                    // The Assembly has already been loaded
                }
                catch (BadImageFormatException)
                {
                    // Ignore Bad Image
                }
            }
        }

        #endregion
    }
}
