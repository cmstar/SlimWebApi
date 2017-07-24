using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace cmstar.WebApi
{
    public class SimpleServiceProvider
    {
        public static double Sum(IList<double> values)
        {
            return values?.Sum() ?? 0;
        }

        private readonly Random _random = new Random();
        private readonly Guid _guid = Guid.NewGuid();

        public string PropValue { get; set; }

        public Guid GetGuid()
        {
            return _guid;
        }

        public void DoNothingWith(DateTime date, DateTime? nullableDate)
        {
        }

        public int PlusRandom(int x, int y)
        {
            return x + y + Random(1000);
        }

        public void Error(int i)
        {
            throw new Exception("An error occured.");
        }

        public SimpleObject GetSelf(SimpleObject simpleObject)
        {
            return simpleObject;
        }

        public string InputStream(string head, Stream input, string tail)
        {
            var ms = new MemoryStream();
            var buf = new byte[1024];
            int len;
            while ((len = input.Read(buf, 0, buf.Length)) > 0)
            {
                ms.Write(buf, 0, len);
            }

            var data = ms.ToArray();
            var s = Encoding.UTF8.GetString(data);
            return string.Concat(head, s, tail);
        }

        private int Random(int max)
        {
            return _random.Next(max);
        }
    }
}