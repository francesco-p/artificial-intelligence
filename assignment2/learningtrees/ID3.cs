using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningTrees
{
    class ID3
    {
        private int classificationIdx;
        private int _maxDepth;

        public ID3(float[,] d, int m)
        {
            classificationIdx = Parser.TRAININGDATA.GetLength(1) - 1;
            _maxDepth = m;
        }
       
        public Node DecisionTreeLearning(List<int> examples, List<Attribute> attributes, Node def, int depth)
        {        
            if (examples.Count == 0)
                return def;

            if (depth == _maxDepth)
                return majorityValue(examples);

            if (AllSameClassification(examples))
                if (Parser.TRAININGDATA[examples[0], classificationIdx] == 1)
                    return new LeafNode(1);
                else
                    return new LeafNode(0);

            bool allOff = true;
            foreach (Attribute a in attributes)
            {
                if (a.IsOn)
                {
                    allOff = false;
                    break;
                }
            }
            if (allOff)
                return majorityValue(examples);

            Attribute best = chooseAttribute(examples, attributes);

            DecisionNode tree = new DecisionNode(best);

            Node m = majorityValue(examples);

            if(best is DiscreteAttribute)
            {
                best.IsOn = false;

                foreach (float value in ((DiscreteAttribute)best).Values)
                {
                    List<int> examples1 = CloneList(((DiscreteAttribute)best).ExamplesOf(value));
                    Node subtree = DecisionTreeLearning(examples1, attributes, m, depth+1);
                    tree.AddBranch(value, subtree);
                }
            }
            else
            {
                List<int> nullList = nullList = ((NumericalAttribute)best).NullValueList;
                float valueSplit = valueSplit = ((NumericalAttribute)best).Splitvalue;
                List<int> preSplit = preSplit = CloneList(((NumericalAttribute)best).PreList);
                List<int> postSplit = postSplit = CloneList(((NumericalAttribute)best).PostList);

                Node subtree = DecisionTreeLearning(nullList, attributes, m, depth + 1);
                tree.AddNullBranch(subtree);

                Node subtree2 = DecisionTreeLearning(preSplit, attributes, m, depth + 1);
                tree.AddPreBranch(valueSplit, subtree2);

                Node subtree3 = DecisionTreeLearning(postSplit, attributes, m, depth + 1);
                tree.AddPostBranch(subtree3);
            }
            return tree;
        }

        private bool AllSameClassification(List<int> examples)
        {
            float prev = Parser.TRAININGDATA[examples[0], classificationIdx];

            foreach (int i in examples)
                if (Parser.TRAININGDATA[i, classificationIdx] != prev)
                    return false;

            return true;
        }

        private Attribute chooseAttribute(List<int> examples, List<Attribute> attributes)
        {
            float tmp = -1;
            float max = -1;
            Attribute maxAtr = null;

            foreach (Attribute a in attributes)
            {

                if (a.IsOn)
                    tmp = a.CalculateGain(examples);

                if (tmp > max)
                {
                    max = tmp;
                    maxAtr = a;
                }
            }
            return maxAtr;
        }

        private Node majorityValue(List<int> attributes)
        {
            int n = 0;
            int p = 0;

            foreach(int i in attributes)
                if (Parser.TRAININGDATA[i, classificationIdx] == 1)
                    p++;
                else
                    n++;

            return p > n ? new LeafNode(1) : new LeafNode(1);
        }


        private List<int> CloneList(List<int> l)
        {
            List<int> aux = new List<int>();
            foreach (int i in l)
                aux.Add(i);

            return aux;
        }

    }
}
