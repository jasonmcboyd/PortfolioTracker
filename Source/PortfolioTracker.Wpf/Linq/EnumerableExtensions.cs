using SimpleInjector.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PortfolioTracker.Wpf.Linq
{
    public static class EnumerableExtensions
    {
        public static async Task<IList<Task<TSource>>> WhenAll<TSource>(this IEnumerable<Task<TSource>> source)
        {
            var tasks = source.ToList();

            await Task.WhenAll(tasks);

            return tasks;
        }
        
        public static IEnumerable<TResult> ZipMany<TEnumerable, TSource, TResult>(
            this IList<TEnumerable> source,
            Func<TSource[], TResult> selector)
        where TEnumerable : IEnumerable<TSource>
        {
            List<IEnumerator<TSource>> enumerators = new List<IEnumerator<TSource>>(source.Count);
            try
            {
                for (int i = 0; i < source.Count; i++)
                {
                    enumerators.Add(source[i].GetEnumerator());
                }

                while (true)
                {

                    var values = new TSource[enumerators.Count];
                    var moveNextCount = 0;

                    for (int i = 0; i < enumerators.Count; i++)
                    {
                        moveNextCount += enumerators[i].MoveNext() ? 1 : 0;
                        values[i] = enumerators[i].Current;
                    }

                    if (moveNextCount == 0)
                    {
                        // We've exhausted all the enumerators.
                        yield break;
                    }
                    if (moveNextCount != enumerators.Count)
                    {
                        // The enumerators were not the same length.
                        throw new InvalidOperationException("Enumerables must be the same length.");
                    }

                    yield return selector(values); ;
                }
            }
            finally
            {
                enumerators?.ForEach(x => x?.Dispose());
            }
        }

        public static IEnumerable<TResult> ZipManyOrdered<TSource, TKey, TResult>(
            this IList<IEnumerable<TSource>> source,
            Func<TSource, TKey> keySelector,
            Func<TKey, IList<TSource>, TResult> selector)
            where TKey : IComparable<TKey>
        {
            if (source == null || source.Count == 0)
            {
                yield break;
            }

            IEnumerator<TSource>[] enumerators = new IEnumerator<TSource>[source.Count];
            try
            {
                for (int i = 0; i < source.Count; i++)
                {
                    enumerators[i] = source[i]?.GetEnumerator() ?? Enumerable.Empty<TSource>().GetEnumerator();
                }

                List<TSource> result = null;
                var greaterKeys = new bool[enumerators.Length];
                var stack = new Stack<List<TSource>>();
                
                var minSet = false;
                var min = default(TKey);

                while (true)
                {
                    if (stack.Count == 0)
                    {
                        result = new List<TSource>(enumerators.Length);
                        min = default;
                        minSet = false;
                    }
                    else
                    {
                        result = stack.Pop();
                        min = keySelector(result.Last());
                        minSet = true;
                    }
                    
                    for (int i = result.Count; i < enumerators.Length; i++)
                    {
                        bool movedNext = false;
                        if (greaterKeys[i])
                        {
                            movedNext = true;
                            greaterKeys[i] = false;
                        }
                        else
                        {
                            movedNext = enumerators[i].MoveNext();
                        }

                        if (movedNext)
                        {
                            var key = keySelector(enumerators[i].Current);
                            if (minSet)
                            {
                                var comparison = key.CompareTo(min);
                                if (comparison < 0)
                                {
                                    stack.Push(result);
                                    result = new List<TSource>(enumerators.Length);
                                    stack.Peek().ForEach(x => result.Add(default));
                                    result.Add(enumerators[i].Current);
                                    min = key;
                                }
                                else if (comparison == 0)
                                {
                                    result.Add(enumerators[i].Current);
                                }
                                else
                                {
                                    greaterKeys[i] = true;
                                    result.Add(default);
                                }
                            }
                            else
                            {
                                result.Add(enumerators[i].Current);
                                minSet = true;
                                min = key;
                            }
                        }
                        else
                        {
                            result.Add(default);
                        }
                    }

                    if (minSet)
                    {
                        yield return selector(min, result);
                    }
                    else
                    {
                        yield break;
                    }
                }


            }
            finally
            {
                enumerators?.ForEach(x => x?.Dispose());
            }
        }

        public static IEnumerable<TResult> ZipOrdered<TLeft, TRight, TKey, TResult>(
            this IEnumerable<TLeft> left,
            IEnumerable<TRight> right,
            Func<TLeft, TKey> leftKeySelector,
            Func<TRight, TKey> rightKeySelector,
            Func<TLeft, TRight, TResult> resultSelector)
            where TKey : IComparable<TKey>
        {
            using (var leftEnumerator = left.GetEnumerator())
            using (var rightEnumerator = right.GetEnumerator())
            {
                var l = leftEnumerator.MoveNext();
                var r = rightEnumerator.MoveNext();

                while (true)
                {
                    if (l && r)
                    {
                        var leftKey = leftKeySelector(leftEnumerator.Current);
                        var rightKey = rightKeySelector(rightEnumerator.Current);

                        var comparison = leftKey.CompareTo(rightKey);

                        var leftResult = default(TLeft);
                        var rightResult = default(TRight);

                        if (comparison <= 0)
                        {
                            leftResult = leftEnumerator.Current;
                            l = leftEnumerator.MoveNext();
                        }
                        
                        if (comparison >= 0)
                        {
                            rightResult = rightEnumerator.Current;
                            r = rightEnumerator.MoveNext();
                        }

                        yield return resultSelector(leftResult, rightResult);
                    }   
                    else if (l)
                    {
                        yield return resultSelector(leftEnumerator.Current, default);
                        l = leftEnumerator.MoveNext();
                    }
                    else if(r)
                    {
                        yield return resultSelector(default, rightEnumerator.Current);
                        r = rightEnumerator.MoveNext();
                    }
                    else
                    {
                        yield break;
                    }
                }
            }
        }
    }
}
