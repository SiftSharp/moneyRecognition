using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiftSharp.SIFT
{
    public class Feature
    {
        public double x;                      /**< x coord */
        public double y;                      /**< y coord */
        public double scl;                    /**< scale of a Lowe-style feature */
        public double ori;                    /**< orientation of a Lowe-style feature */
        public double[] descr;                /**< descriptor */
        public int type;                      /**< feature type, OXFD or LOWE */
        public int category;                  /**< all-purpose feature category */
        public int octave;                    /**< octave Lowe-style feature */
        public int level;                     /**< level Lowe-style feature */
        public double subLevel;                  /**< level Lowe-style feature */
    }
}
