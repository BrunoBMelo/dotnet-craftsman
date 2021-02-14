using System.Collections;
using System.Collections.Generic;

namespace FP.DotNet.Tests.ClassDatas
{
    public class EmailClassData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { true, "bruno.b.melo@live.com" };
            yield return new object[] { false, "bruno.b.melolive.com" };
            yield return new object[] { false, string.Empty };
            yield return new object[] { false, "bruno.b.melo@livecom" };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}