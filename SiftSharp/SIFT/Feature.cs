using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiftSharp.SIFT
{
    public class Feature
    {
        // X Coordinate
        public double x;
        // Y Coordinate
        public double y;
        // Scale of feature
        public double scale;
        // Orientation of feaute in form of percentage of circle
        public double orientation;
        // Descriptor
        public double[] descr;
        // Octave feature is in
        public int octave;
        // Level feature is in
        public int level;
        // Sublevel feature is in
        public double subLevel;
    }
}
