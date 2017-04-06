
namespace SiftSharp.SIFT
{
    public class Feature
    {
        // X Coordinate
        public double x;
        // Y Coordinate
        public double y;
        // X coordinate on layer
        public double xLayer;
        // Y coordinate on layer
        public double yLayer;
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
