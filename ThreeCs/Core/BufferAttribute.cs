namespace ThreeCs.Core
{
    public class BufferAttribute<T> : IBufferAttribute
    {
        public T[] Array;

        public int ItemSize;

        public int buffer { get; set; }

        public bool needsUpdate { get; set; }

        public int length
        {
            get
            {
                return this.Array.Length;
            }
        }

        public BufferAttribute(T[] array, int itemSize)
        {
            this.Array = array;
            this.ItemSize = itemSize;
        }

        public BufferAttribute<T> SetXY(int index, T x, T y)
        {
            index *= this.ItemSize;

            this.Array[index] = x;
            this.Array[index + 1] = y;

            return this;
        }

        public BufferAttribute<T> SetXYZ(int index, T x, T y, T z)
        {
            index *= this.ItemSize;

            this.Array[index] = x;
            this.Array[index + 1] = y;
            this.Array[index + 2] = z;

            return this;
        }

        public BufferAttribute<T> SetXYZW(int index, T x, T y, T z, T w)
        {
            index *= this.ItemSize;

            this.Array[index] = x;
            this.Array[index + 1] = y;
            this.Array[index + 2] = z;
            this.Array[index + 3] = w;

            return this;
        }
    }
}
