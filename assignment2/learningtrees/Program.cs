using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningTrees
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser parser;
            List<Attribute> attributes;
            float correctPredictions = 0;
            float currAccuracy = 0;
            List<int> training = null;
            List<int> test = null;
            ID3 alg = null;
            Node tree = null;
            int maxDepth = 0;
            List<float> foldAccuracy;
            List<float> depthAvgAccuracy = new List<float>();


            /*******************************************************************************************
            ************************10-FOLD XVALIDATION FOR 23 DIFFERENT DEPTHS*************************
            ********************************************************************************************/
            for (int depth = 23; depth > 0; depth--)
            {
                foldAccuracy = new List<float>();

                // Dependency effect: parser must be created before attributes
                parser = new Parser("crossvalidationset.data");


                while (parser.NextSplit())
                {

                    attributes = new List<Attribute>()
                               {
                                   new NumericalAttribute("Age",0),
                                   new DiscreteAttribute ("Sex",1, new List<float> {-9,0,1}),
                                   new DiscreteAttribute ("Chest Pain",2, new List<float> { -9,1,2,3,4 } ),
                                   new NumericalAttribute ("trestBPS", 3),
                                   new NumericalAttribute ("Chol",4),
                                   new DiscreteAttribute ("FBS", 5, new List<float> { -9, 0,1} ),
                                   new DiscreteAttribute ("restECG", 6, new List<float> { -9, 0,1,2 } ),
                                   new NumericalAttribute("Talalch",7),
                                   new DiscreteAttribute ("Exang", 8, new List<float> { -9, 0,1}),
                                   new NumericalAttribute ("Oldpeak",9),
                                   new DiscreteAttribute ("Slope", 10, new List<float> { -9, 0,1,2,3} ),
                                   new DiscreteAttribute ("Ca", 11, new List<float> { -9, 0, 1, 2, 3} ),
                                   new DiscreteAttribute ("Thal", 12, new List<float> { -9, 3,6,7} )
                               };

                    training = Enumerable.Range(0, Parser.TRAININGDATA.GetLength(0)).ToList();
                    alg = new ID3(Parser.TRAININGDATA, depth);
                    tree = alg.DecisionTreeLearning(training, attributes, new LeafNode(1), 0);

                    test = Enumerable.Range(0, Parser.TESTDATA.GetLength(0)).ToList();
                    foreach (int i in test)
                        correctPredictions += tree.CheckClassification(i);

                    currAccuracy = correctPredictions / Parser.TESTDATA.GetLength(0);
                    Console.WriteLine("Accuracy : {0}/{1} = {2} @ depth {3} ", correctPredictions, Parser.TESTDATA.GetLength(0), currAccuracy, depth);
                    foldAccuracy.Add(currAccuracy);

                    // Remove refs for GC
                    correctPredictions = 0;
                    training = null;
                    test = null;
                    alg = null;
                    tree = null;
                    attributes = null;
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Average Accuracy: {0} @ depth {1}", foldAccuracy.Average(), depth);
                Console.ResetColor();

                depthAvgAccuracy.Add(foldAccuracy.Average());
                foldAccuracy = null;

                parser = null;
            }


            
            /*******************************************************************************************
            **************************************TRAINING+TEST*****************************************
            ********************************************************************************************/

            Console.ForegroundColor = ConsoleColor.Green;

            maxDepth = 23 - depthAvgAccuracy.IndexOf(depthAvgAccuracy.Max());

            parser = new Parser();
            parser.setTrainingData("crossvalidationset.data");

            attributes = new List<Attribute>()
                    {
                        new NumericalAttribute("Age",0),
                        new DiscreteAttribute ("Sex",1, new List<float> {-9,0,1}),
                        new DiscreteAttribute ("Chest Pain",2, new List<float> { -9,1,2,3,4 } ),
                        new NumericalAttribute ("trestBPS", 3),
                        new NumericalAttribute ("Chol",4),
                        new DiscreteAttribute ("FBS", 5, new List<float> { -9, 0,1} ),
                        new DiscreteAttribute ("restECG", 6, new List<float> { -9, 0,1,2 } ),
                        new NumericalAttribute("Talalch",7),
                        new DiscreteAttribute ("Exang", 8, new List<float> { -9, 0,1}),
                        new NumericalAttribute ("Oldpeak",9),
                        new DiscreteAttribute ("Slope", 10, new List<float> { -9, 0,1,2,3} ),
                        new DiscreteAttribute ("Ca", 11, new List<float> { -9, 0, 1, 2, 3} ),
                        new DiscreteAttribute ("Thal", 12, new List<float> { -9, 3,6,7} )
                    };

            training = Enumerable.Range(0, Parser.TRAININGDATA.GetLength(0)).ToList();
            alg = new ID3(Parser.TRAININGDATA, maxDepth);
            tree = alg.DecisionTreeLearning(training, attributes, new LeafNode(1), 0);
            Console.WriteLine(tree);

            parser.setTestData("testset.data");
            test = Enumerable.Range(0, Parser.TESTDATA.GetLength(0)).ToList();

            foreach (int i in test)
                correctPredictions += tree.CheckClassification(i);

            currAccuracy = correctPredictions / Parser.TESTDATA.GetLength(0);

            Console.WriteLine("Accuracy : {0}/{1} = {2} @ depth {3} ", correctPredictions, Parser.TESTDATA.GetLength(0), currAccuracy, maxDepth);
            Console.ResetColor();

            Console.ReadLine();

        }



    }
}
