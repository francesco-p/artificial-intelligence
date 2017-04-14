using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningTrees
{
    interface Node
    {
        string ToString(string indent);
        float CheckClassification(int example);
    }
}
