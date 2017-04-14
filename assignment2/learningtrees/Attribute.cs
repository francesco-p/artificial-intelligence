using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningTrees
{
    interface Attribute
    {
        string Name { get; }
        bool IsOn { get; set; }
        int Index { get; }
        float CalculateGain(List<int> examples);
        void PopulateDictionary(List<int> examples);
        
    }
}
