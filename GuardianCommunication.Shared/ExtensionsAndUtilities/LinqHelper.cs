using System;
using System.Collections.Generic;

namespace GuardianCommunication.Shared.ExtensionsAndUtilities
{

    public static class LinqHelper
    {
        public static IEnumerable<List<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (batchSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(batchSize), "batchSize باید بزرگتر از صفر باشد");

            var batch = new List<T>(batchSize);

            foreach (var item in source)
            {
                batch.Add(item);
                if (batch.Count != batchSize)
                    continue;
                yield return batch;
                batch = new List<T>(batchSize);
            }
            if (batch.Count > 0)
                yield return batch;
        }
    }

}
