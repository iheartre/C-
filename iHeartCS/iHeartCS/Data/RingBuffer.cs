using System.Collections;

namespace iHeartCS.Data
{
    internal class RingBuffer<T> : IEnumerable<T>
    {
        public int Capacity { get { return _buffer.Length; } }

        private T[] _buffer;
        private int _begin;
        private int _end;
        private int _length;

        public int Length { get { return _length; } }

        public RingBuffer(int lenght)
        {
            _buffer = new T[lenght];
            _begin = 0;
            _end = 0;
            _length = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            int position;

            position = _begin;

            for (int i = 0; i < _length; i++)
            {
                yield return _buffer[position];
                position++;
                if (position > _buffer.Length - 1)
                    position = 0;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddRange(T[] values)
        {
            for (int i = 0; i < values.Length; i++)
                Add(values[i]);
        }

        public void Add(T value)
        {
            _buffer[_end] = value;

            if (_length < _buffer.Length)
                _length++;

            if (_begin == _end && _length > 1)
            {
                _begin++;
                if (_begin > _buffer.Length - 1)
                    _begin = 0;
            }

            _end++;
            if (_end > _buffer.Length - 1)
                _end = 0;
        }

        public void RemoveFromBegining(int lenght)
        {
            if (lenght >= _buffer.Length || lenght >= _length)
            {
                _begin = 0;
                _end = 0;
                _length = 0;
                return;
            }

            _begin += lenght;
            if (_begin > _buffer.Length - 1)
                _begin = _begin - _buffer.Length;

            _length -= lenght;
        }

        public void Clear()
        {
            _begin = 0;
            _end = 0;
            _length = 0;
        }
    }
}
