using System.Text;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;

namespace Config
{
    public static class AssetIdHelper
    {
        /**
         * Since assetIds underneath are a 128bit value and md5 hashing creates a
         * consistent-output, low-collision 128bit value, we can use it
         * to convert arbitrary length strings into a suitable assetId.
         */
        public static NetworkHash128 GenerateAssetIdFromString(string source)
        {
            byte[] hash;
            using (MD5 md5Hash = MD5.Create())
            {
                hash = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(source));
            }

            /**
             * Annoyingly assetIds only have an assignment method for strings, so
             * we just do the bytes unrolled manually.
             */
            NetworkHash128 assetId = new NetworkHash128();
            assetId.i0 = hash[0];
            assetId.i1 = hash[1];
            assetId.i2 = hash[2];
            assetId.i3 = hash[3];
            assetId.i4 = hash[4];
            assetId.i5 = hash[5];
            assetId.i6 = hash[6];
            assetId.i7 = hash[7];
            assetId.i8 = hash[8];
            assetId.i9 = hash[9];
            assetId.i10 = hash[10];
            assetId.i11 = hash[11];
            assetId.i12 = hash[12];
            assetId.i13 = hash[13];
            assetId.i14 = hash[14];
            assetId.i15 = hash[15];

            Debug.Log("Generated assetId for " + source + " as " + assetId.ToString());

            return assetId;
        }
    }
}
