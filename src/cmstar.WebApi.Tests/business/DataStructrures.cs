using System;
using System.Collections.Generic;
using System.Drawing;

namespace cmstar.WebApi
{
    public enum AbcItemType
    {
        ItemA,
        ItemB,
        ItemC
    }

    public class SimpleObject
    {
        public int Id;
        public string Name { get; set; }
        public double Number { get; set; }
        public DateTime DateTime;
        public Guid Guid { get; set; }
        public AbcItemType Abc { get; set; }
    }

    public class ComplexData
    {
        public SimpleObject SimpleObject { get; set; }
        public Dictionary<string, int> StringIntegerMap;
        public IList<Point> Points { get; set; }
    }
}