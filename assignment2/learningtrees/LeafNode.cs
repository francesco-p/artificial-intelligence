using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningTrees
{
    class LeafNode : Node
    {

        public string Name { get; private set; }
        public float Classification { get; private set; }
        private int _classificationIndex;

        /* Dependencies:
        *
        * classificationIndex
        */
        public LeafNode(float classification = 1)
        {
            Classification = classification;
            if (Classification == 1)
                Name = "Yes";
            else
                Name = "No";

            _classificationIndex = 13;
        }

        public float CheckClassification(int example)
        {
            if (Parser.TESTDATA[example, _classificationIndex] == Classification)
                return 1;

            return 0;
        }

        public override string ToString()
        {
            return ToString("");
        }

        public string ToString(string indent)
        {
            return "[" + Name + "]\n";
        }

    }
}
