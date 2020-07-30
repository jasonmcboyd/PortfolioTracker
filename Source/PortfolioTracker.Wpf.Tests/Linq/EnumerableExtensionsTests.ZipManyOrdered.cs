using Microsoft.VisualStudio.TestTools.UnitTesting;
using PortfolioTracker.Wpf.Linq;
using System.Collections.Generic;
using System.Linq;

namespace PortfolioManager.Wpf.Tests
{
    [TestClass]
    public class EnumerableExtensionsTests
    {
        public static IEnumerable<object[]> GetLists()
        {
            var lists = new List<IEnumerable<int>>
            {
                new List<int>{ 1, 2, 3 },
                new List<int>{ 2, 3, 4 },
                new List<int>{ 2, 4, 6 },
                new List<int>{ 4, 5, 6 },

            };

            yield return new object[]
            {
                lists,
                new List<int>[] {
                    new List<int> { 1, 0, 0, 0 },
                    new List<int> { 2, 2, 2, 0 },
                    new List<int> { 3, 3, 0, 0 },
                    new List<int> { 0, 4, 4, 4 },
                    new List<int> { 0, 0, 0, 5 },
                    new List<int> { 0, 0, 6, 6 },
                }
            };
            yield return new object[]
            {
                new List<IEnumerable<int>> { lists[0], lists[1] },
                new List<int>[] {
                    new List<int> { 1, 0 },
                    new List<int> { 2, 2 },
                    new List<int> { 3, 3 },
                    new List<int> { 0, 4 }
                }
            };
            yield return new object[]
            {
                new List<IEnumerable<int>> { lists[1], lists[0] },
                new List<int>[] {
                    new List<int> { 0, 1 },
                    new List<int> { 2, 2 },
                    new List<int> { 3, 3 },
                    new List<int> { 4, 0 }
                }
            };
            yield return new object[]
            {
                new List<IEnumerable<int>> { lists[0], lists[2] },
                new List<int>[] {
                    new List<int> { 1, 0 },
                    new List<int> { 2, 2 },
                    new List<int> { 3, 0 },
                    new List<int> { 0, 4 },
                    new List<int> { 0, 6 },
                }
            };
            yield return new object[]
            {
                new List<IEnumerable<int>> { lists[2], lists[0] },
                new List<int>[] {
                    new List<int> { 0, 1 },
                    new List<int> { 2, 2 },
                    new List<int> { 0, 3 },
                    new List<int> { 4, 0 },
                    new List<int> { 6, 0 },
                }
            };
            yield return new object[]
            {
                new List<IEnumerable<int>> { lists[0], lists[3] },
                new List<int>[] {
                    new List<int> { 1, 0 },
                    new List<int> { 2, 0 },
                    new List<int> { 3, 0 },
                    new List<int> { 0, 4 },
                    new List<int> { 0, 5 },
                    new List<int> { 0, 6 },
                }
            };
            yield return new object[]
            {
                new List<IEnumerable<int>> { lists[3], lists[0] },
                new List<int>[] {
                    new List<int> { 0, 1 },
                    new List<int> { 0, 2 },
                    new List<int> { 0, 3 },
                    new List<int> { 4, 0 },
                    new List<int> { 5, 0 },
                    new List<int> { 6, 0 },
                }
            };
            yield return new object[]
            {
                new List<IEnumerable<int>> { lists[0], Enumerable.Empty<int>().ToList() },
                new List<int>[] {
                    new List<int> { 1, 0 },
                    new List<int> { 2, 0 },
                    new List<int> { 3, 0 }
                }
            };
            yield return new object[]
            {
                new List<IEnumerable<int>> { lists[0], null },
                new List<int>[] {
                    new List<int> { 1, 0 },
                    new List<int> { 2, 0 },
                    new List<int> { 3, 0 }
                }
            };
            yield return new object[]
            {
                null,
                new List<int>[0]
            };
        }

        [TestMethod]
        [DynamicData(nameof(GetLists), DynamicDataSourceType.Method)]
        public void ZipManyOrdered_CorrectResultsReturned(IList<IEnumerable<int>> input, List<int>[] expectedValues)
        {
            // Arrange
            var expectedKeys = expectedValues.Select(x => x.First(y => y != 0)).ToList();

            // Act
            var actual = input.ZipManyOrdered(x => x, (key, values) => (key, values)).ToList();

            // Assert
            CollectionAssert
            .AreEqual(
                expectedValues.SelectMany(x => x).ToList(),
                actual.SelectMany(x => x.values).ToList());

            CollectionAssert.AreEqual(expectedKeys, actual.Select(x => x.key).ToList());
        }
    }
}
