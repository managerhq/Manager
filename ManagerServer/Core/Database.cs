using ManagerServer.Model;
using ManagerServer.Query.GeneralLedger;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace ManagerServer
{
    public sealed class Database : IProgress<Tuple<int, int>>
    {
        private TypePartitionedDictionary<Guid, Model.Object> objects;
        private ImmutableDictionary<Guid, Model.Object> prototypes;
        private ImmutableDictionary<Guid, long> images;
        private ImmutableDictionary<Type, Model.Object[]> objectsByType = ImmutableDictionary<Type, Model.Object[]>.Empty;
        private GeneralLedgerTransactions generalLedgerTransactions;

        //public SemaphoreSlim SemaphoreSlim { get; init; } = new SemaphoreSlim(1, 1);
        public DatabaseStatus Status { get; private set; }
        public bool HasNonCriticalError { get; private set; }        
        public long Priority { get; set; }

        public int ProgressPosition { get; private set; }
        public int ProgressLength { get; private set; }

        public void SetInvalid() => Status = DatabaseStatus.Invalid;
        public void SetIncompatible() => Status = DatabaseStatus.Incompatible;
        public void SetCorrupted() => Status = DatabaseStatus.Corrupted;

        public void SetOutOfMemory()
        {
            Status = DatabaseStatus.OutOfMemory;
            objects = null;
            prototypes = null;
            images = null;
            objectsByType = null;
            generalLedgerTransactions = null;
        }

        public void SetProgress(int position, int length)
        {
            ProgressPosition = position;
            ProgressLength = length;
            Status = DatabaseStatus.Progress;
        }

        public void Report(Tuple<int, int> value)
        {
            SetProgress(value.Item1, value.Item2);
        }

        public void SetData(Dictionary<Guid, Model.Object> prototypes, TypePartitionedDictionary<Guid, Model.Object> objects, Dictionary<Guid, long> images, bool hasNonCriticalError)
        {
            this.prototypes = prototypes.ToImmutableDictionary();
            this.objects = objects;
            this.images = images.ToImmutableDictionary();
            HasNonCriticalError = hasNonCriticalError;
        }

        public void Precompute()
        {
            OfType<InventoryUnitCost>(); // we pre-cache this because it can be used by CreateGeneralLedgerTransactions
            OfType<ExchangeRate>(); // we pre-cache this because it can be used by CreateGeneralLedgerTransactions

            var data = objects.OfType<Transaction>()
                .AsParallel()
                .Select(x => new KeyValuePair<Guid, GeneralLedgerTransactionContainer>(x.Key, new GeneralLedgerTransactionContainer(x.CreateGeneralLedgerTransactions(this))))
                .ToDictionary();

            this.generalLedgerTransactions = new GeneralLedgerTransactions(this, data);            
        }

        public void SetReady()
        {
            Status = DatabaseStatus.Ready;
        }

        public GeneralLedgerTransactions GetGeneralLedgerTransactions()
        {
            return generalLedgerTransactions;
        }        

        public bool Exists<T>()
        {
            var key = ManagerServer.Model.Object.GetGuidByType(typeof(T));
            return prototypes.ContainsKey(key);
        }

        public T SingleOrDefault<T>(Guid? key) where T : Model.Object
        {
            if (!key.HasValue) return null;
            objects.TryGetValue(key.Value, out var value);
            return value as T;
        }

        private ImmutableDictionary<Guid, ManagerServer.Model.Object> singles = ImmutableDictionary<Guid, Model.Object>.Empty;
        public T Single<T>() where T : Model.Object, new()
        {
            var key = ManagerServer.Model.Object.GetGuidByType(typeof(T));

            var value = prototypes.GetValueOrDefault(key);
            if (value is T) return value as T;

            var single = singles.GetValueOrDefault(key);
            if (single != null)
            {
                return single as T;
            }
            else
            {
                var o2 = new T() { Key = key };
                singles = singles.SetItem(key, o2);
                return o2;
            }
        }

        public Model.Object Single(Guid key)
        {
            var type = ManagerServer.Model.Object.GetTypeByGuid(key);
            if (type == null) return null;

            var value = prototypes.GetValueOrDefault(key);
            if (value != null) return value;

            var single = singles?.GetValueOrDefault(key);
            if (single != null)
            {
                return single;
            }
            else
            {
                var o2 = (Model.Object)Activator.CreateInstance(type);
                o2.Key = key;
                singles = singles.SetItem(key, o2);
                return o2;
            }
        }

        public Model.Object SingleOrDefault(Guid key)
        {
            objects.TryGetValue(key, out var value);
            return value;
        }

        public InventoryUnitCost FindInventoryUnitCost(Guid inventoryItem, DateTime date)
        {
            var inventoryUnitCosts = OfType<InventoryUnitCost>();
            var index = Array.BinarySearch(inventoryUnitCosts, new InventoryUnitCost() { InventoryItem = inventoryItem, Date = date });
            if (index >= 0)
            {
                return inventoryUnitCosts[index];
            }

            var insertPoint = ~index;
            if (insertPoint == 0) return null;
            var inventoryUnitCost = inventoryUnitCosts[insertPoint-1];
            if (inventoryUnitCost.InventoryItem == inventoryItem) return inventoryUnitCost;

            return null;
        }

        public ExchangeRate FindExchangeRate(Guid foreignCurrency, DateTime date)
        {
            var exchangeRates = OfType<ExchangeRate>();
            var index = Array.BinarySearch(exchangeRates, new ExchangeRate() { Currency = foreignCurrency, Date = date });
            if (index >= 0)
            {
                return exchangeRates[index];
            }

            var insertPoint = ~index;
            if (insertPoint == 0) return null;
            var exchangeRate = exchangeRates[insertPoint - 1];
            if (exchangeRate.Currency == foreignCurrency) return exchangeRate;

            return null;
        }

        internal void RemoveImage(Guid key)
        {
            ImmutableInterlocked.Update(ref images, x => images.Remove(key));
        }

        public long? GetImage(Guid key)
        {
            if (images.TryGetValue(key, out long value)) return value;
            return null;
        }

        internal void SetImage(Guid key)
        {
            ImmutableInterlocked.Update(ref images, x => images.SetItem(key, DateTime.UtcNow.Ticks));
        }

        public int GetCount<T>()
        {
            return objects.GetCount<T>();
        }

        public bool Any<T>()
        {
            return GetCount<T>() > 0;
        }

        public T[] OfType<T>() where T : Model.Object, new()
        {
            if (objectsByType.TryGetValue(typeof(T), out var output))
            {
                return (T[])output;
            }
            else
            {
                var items = objects.OfType<T>();
                if (typeof(T).IsAssignableTo(typeof(IComparable<T>))) output = items.Order().ToArray();
                else output = items.ToArray();
                objectsByType = objectsByType.SetItem(typeof(T), output);

#if DEBUG
                Console.WriteLine($"Sorted {typeof(T).Name} ({output.Length})");
#endif


                return (T[])output;
            }
        }

        public IEnumerable<T> UnorderedOfType<T>() where T : Model.Object
        {
            return objects.OfType<T>();
        }

        public IEnumerable<Model.Object> UnorderedOfType(Type type)
        {
            return objects.OfType(type);
        }

        public ManagerServer.Model.ICode SingleOrDefaultByCode(string code, Type allowedType)
        {
            if (string.IsNullOrWhiteSpace(code)) return null;
            return objects.OfType(allowedType).OfType<ICode>().FirstOrDefault(x => x.Code == code);
        }

        public ManagerServer.Model.ICustomField[] GetCustomFields(Type t)
        {
            var list = new List<ManagerServer.Model.ICustomField>();
            list.AddRange(UnorderedOfType<ManagerServer.Model.DateCustomField>().Cast<ManagerServer.Model.ICustomField>().Where(x => x.Contains(t)));
            list.AddRange(UnorderedOfType<ManagerServer.Model.TextCustomField>().Cast<ManagerServer.Model.ICustomField>().Where(x => x.Contains(t)));
            list.AddRange(UnorderedOfType<ManagerServer.Model.NumberCustomField>().Cast<ManagerServer.Model.ICustomField>().Where(x => x.Contains(t)));
            list.AddRange(UnorderedOfType<ManagerServer.Model.CheckboxCustomField>().Cast<ManagerServer.Model.ICustomField>().Where(x => x.Contains(t)));
            list.AddRange(UnorderedOfType<ManagerServer.Model.MultipleValueCustomField>().Cast<ManagerServer.Model.ICustomField>().Where(x => x.Contains(t)));
            list.AddRange(UnorderedOfType<ManagerServer.Model.ImageCustomField>().Cast<ManagerServer.Model.ICustomField>().Where(x => x.Contains(t)));
            return list.ToArray();
        }

        internal void ApplyChanges(Model.Object[] toAdd, Guid[] toRemove)
        {
            var invalidations = new List<Model.Object>();

            foreach (var e in toAdd)
            {
                var key = ManagerServer.Model.Object.GetGuidByType(e.GetType());
                if (key != e.Key)
                {
                    objects[e.Key] = e;
                }
                else
                {
                    prototypes = prototypes.SetItem(e.Key, e);
                }
                invalidations.Add(e);
            }

            foreach (var e in toRemove)
            {
                if (objects.TryGetValue(e, out Model.Object o))
                {
                    objects.Remove(e);
                    invalidations.Add(o);
                }
                if (prototypes.TryGetValue(e, out Model.Object o2))
                {
                    prototypes = prototypes.Remove(e);
                    invalidations.Add(o2);
                }
            }

            Invalidate(invalidations.ToArray());
        }

        public void Invalidate<T>()
        {
            if (objectsByType.ContainsKey(typeof(T)))
            {
                objectsByType = objectsByType.Remove(typeof(T));
            }
        }

        private void Invalidate(ManagerServer.Model.Object[] objects)
        {
            foreach (var e in objects.GroupBy(x => x.GetType()))
            {
                if (objectsByType.ContainsKey(e.Key))
                {
                    objectsByType = objectsByType.Remove(e.Key);
                }
            }

            if (generalLedgerTransactions != null)
            {
                var invalidations = new HashSet<Guid>();

                foreach (var e in objects)
                {
                    if (e is ManagerServer.Model.ExchangeRate exchangeRate)
                    {
                        if (exchangeRate.Currency.HasValue)
                        {
                            invalidations.Add(exchangeRate.Currency.Value);
                        }
                    }
                    else if (e is ManagerServer.Model.InventoryUnitCost inventoryUnitCost && inventoryUnitCost.InventoryItem.HasValue)
                    {
                        var previousInventoryUnitCost = inventoryUnitCost.Date > DateTime.MinValue ? FindInventoryUnitCost(inventoryUnitCost.InventoryItem.Value, inventoryUnitCost.Date.AddDays(-1)) : null;

                        if (previousInventoryUnitCost != null)
                        {
                            invalidations.Add(previousInventoryUnitCost.Key);
                        }
                        invalidations.Add(Single<InventoryUnitCost>().Key);
                        invalidations.Add(e.Key);
                    }
                    else
                    {
                        invalidations.Add(e.Key);
                    }
                }

                generalLedgerTransactions.Invalidate(invalidations.ToArray());
            }
        }

        public ManagerServer.Model.ReportTransformation2 GetReportTransformation2(Guid? key)
        {
            if (!key.HasValue) return null;

            var businessDetails = Single<ManagerServer.Model.BusinessDetails>();
            var countryObjects = ManagerServer.Localizations.Localizations.Get(businessDetails.Obsolete_Country);
            var reportTransformation = countryObjects.OfType<ManagerServer.Model.ReportTransformation2>().SingleOrDefault(x => x.Key == key);
            if (reportTransformation != null) return reportTransformation;

            return SingleOrDefault<ManagerServer.Model.ReportTransformation2>(key);
        }

        public enum DatabaseStatus : int
        {
            Fresh = 0,
            Progress = 2,
            Ready = 3,
            Corrupted = 4,
            Invalid = 5,
            Incompatible = 6,
            OutOfMemory = 7
        }
    }
}