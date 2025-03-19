// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("OUhcItdPTxLAA5Biilch4ZwrHGTdYiUVSKYVCP6kfiCk20az3Ht5Lwq4OxgKNzwzELxyvM03Ozs7Pzo5uDs1Ogq4OzA4uDs7Opl3NJBvF0vPp+c3InCAUIWZBHnfrk5KB+7epKFXOOSkm55Ni+8U64bLVPrxIepSXGpqNsBA9UDleXMRa0+38u1EtUMKGJuH7K4inTu+JoJD7hiYgUrlaIVz+tPjzGVZ1iWMcciYOwFECshIBulfeX2MY3wVu0TuYl3tzwPfDksJPGKwsbTv5p4lAatx60yBsgg1ZoUpobGqwAnqIzqPKtuFL2xT9YLDsjc8HVCq1VxpN+TtQj69zYLBlDG0JM3JCvGNWce5PVogMJl3RMo27To8kJTFHD1t2zg5Ozo7");
        private static int[] order = new int[] { 10,13,10,13,12,9,13,11,13,10,12,11,12,13,14 };
        private static int key = 58;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
