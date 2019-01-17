﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ADN.TimeSeries
{
    /// <summary>
    /// A static class that implements Euclidean distance algorithm.
    /// </summary>
    static public class Euclidean
    {
        /// <summary>
        /// Get the value of the calculated Euclidean distance.
        /// </summary>
        /// <param name="serie1">The first <see cref="Array"/> that contains data to calculate the Euclidean distance.</param>
        /// <param name="serie2">The second <see cref="Array"/> that contains data to calculate the Euclidean distance.</param>
        /// <returns>Value of the calculated Euclidean distance.</returns>
        static public double Distance(double[] serie1, double[] serie2)
        {
            // Check arguments
            if (serie1 is null || serie1.Length <= 0)
            {
                throw (new ArgumentNullException("serie1"));
            }

            if (serie2 is null || serie2.Length <= 0)
            {
                throw (new ArgumentNullException("serie2"));
            }

            double totalDist = 0;
            for (int i = 0; i < Math.Min(serie1.Length, serie2.Length); i++)
            {
                totalDist += Math.Pow(serie1[i] - serie2[i], 2);
            }
            return Math.Sqrt(totalDist);
        }
    }
}
