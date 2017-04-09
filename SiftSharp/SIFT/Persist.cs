using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace SiftSharp.SIFT
{
    public class Persist
    {
        private const int featLength = (8 + 8 + 4 + 4 + 8 + 8 + 4 + 4 + 8) + (8 * 128);

        // File format
        // Header (38 bytes) =>
        //      %SIFT% = 6 bytes
        //      checksum(number of features, features) OR 32 bytes
        // payload (1080 bytes) =>
        //     x,y,xLayer,yLayer,scale,orientation,octave,level => 8,8,4,4,8,8,4,4,8 +
        //     descriptor => 128 * 8 bytes

        /// <summary>
        /// Checks header of sift file
        /// </summary>
        /// <param name="fileContents">Byte array of file</param>
        /// <returns>size of header</returns>
        private static int HeaderCheck(byte[] fileContents)
        {
            List<byte> byteList = fileContents.ToList();

            // Test file format identifier
            if (Encoding.UTF8.GetString(fileContents, 0, 6) != "%SIFT%")
            {
                throw new InvalidDataException("File format incorrect");
            }

            //TODO: checksum

            // Return static 38 bytes length
            return 38;
        }

        /// <summary>
        /// Saves features to file
        /// </summary>
        /// <param name="features">Features to save</param>
        /// <param name="path">Path to file where features should be stored</param>
        /// <returns>Returns true if save is successful</returns>
        public static bool Save(Feature[] features, string path)
        {
            List<byte> byteList = new List<byte>();

            // Add header
            byteList.AddRange(Encoding.UTF8.GetBytes("%SIFT%"));

            // Add 32 bytes of emptyness
            byteList.AddRange(BitConverter.GetBytes(0.0));
            byteList.AddRange(BitConverter.GetBytes(0.0));
            byteList.AddRange(BitConverter.GetBytes(0.0));
            byteList.AddRange(BitConverter.GetBytes(0.0));

            // Check if our header is valid
            HeaderCheck(byteList.ToArray());
            
            // Encode and add features
            foreach (Feature f in features)
            {
                byteList.AddRange(EncodeFeature(f));
            }

            // Compress and save to file
            File.WriteAllBytes(
                path, 
                Compress(byteList.ToArray())
            );

            // Signal successful save
            return true;
        }

        /// <summary>
        /// Loads features from file
        /// </summary>
        /// <param name="path">Path to .sift file</param>
        /// <returns>Features in file</returns>
        public static Feature[] Load(string path)
        {
            List<Feature> features = new List<Feature>();

            // Load and decompress file
            byte[] file = Decompress(File.ReadAllBytes(path));

            // Check header validity and length
            int pntr = HeaderCheck(file);
            
            // Modulo check if file is valid
            if ((file.Length - pntr) % featLength != 0)
            {
                throw new InvalidDataException("Corrupt file...");
            }

            // Decode features and add to list
            for (int i = pntr; i < file.Length; i += featLength)
            {
                Feature feat = DecodeFeature(file, i);
                feat.userDefined = Path.GetFileNameWithoutExtension(path);
                features.Add(feat);
            }

            return features.ToArray();
        }
        
        /// <summary>
        /// Decode a feature
        /// </summary>
        /// <param name="featBytes">File contents</param>
        /// <param name="index">Where in file decoding should start</param>
        /// <returns>Decoded feature</returns>
        public static Feature DecodeFeature(byte[] featBytes, int index = 0)
        {
            Feature feat = new Feature();
            feat.descr = new double[128];

            // Decode feature with following format:
            // x,y,xLayer,yLayer,scale,orientation,octave,level => 8,8,4,4,8,8,4,4,8
            feat.x = BitConverter.ToDouble(featBytes, index + 0);            //  0 ->  8
            feat.y = BitConverter.ToDouble(featBytes, index + 8);            //  8 -> 16
            feat.xLayer = BitConverter.ToInt32(featBytes, index + 16);       // 16 => 20
            feat.yLayer = BitConverter.ToInt32(featBytes, index + 20);       // 20 => 24
            feat.scale = BitConverter.ToDouble(featBytes, index + 24);       // 24 => 32
            feat.orientation = BitConverter.ToDouble(featBytes, index + 32); // 32 => 40
            feat.octave = BitConverter.ToInt32(featBytes, index + 40);       // 40 => 44
            feat.level = BitConverter.ToInt32(featBytes, index + 44);        // 44 => 48
            feat.subLevel = BitConverter.ToDouble(featBytes, index + 48);    // 48 => 56

            // Decode descriptor
            for (int i = 0; i < 128; i++)
            {
                feat.descr[i] = BitConverter.ToDouble(featBytes, index + 56 + (i * 8));
            }
            
            return feat;
        }

        public static byte[] EncodeFeature(Feature feat)
        {
            List<byte> byteFeat = new List<byte>();

            // Encode feature attributes
            byteFeat.AddRange(BitConverter.GetBytes(feat.x));            //  0 ->  8
            byteFeat.AddRange(BitConverter.GetBytes(feat.y));            //  8 -> 16
            byteFeat.AddRange(BitConverter.GetBytes((int)feat.xLayer));  // 16 => 20
            byteFeat.AddRange(BitConverter.GetBytes((int)feat.yLayer));  // 20 => 24
            byteFeat.AddRange(BitConverter.GetBytes(feat.scale));        // 24 => 32
            byteFeat.AddRange(BitConverter.GetBytes(feat.orientation));  // 32 => 40
            byteFeat.AddRange(BitConverter.GetBytes(feat.octave));       // 40 => 44
            byteFeat.AddRange(BitConverter.GetBytes(feat.level));        // 44 => 48
            byteFeat.AddRange(BitConverter.GetBytes(feat.subLevel));     // 48 => 56

            // Encode descriptor
            for (int i = 0; i < 128; i++)
            {
                byteFeat.AddRange(BitConverter.GetBytes(feat.descr[i]));
            }

            return byteFeat.ToArray();
        }

        /// <summary>
        /// Compress byte array
        /// </summary>
        /// <param name="data">data to compress</param>
        /// <returns>compressed data array</returns>
        public static byte[] Compress(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
            {
                dstream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        /// <summary>
        /// Decompress byte array
        /// </summary>
        /// <param name="data">data to decompress</param>
        /// <returns>decompressed data array</returns>
        public static byte[] Decompress(byte[] data)
        {
            MemoryStream input = new MemoryStream(data);
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }
            return output.ToArray();
        }
    }
}
