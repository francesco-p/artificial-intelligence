using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningTrees
{
    class NumericalAttribute : Attribute
    {
        public int Index { get; private set; }
        public int ClassificationIdx { get; private set; }
        public string Name { get; private set; }
        public float NullValue { get; private set; }
        public bool IsOn { get; set; }

        private Dictionary<float, List<int>> _dictionaryValueIdxs;
        public List<int> NullValueList { get; private set; }
        public List<int> PreList { get; private set; }
        public float Splitvalue { get; private set; }
        public List<int> PostList { get; private set; }


    /* Dependencies
    * - classification index
    * - Dataset
    * - Nullvalue 
    */
    public NumericalAttribute(string name, int index = -1)
        {
            Name = name;
            Index = index;

            ClassificationIdx = 13;
            NullValue = -9;

            Splitvalue = -9;
            IsOn = true;

        }


        // Populates the dictionary structure to search among the examples
        public void PopulateDictionary(List<int> examples)
        {
            _dictionaryValueIdxs = new Dictionary<float, List<int>>();

            NullValueList = new List<int>();

            List<int> temp;
            float currentValue;

            foreach (int i in examples)
            {
                currentValue = Parser.TRAININGDATA[i, Index];

                if (currentValue == NullValue)
                    NullValueList.Add(i);
                else
                {
                    if (_dictionaryValueIdxs.TryGetValue(currentValue, out temp))
                        temp.Add(i);
                    else
                        _dictionaryValueIdxs.Add(currentValue, new List<int>() { i });
                }

            }
        }


        public float CalculateGain(List<int> examples)
        {
            if (examples.Count == 1)
                IsOn = false;

            PopulateDictionary(examples);
            float splitEntropy = FindOptimalSplit(examples);

            float h = entropy(examples);

            float ratio1, h1;

            ratio1 = (float)NullValueList.Count / (float)examples.Count;
            h1 = entropy(NullValueList);

            float n = (ratio1 * h1);

            float ret = h - (n + splitEntropy);

            return ret ;
        }

        private float FindOptimalSplit(List<int> examples)
        {
            List<float> orderedValues = _dictionaryValueIdxs.Keys.ToList();
            orderedValues.Sort();

            List<int> pre = new List<int>();
            List<int> pst = new List<int>();

            float val,ratio1, ratio2,h1,h2,tmp;
            float min = 100;

            foreach (float value in orderedValues)
                if(value != NullValue)
                    pst.AddRange(_dictionaryValueIdxs[value]);

            for(int i = 0; i < orderedValues.Count-1; i++)
            {
                val = orderedValues[i];

                pre.AddRange(_dictionaryValueIdxs[val]);
                ratio1 = (float)pre.Count / (float)examples.Count;
                h1 = entropy(pre);

                pst = pst.Except(pre).ToList();
                ratio2 = (float)pst.Count / (float)examples.Count;
                h2 = entropy(pst);

                tmp = (ratio1 * h1) + (ratio2 * h2);

                if (tmp < min)
                {
                    min = tmp;
                    Splitvalue = val;
                    PreList = CloneList(pre);
                    PostList = CloneList(pst);
                }

            }
            return min;
        }

        private float entropy(List<int> S)
        {
            if (S.Count == 0)
                return 0;

            float tot = S.Count;
            float pos = positives(S);
            float neg = negatives(S);

            if (neg == 0 || pos == 0)
                return 0;

            float f1 = -(neg / tot);
            float log1 = (float)Math.Log((neg / tot), 2);
            float f2 = -(pos / tot);
            float log2 = (float)Math.Log((pos / tot), 2);

            float ris = (f1 * log1) + (f2 * log2);

            return ris;
        }

        private int positives(List<int> S)
        {
            int pos = 0;

            foreach (int i in S)
                if (Parser.TRAININGDATA[i, ClassificationIdx] == 1)
                    pos++;

            return pos;
        }

        private int negatives(List<int> S)
        {
            int neg = 0;

            foreach (int i in S)
                if (Parser.TRAININGDATA[i, ClassificationIdx] == 0)
                    neg++;

            return neg;
        }


        private List<int> CloneList(List<int> l)
        {
            List<int> aux = new List<int>();
            foreach (int i in l)
                aux.Add(i);

            return aux;
        }

        public override string ToString()
        {
            string s = string.Format("Name : {0}\n AttrIdx : {1}\n ", Name, Index);

            s += string.Format("SplitValue : {0}\n ", Splitvalue);


            return s;
        }
        


    }
}