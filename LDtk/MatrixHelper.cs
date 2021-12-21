namespace LDtk
{
    internal class MatrixHelper
    {
        public static T[][] CreateMatrix<T>(long width, long height)
        {
            var matrix = new T[width][];

            for (var x = 0; x < matrix.Length; x++)
            {
                matrix[x] = new T[height];
            }

            return matrix;
        }
    }
}