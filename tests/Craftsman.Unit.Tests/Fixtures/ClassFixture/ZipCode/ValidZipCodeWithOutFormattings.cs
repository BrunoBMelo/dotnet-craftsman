using System.Collections;
using System.Collections.Generic;

namespace Craftsman.Unit.Tests.Fixtures.ClassFixture.ZipCode
{
    public class ValidZipCodeWithOutFormattings : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { "04173020" };
        }

        IEnumerator IEnumerable.GetEnumerator()
        => default;
    }
}