namespace ThreeCs.Core
{
    public class BufferAttribute<T> : IBufferAttribute
    {
        public T[] array;

        public int itemSize;

        public int buffer { get; set; }

        public bool needsUpdate { get; set; }

        public int length
        {
            get
            {
                return this.array.Length;
            }
        }

        public BufferAttribute(T[] array, int itemSize)
        {
            this.array = array;
            this.itemSize = itemSize;
        }

        public BufferAttribute<T> SetXY(int index, T x, T y)
        {
            index *= this.itemSize;

            this.array[index] = x;
            this.array[index + 1] = y;

            return this;
        }

        public BufferAttribute<T> SetXYZ(int index, T x, T y, T z)
        {
            index *= this.itemSize;

            this.array[index] = x;
            this.array[index + 1] = y;
            this.array[index + 2] = z;

            return this;
        }

        public BufferAttribute<T> SetXYZW(int index, T x, T y, T z, T w)
        {
            index *= this.itemSize;

            this.array[index] = x;
            this.array[index + 1] = y;
            this.array[index + 2] = z;
            this.array[index + 3] = w;

            return this;
        }
    }
}
