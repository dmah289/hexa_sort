using System;
using UnityEngine;

namespace manhnd_sdk.Scripts.Helpers
{
    public static class AlgebraicMathHelper
    {
        /// <summary>
        /// Find the linear function y = a.x + b based on the two given points
        /// </summary>
        /// <param name="p1">First Point</param>
        /// <param name="p2">Second Point</param>
        /// <param name="allowLog">Allow to print function</param>
        /// <returns>Return the slope and the y-intercept</returns>
        /// <exception cref="ArgumentException"></exception>
        public static (double a, double b) FindLinearParameters((double x, double y) p1, (double x, double y) p2, bool allowLog = true)
        {
            if(Math.Abs(p1.x - p2.x) < 1e-9)
                throw new ArgumentException("The 2 points must not have the same x-coordinate");

            double a = (p2.y - p1.y) / (p2.x - p1.x);
            double b = p1.y - a * p1.x;
            
            if(allowLog)
                Debug.Log($"The linear function found is: y = {a}.x + {b}");

            return (a, b);
        }
    }
}