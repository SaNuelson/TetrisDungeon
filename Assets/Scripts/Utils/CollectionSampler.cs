using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Util
{
    public enum StrategyType
    {
        Random,
        Bag,
        MultiBag,
        History
    }
    public class CollectionSampler<T>
    {
        private Strategy picker;

        public CollectionSampler(IEnumerable<T> items, StrategyType strategy, int strategyParam)
        {
            var arr = items.ToArray();
            switch (strategy)
            {
                case StrategyType.Random:
                    picker = new PseudoRandomStrategy(arr);
                    break;
                case StrategyType.Bag:
                    picker = new BagStrategy(arr);
                    break;
                case StrategyType.MultiBag:
                    picker = new MultiBagStrategy(arr, strategyParam);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public T Next()
        {
            return picker.Get();
        }

        internal abstract class Strategy
        {
            protected T[] items;
            public abstract T Get();
        }

        internal class PseudoRandomStrategy : Strategy
        {
            public PseudoRandomStrategy(T[] items)
            {
                this.items = items;
            }

            public override T Get()
            {
                return items[UnityEngine.Random.Range(0, items.Length)];
            }
        }

        internal class BagStrategy : Strategy
        {
            private bool[] used;
            private int usedCounter = 0;
            public BagStrategy(T[] items)
            {
                this.items = items;
                used = new bool[items.Length];
            }

            public void Reset()
            {
                for (int i = 0; i < used.Length; i++)
                    used[i] = false;
                usedCounter = 0;
            }

            public bool IsEmpty => usedCounter == used.Length;

            public override T Get()
            {
                if (usedCounter == used.Length)
                    Reset();

                var rnd = UnityEngine.Random.Range(0, used.Length - usedCounter);
                for (int i = 0; i < used.Length; i++)
                {
                    if (!used[i])
                    {
                        if (rnd > 0)
                        {
                            rnd--;
                        }
                        else
                        {
                            used[i] = true;
                            usedCounter++;
                            return items[i];
                        }
                    }
                }
                throw new Exception("Internal error in bag strategy");
            }
        }

        internal class MultiBagStrategy : Strategy
        {
            private BagStrategy[] bags;
            private bool[] used;
            private int usedCounter = 0;

            public MultiBagStrategy(T[] items, int numOfBags)
            {
                this.items = items;
                bags = new BagStrategy[numOfBags];
                for (int i = 0; i < bags.Length; i++)
                    bags[i] = new BagStrategy(items);
            }

            public void Reset()
            {
                for (int i = 0; i < used.Length; i++)
                {
                    bags[i].Reset();
                    used[i] = false;
                }
                usedCounter = 0;
            }

            public override T Get()
            {
                if (usedCounter == used.Length)
                    Reset();

                var rnd = UnityEngine.Random.Range(0, used.Length - usedCounter);
                for (int i = 0; i < used.Length; i++)
                {
                    if (!used[i])
                    {
                        if (rnd > 0)
                        {
                            rnd--;
                        }
                        else
                        {
                            var item = bags[i].Get();
                            if (bags[i].IsEmpty)
                            {
                                used[i] = true;
                                usedCounter++;
                            }
                            return item;
                        }
                    }
                }
                throw new Exception("Internal error in bag strategy");
            }
        }
    }

}
