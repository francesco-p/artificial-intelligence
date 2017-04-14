using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningTrees
{
    class DiscreteAttribute : Attribute
    {
       
        public int Index { get; private set; }
        public int ClassificationIdx { get; set; }
        public List<float> Values { get; private set; } 
        public string Name { get; private set; }
        public bool IsOn { get; set; }

        private Dictionary<float, List<int>> _dictionaryValueIdxs;

        /* Dependencies
        * - classification index
        * - Dataset
        */
        public DiscreteAttribute(string name, int index = -1 , List<float> values = null )
        {
            Name = name;
            Index = index;
            Values = values;

            ClassificationIdx = 13;
            IsOn = true;
        }

        // Populates the dictionary structure to search among the examples
        public void PopulateDictionary(List<int> examples)
        {
            _dictionaryValueIdxs = new Dictionary<float, List<int>>();

            foreach (float value in Values)
            {
                _dictionaryValueIdxs.Add(value, new List<int>());

                foreach (int i in examples)
                    if (Parser.TRAININGDATA[i, Index] == value)
                        _dictionaryValueIdxs[value].Add(i);
            }
        }

        public List<int> ExamplesOf(float value)
        {
            List<int> aux = new List<int>();

            foreach (int i in _dictionaryValueIdxs[value])
                aux.Add(i);

            return aux;
        }

        public float CalculateGain(List<int> examples)
        {
            PopulateDictionary(examples);
                   
            float h = entropy(examples);
            float sum = 0;
            float aux, aux2;

            foreach (float value in Values)
            {
                aux = ((float)_dictionaryValueIdxs[value].Count / (float)examples.Count);
                aux2 = entropy(_dictionaryValueIdxs[value]);
                aux = aux * aux2;
                sum += aux;
            }
            return h - sum;
        }

        private float entropy(List<int> examples)
        {
            float tot = examples.Count;
            float pos = positives(examples);
            float neg = negatives(examples);

            if (neg == 0 || pos == 0)
                return 0;

            float f1 = -(neg / tot);
            float log1 = (float)Math.Log((neg / tot), 2);
            float f2 = -(pos / tot);
            float log2 = (float)Math.Log((pos / tot), 2);
            float ris = (f1 * log1) + (f2 * log2);

            return ris;
        }

        private int positives(List<int> examples)
        {
            int pos = 0;

            foreach (int i in examples)
                if (Parser.TRAININGDATA[i, ClassificationIdx] == 1)
                    pos++;

            return pos;
        }

        private int negatives(List<int> examples)
        {
            int neg = 0;

            foreach (int i in examples)
                if (Parser.TRAININGDATA[i, ClassificationIdx] == 0)
                    neg++;

            return neg;
        }

        public override string ToString()
        {
            string s = string.Format("Name : {0}\n AttrIdx : {1}\n ", Name, Index);
            s += "Values: [ ";
            foreach (float v in Values)
                s += string.Format("{0}, ", v);
            s += string.Format("]\n ");

            return s;
        }
    }
}