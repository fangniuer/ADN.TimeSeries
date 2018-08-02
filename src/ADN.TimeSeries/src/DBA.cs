﻿using ADN.Helpers.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADN.TimeSeries
{
    /// <summary>
    /// DTW Barycenter Averaging
    /// </summary>
    public class DBA
    {
        /// <summary>
        /// Generate average of supplied series.
        /// </summary>
        public static double[] Average(List<double[]> series, int maxIterations = 100)
        {
            // Check arguments
            if (ReferenceEquals(series, null) || series.Count <= 0)
            {
                throw (new ArgumentNullException("series"));
            }

            if (series.Count == 1)
            {
                return series[0];
            }

            int length = 0;
            for (int i = 0; i < series.Count; i++)
            {
                length += series[i].Length;
            }
            length /= series.Count;

            //initialize to series closest to median min/max after detrending
            List<double[]> tempSeries = series.Select(Detrend).ToList();
            List<int> maxIndexes = tempSeries.Select(x => x.IndexOfMax()).ToList();
            List<int> minIndexes = tempSeries.Select(x => x.IndexOfMin()).ToList();
            int medianMaxIndex = maxIndexes.Median();
            int medianMinIndex = minIndexes.Median();
            double[] average = new double[length];
            for (int i = 0; i < length; i++)
            {
                average[i] = 0;
            }

            //this list will hold the values of each aligned point, 
            //later used to construct the aligned average
            List<double>[] points = new List<double>[length];
            for (int i = 0; i < length; i++)
            {
                points[i] = new List<double>();
            }

            double prevTotalDist = -1;
            double totalDist = -2;

            //sometimes the process gets "stuck" in a loop between two different states
            //so we have to set a hard limit to end the loop
            int count = 0;

            //get the path between each series and the average
            while (totalDist != prevTotalDist && count < maxIterations)
            {
                prevTotalDist = totalDist;

                //clear the points from the last calculation
                foreach (List<double> list in points)
                {
                    list.Clear();
                }

                //here we do the alignment for every series
                foreach (double[] ts in series)
                {
                    DTW dtw = new DTW(ts, average, 3);
                    Tuple<int, int>[] path = dtw.GetPath();

                    //use the path to distribute the points according to the warping
                    Array.ForEach(path, x => points[x.Item2].Add(ts[x.Item1]));
                }

                //Then simply construct the new average series by taking the mean of every List in points.
                for (int i = 0; i < points.Length; i++)
                {
                    if (points[i].Count > 0)
                    {
                        average[i] = points[i].Average();
                    }
                }

                //calculate Euclidean distance to stop the loop if no further improvement can be made
                double[] average1 = average;
                totalDist = 0;
                for (int i = 0; i < series.Count; i++)
                {
                    for (int j = 0; j < series[i].Length; j++)
                    {
                        if (average1.Length <= j) break;
                        totalDist += Math.Pow(series[i][j] - average1[j], 2);
                    }
                }
                count++;
            }

            return average;
        }

        private static double[] Detrend(double[] input)
        {
            int len = input.Length;
            double step = (input[len - 1] - input[0]) / len;
            double[] output = new double[len];

            for (int i = 0; i < len; i++)
            {
                output[i] = input[i] - step * i;
            }

            return output;
        }
    }
}