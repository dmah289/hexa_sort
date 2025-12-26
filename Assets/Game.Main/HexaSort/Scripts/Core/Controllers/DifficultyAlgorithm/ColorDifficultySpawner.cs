using System;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using UnityEngine;

namespace HexaSort.Controllers.DifficultyAlgorithm
{
    public class ColorDifficultySpawner : MonoBehaviour
    {
        [Header("----- Cached Weights for Color Distribution -----")]
        [SerializeField] private int[] weights;
        [SerializeField] private int[] cumulativeWeights;
        [SerializeField] private int totalWeight;
        
        public int TotalWeight
        {
            get
            {
                if (totalWeight == 0)
                    PrecalculateWeights();
                
                return totalWeight;
            }
        }
        
        public int[] CumulativeWeights
        {
            get
            {
                if (cumulativeWeights == null || cumulativeWeights.Length == 0)
                    PrecalculateWeights();
                
                return cumulativeWeights;
            }
        }

        private void Awake()
        {
            PrecalculateWeights();
        }

        private void PrecalculateWeights()
        {
            weights = new int[ConstantKey.MaxSpawnedColorPerStack];
            cumulativeWeights = new int[ConstantKey.MaxSpawnedColorPerStack];
            
            // pre-calculate weights for each color layer
            totalWeight = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = (i + 1) * (i + 1); // Quadratic weight
                totalWeight += weights[i];
            }
            
            // precalculate cumulative weights
            // using for easier last color (min pieces) selection
            cumulativeWeights[0] = weights[0];
            for (int i = 1; i < cumulativeWeights.Length; i++)
            {
                cumulativeWeights[i] = cumulativeWeights[i - 1] + weights[i];
            }
        }
    }
}