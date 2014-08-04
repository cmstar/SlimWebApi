using System;
using System.Collections.Generic;
using System.Linq;

namespace cmstar.WebApi.Tests
{
    public class ServiceMethodProvider
    {
        private readonly Guid _guid = Guid.NewGuid();
        private readonly int _id;
        private int _value;

        public ServiceMethodProvider()
            : this(0)
        {
        }

        public ServiceMethodProvider(int id)
        {
            _id = id;
        }

        public Guid GetGuid()
        {
            return _guid;
        }

        public void SetIntValue(int value)
        {
            _value = value;
        }

        public int GetIntValue()
        {
            return _value;
        }

        public string Concat(string x, string y)
        {
            return string.Concat(x, y);
        }

        public string Concat(string x, string y, string z)
        {
            return string.Concat(x, y, z);
        }

        public string Concat(string[] args)
        {
            return string.Concat(args);
        }

        public PlainData GetSameObject(PlainData data)
        {
            return data;
        }

        public Guid Random(PlainData data)
        {
            return Guid.NewGuid();
        }

        public float FloatValue { get; set; }

        public int GetId()
        {
            return _id;
        }

        protected int NonPublicPlus(int x, int y)
        {
            return x + y;
        }

        public int Count(ICollection<int> collection, string[] strings)
        {
            var count = 0;

            if (collection != null)
                count += collection.Count;

            if (strings != null)
                count += strings.Length;

            return count;
        }

        public static DateTime NextDay(DateTime date)
        {
            return date.Date.AddDays(1);
        }

        public static object ConvertToAnonymous(ComplexData data)
        {
            return new { MapCount = data.Map.Count, ValueSum = data.Values.Sum() };
        }

        public int GetEnumIndex(TheEnum e)
        {
            return (int)e;
        }
    }

    public class ComplexData
    {
        public Dictionary<string, int> Map;
        public IList<float> Values { get; set; }
    }

    public class PlainData
    {
        public int Id;
        public string Name { get; set; }
        public DateTime Date;
        public Guid Guid { get; set; }
    }

    public enum TheEnum
    {
        ItemA, ItemB, ItemC
    }
}