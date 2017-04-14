using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace LearningTrees
{
    class Parser
    {
        private int _currentSplit;
        private int _splitStep;

        private string _filename;

        public static float[,] DATA;
        public static float[,] TRAININGDATA;
        public static float[,] TESTDATA;

        public Parser() { }

        public Parser(string filename)
        {
            _filename = filename;
            fillStructure();
        }

        // Raw data parser, it creates a matrix with corrispective values
        private void fillStructure()
        {
            // CRLF termination lines
            string[] lines = File.ReadAllText(_filename).Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            int i = lines.Length;
            int j = (lines[0].Split(',')).Length;
            DATA = new float[i, j];

            // Xval
            _splitStep = (i%2 ==0)? i/10 : (i / 10)+1;
            TESTDATA = new float[_splitStep, j];
            TRAININGDATA = new float[i - _splitStep, j];


            i = -1;

            foreach (string line in lines)
            {
                i++;
                j = -1;

                foreach (string value in line.Split(','))
                {
                    j++;
                    float.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out DATA[i, j]);
                }
            }

            int lastColumn = DATA.GetLength(1) - 1;

            // Normalization of categorization
            for (i = 0; i < DATA.GetLength(0); i++)
                DATA[i, lastColumn] = DATA[i, lastColumn] > 0 ? 1 : 0;



        }

        public void setTestData(string filename)
        {
            // CRLF termination lines
            string[] lines = File.ReadAllText(filename).Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            int i = lines.Length;
            int j = (lines[0].Split(',')).Length;
            TESTDATA = new float[i, j];

            i = -1;
            foreach (string line in lines)
            {
                i++;
                j = -1;

                foreach (string value in line.Split(','))
                {
                    j++;
                    float.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out TESTDATA[i, j]);
                }
            }

            int lastColumn = TESTDATA.GetLength(1) - 1;

            // Normalization of categorization
            for (i = 0; i < TESTDATA.GetLength(0); i++)
                TESTDATA[i, lastColumn] = TESTDATA[i, lastColumn] > 0 ? 1 : 0;

        }

        public void setTrainingData(string filename)
        {
            // CRLF termination lines
            string[] lines = File.ReadAllText(filename).Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            int i = lines.Length;
            int j = (lines[0].Split(',')).Length;
            TRAININGDATA = new float[i, j];

            i = -1;
            foreach (string line in lines)
            {
                i++;
                j = -1;

                foreach (string value in line.Split(','))
                {
                    j++;
                    float.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out TRAININGDATA[i, j]);
                }
            }

            int lastColumn = TRAININGDATA.GetLength(1) - 1;

            // Normalization of categorization
            for (i = 0; i < TRAININGDATA.GetLength(0); i++)
                TRAININGDATA[i, lastColumn] = TRAININGDATA[i, lastColumn] > 0 ? 1 : 0;

        }

        public bool NextSplit()
        {

            int height = DATA.GetLength(0);
            int width = DATA.GetLength(1);

            if (_currentSplit == height)
                return false;

            int i, j;

            // First split
            if (_currentSplit < _splitStep)
            {
                for (i = 0; i < _splitStep; i++)
                    for (j = 0; j < width; j++)
                            TESTDATA[i,j]= DATA[i,j];

                for (i = 0; i < height-_splitStep; i++)
                    for (j = 0; j < width; j++)
                        TRAININGDATA[i, j] = DATA[i+_splitStep, j];

                _currentSplit += _splitStep;
            }
            // This is not the first split we do
            else
            {
                int nextIndex = _currentSplit +  _splitStep;

                // We are overflowing
                if (nextIndex > height)
                {
                    TRAININGDATA = new float[_currentSplit, width];

                    for (i = 0; i < _currentSplit; i++)
                        for (j = 0; j < width; j++)
                            TRAININGDATA[i, j] = DATA[i, j];

                    int residualHeight = height - _currentSplit;
                    TESTDATA = new float[residualHeight, width];

                    for (i = 0 ; i < residualHeight; i++)
                        for (j = 0; j < width; j++)
                            TESTDATA[i, j] = DATA[i+ _currentSplit, j];

                    _currentSplit = height;

                }
                else
                {

                    for (i = 0; i < _splitStep; i++)
                        for (j = 0; j < width; j++)
                            TESTDATA[i, j] = DATA[i+_currentSplit, j];


                    int k = -1;
                    for (i = 0; i < height; i++)
                    {
                        if (i >= _currentSplit && i < nextIndex)
                            continue;
                        else
                        {
                            k++;
                            for (j = 0; j < width; j++)
                                TRAININGDATA[k, j] = DATA[i, j];
                        }

                    }

                    _currentSplit = nextIndex;
                }

            }
            return true;

        }


    }
}
