using System;

public class Dump{
	public Dump() {

	}
    private static void dumpMatrix(double[,] values) {
        for (int i = 0; i < values.GetLength(0); i++) {
            Console.Write("[ ");
            for (int k = 0; k < values.GetLength(1); k++) {
                Console.Write("{0}", values[i, k] + (k + 1 != values.GetLength(1) ? ", " : ""));
            }
            Console.Write(" ]");
            Console.WriteLine();
        }
    }
}
