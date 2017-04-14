using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningTrees
{
    class DecisionNode : Node
    {
        private Attribute _attribute;
        private float _splitValue;
        private Node _preChild;
        private Node _postChild;
        private float _nullValue;
        private Node _nullChild = null;
        private Dictionary<float, Node> _childs;


        // Constructor
        public DecisionNode(Attribute attribute)
        {
            _childs = new Dictionary<float, Node>();
            _nullValue = -9;
            _attribute = attribute;
        }


        public void AddNullBranch(Node child)
        {
            _nullChild = child;
        }

        public void AddPreBranch(float value, Node child)
        {
            _splitValue = value;
            _preChild = child;
        }

        public void AddPostBranch(Node child)
        {
            _postChild = child;
        }

        public void AddBranch(float value, Node child)
        {
            _childs.Add(value, child);
        }


        public float CheckClassification(int example)
        {
            float exampleValue = Parser.TESTDATA[example, _attribute.Index];

            if (_attribute is NumericalAttribute)
            {
                if (exampleValue == _nullValue)
                {
                    return _nullChild.CheckClassification(example);
                }
                else
                {
                    if (exampleValue > _splitValue)
                        return _postChild.CheckClassification(example);
                    else
                        return _preChild.CheckClassification(example);
                }
            }
            else
                return _childs[exampleValue].CheckClassification(example);
        }
      

        public override string ToString()
        {
            return ToString("\t\t");
        }

        public string ToString(string indent)
        {
            string str = "("+ _attribute.Name + ")\n";

            if(_attribute is NumericalAttribute)
            {
                if (_nullChild != null)
                    str += indent + "--?--" + _nullChild.ToString(indent+"|\t\t");

                str += indent+ "\\-- <" + _splitValue + "-- " + _preChild.ToString(indent+"|\t\t");
                str += indent + "\\-- >" + _splitValue + "-- " + _postChild.ToString(indent + "\t\t");
            }
            else
            {
                int count = 0;
                foreach (KeyValuePair<float, Node> child in _childs)
                {
                    count++;
                    if(count != _childs.Count)
                    {
                        if (child.Key == -9)
                            str += indent + "\\--?-- " + child.Value.ToString(indent + "|\t\t");
                        else
                            str += indent + "\\--" + child.Key + "-- " + child.Value.ToString(indent + "|\t\t");
                    }
                    else
                    {
                        if (child.Key == -9)
                            str += indent + "\\--?-- " + child.Value.ToString(indent + "\t\t");
                        else
                            str += indent + "\\--" + child.Key + "-- " + child.Value.ToString(indent + "\t\t");
                    }

                }
            }

            return str;
        }
        
    }
}
