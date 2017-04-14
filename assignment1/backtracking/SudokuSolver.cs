using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class SudokuSolver
    {
        // Substitute the comma when printing double values....
        System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        

        private Cell[,] _sudoku;
   
        public SudokuSolver(int [,] rawSudoku)
        {
            _sudoku = new Cell[9, 9];

            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    _sudoku[i, j] = new Cell(i, j, rawSudoku[i, j]);

            //PUNTO AL POSTO DELLA VIRGOLA
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

        }

        public void SetSudoku(int[,] rawSudoku)
        {
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    _sudoku[i, j] = new Cell(i, j, rawSudoku[i, j]);

        }


        /*************************************************************************************/
        /***************************** SIMPLE BACKTRACKING SOLUTION **************************/
        /*************************************************************************************/

        //i have disabled the debug since it could take very long before printing everything
        public static int NCALL = 0;
        public bool Backtracking()
        {
            int RID = ++NCALL;

            for (int i=0;i<9;i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (_sudoku[i, j].Value == 0)
                    {
                        //ADVANCED DEBUG
                        //Console.ForegroundColor = ConsoleColor.Cyan;
                        //Console.WriteLine("[DEBUG {0}]> [{1},{2}] : FREE cell found!", RID, i, j);
                        //Console.ResetColor();

                        for (int k=1;k<10;k++)
                        {
                            _sudoku[i, j].Value = k;

                            //ADVANCED DEBUG
                            //Console.WriteLine("[DEBUG {0}]> [{1},{2}] : trying value {3}...", RID, i, j, k);

                            if (RespectAllConstrains(_sudoku[i, j]))
                            {
                                //DEBUG
                                //Console.ForegroundColor = ConsoleColor.Green;
                                //Console.WriteLine("[DEBUG {0}]> Value {1} worked @ [{2},{3}]! Recursion call... ", RID, k, i, j);
                                //Console.ResetColor();

                                bool ris = Backtracking();
                                if (ris)
                                    return ris;
                                else
                                    continue;
                            }
                        }
                        _sudoku[i, j].Value = 0;

                        //DEBUG
                        //Console.ForegroundColor = ConsoleColor.Red;
                        //Console.WriteLine("[DEBUG {0}]> Fail I tried all values @ [{1},{2}] rollback...", RID, i, j);
                        //Console.ResetColor();
                        return false; 
                    }
                }
            }
            return true; 
        }
 
        public bool RespectAllConstrains(Cell c)
        {
            return (ColumnConstrain(c) && RowConstrain(c) && BoxConstrain(c));
        }
        private bool ColumnConstrain(Cell c)
        {
            int occur = 0;
            for (int i = 0; i < 9; i++)
                if (_sudoku[i, c.Column].Value == c.Value)
                    occur++;

            if (occur > 1)
                return false;
            return true;
        }
        private bool RowConstrain(Cell c)
        {
            int occur = 0;
            for (int j = 0; j < 9; j++)
                if (_sudoku[c.Row, j].Value == c.Value)
                    occur++;

            if (occur > 1)
                return false;
            return true;
        }
        private bool BoxConstrain(Cell c)
        {
            int occur = 0;
            int rBox = ((int)(c.Row / 3)) * 3;
            int cBox = ((int)(c.Column / 3)) * 3;

            for (int i = rBox; i < rBox + 3; i++)
                for (int j = cBox; j < cBox + 3; j++)
                    if (_sudoku[i, j].Value == c.Value)
                        occur++;

            if (occur > 1)
                return false;
            return true;
        }


        /***************************** BACKTRACKING + MRV + FORWARD CHECKING = CONSTRAINT PROPAGATION SOLUTION *************************/

        //This is the Minimum Remaining Value list
        List<Cell> mrvList = new List<Cell>();

        //This method is called before the backtracking solution method to initialize the structures
        public void PreliminaryCheck()
        {
            //DEBUG
            NCALL = 0;

            Cell tmp;
            
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    tmp = _sudoku[i, j];
                    if (tmp.Value != 0)
                        restrictDomainsConnectedTo(tmp);
                    else
                        mrvList.Add(tmp);
                }
            }
            mrvList.Sort();
        }

        //Main backtracking with constraint propagation
        public bool BacktrackingConstr()
        {
            
            Cell candidate;

            if (mrvList.Count == 0)
                return true;
            else
                candidate = mrvList[0];

            //DEBUG
            int RID = ++NCALL;

            if (candidate.DomainCount() == 0)
            {
                //DEBUG
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[DEBUG {0}]> Wrong branch! DomainCount={3} @ [{1},{2}] ...", RID, candidate.Row, candidate.Column, candidate.DomainCount());
                Console.ResetColor();

                return false;

            }

            for(int i=0;i<9; i++)
            {
                if(candidate.Domain[i]==false)
                    continue;

                //DEBUG
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[DEBUG {0}]> Candidate @ [{1},{2}] trying value:{3}...", RID, candidate.Row, candidate.Column, i + 1);
                Console.ResetColor();

                mrvList.Remove(candidate);              
                candidate.Value = i+1;                    
                restrictDomainsConnectedTo(candidate);  
                mrvList.Sort();

                //ADVANCED DEBUG
                //printdomains();
                //Console.WriteLine(this);

                bool ris = BacktrackingConstr();

                if (ris)
                    return true;
                else
                {
                    //DEBUG
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("[DEBUG {0}]> Rollingback  @ [{1},{2}] i tried value:{3}...", RID, candidate.Row, candidate.Column, i + 1);
                    Console.ResetColor();

                    de_restrictDomainsConnectedTo(candidate);
                    candidate.Value = 0;
                    mrvList.Add(candidate);
                    mrvList.Sort();

                    //ADVANCED DEBUG (after derestrict)
                    //printdomains();           
                    //Console.WriteLine(this);

                }
            }

            return false;   
        }

        //Restrict all the domains of the cells that are connected to c
        private void restrictDomainsConnectedTo(Cell c)
        {
            if (c.Value == 0)
                return;

            restrictBoxDomains(c);
            restrictRowDomains(c);
            restrictColumnDomains(c);
        }
        private void restrictRowDomains(Cell c)
        {
            for (int i = 0; i < 9; i++)
                if (_sudoku[c.Row, i].Value == 0)
                    _sudoku[c.Row, i].Domain[c.Value-1] = false;
        }
        private void restrictColumnDomains(Cell c)
        {
            for (int i = 0; i < 9; i++)
                if (_sudoku[i, c.Column].Value == 0)
                    _sudoku[i, c.Column].Domain[c.Value-1] = false;
            
        }
        private void restrictBoxDomains(Cell c)
        {
            int rBox = ((int)(c.Row / 3)) * 3;
            int cBox = ((int)(c.Column / 3)) * 3;

            for (int i = rBox; i < rBox + 3; i++)
                for (int j = cBox; j < cBox + 3; j++)
                    if (_sudoku[i, j].Value == 0)
                        _sudoku[i, j].Domain[c.Value-1] = false;
        }
        
        //De-restrict all the domains of the cells that are connected to c 
        //This function is called when the algorithm return from a wrong branch
        private void de_restrictDomainsConnectedTo(Cell c)
        {
            if (c.Value == 0)
                return;

            de_restrictBoxDomains(c);
            de_restrictColumnDomains(c);
            de_restrictRowDomains(c);
        }
        private void de_restrictRowDomains(Cell c)
        {
            for (int i = 0; i < 9; i++)
                    if (_sudoku[c.Row, i].Value == 0)
                        _sudoku[c.Row, i].Domain[c.Value-1] = true;
            
        }
        private void de_restrictColumnDomains(Cell c)
        {
            for (int i = 0; i < 9; i++)
                if (_sudoku[i, c.Column].Value == 0)
                    _sudoku[i, c.Column].Domain[c.Value-1] = true;
            
        }
        private void de_restrictBoxDomains(Cell c)
        {
            int rBox = ((int)(c.Row / 3)) * 3;
            int cBox = ((int)(c.Column / 3)) * 3;

            for (int i = rBox; i < rBox + 3; i++)
                for (int j = cBox; j < cBox + 3; j++)
                    if (_sudoku[i,j].Value == 0)
                        _sudoku[i, j].Domain[c.Value-1] = true;
        }

        //DEBUG
        public void printdomains()
        {
            Console.WriteLine();

            foreach (Cell i in mrvList)
            {
                Console.WriteLine("[DEBUG MRV]> free cell @ [{0},{1}] DomainCount:{3}", i.Row, i.Column, i.Value, i.DomainCount());

                Console.ForegroundColor = ConsoleColor.Yellow;
                for(int j =0; j<9; j++)
                    if(i.Domain[j]==true)
                        Console.Write("{0},", j+1);
                Console.WriteLine();
                Console.ResetColor();

            }

        }


  
        /*************************************************************************************/
        /************************RELAXATION LABELING ONLY WITH ALC****************************/
        /*************************************************************************************/

        //DEBUG
        public void printProbabilities()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Console.Write("\n[DEBUG]> [{0},{1}] val: {2}  \n", i, j, _sudoku[i, j].Value);
                    foreach (double d in _sudoku[i, j].Pr)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("{0},", d);
                    }
                    Console.ResetColor();
                    Console.WriteLine();
                }
            }

            Console.WriteLine("[DEBUG] ALC : {0}", avgLocalConsistency());
        }

        //Relaxtion labeling termination by local average consistency
        public bool RelaxationLabelingALC()
        {
            //DEBUG
            NCALL = 0;

            double old_alc = avgLocalConsistency();
            updatePr();
            double new_alc = avgLocalConsistency();

            double stopRis = Math.Abs(old_alc - new_alc);

            while (stopRis>0.001)
            {
                NCALL++;
                old_alc = new_alc;
                updatePr();
                new_alc = avgLocalConsistency();
                stopRis= Math.Abs(old_alc - new_alc);

                //DEBUG
                Console.WriteLine("[DEBUG {0}] ALC : {1}",NCALL++, stopRis);
            }

            Cell tmp;
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    tmp = _sudoku[y, x];
                    tmp.Value = tmp.MaxPr() + 1;
                }
            }

            //DEBUG
            //printstructures();

            return true;
        }

        //Average local consistency
        private double avgLocalConsistency()
        {
            Cell i;
            double sum = 0;
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    i = _sudoku[x, y];
                    for (int lambda = 0; lambda < 9; lambda++)
                        sum += i.Pr[lambda] * support(i, lambda);
                }
            }
            return sum;
        }

        //Denominator for the update function
        private double denominator(Cell i)
        {
            double sum = 0;
            for (int mu = 0; mu < 9; mu++)
                sum+=i.Pr[mu] * support(i, mu);

            return sum;
        }

        //Update the probabilities of the cells 
        public void updatePr()
        {
            Cell i;
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    i = _sudoku[x, y];

                    for (int lambda = 0; lambda < 9; lambda++)
                    {
                        //togli
                        if (i.Value == 0)
                            i.Pr[lambda] = (i.Pr[lambda]*support(i,lambda))/denominator(i) ;
                    }

                }
            }
        }

        //Support function 
        private double support(Cell i, int lambda)
        {
            double sum = 0;
            Cell j;

            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    j = _sudoku[x, y];

                    for (int mu = 0; mu < 9; mu++)
                        sum += compatibility(i, lambda, j, mu)*j.Pr[mu];
                }
            }
            return sum;
        }

        //Compatibility of two cells
        private int compatibility(Cell i, int lambda, Cell j, int mu)
        {
            if (!sameRow(i, j) && !sameColumn(i, j) && !sameBox(i, j))
                return 1;

            if (lambda != mu)
                return 1;

            return 0;

        }

        private bool sameRow(Cell a, Cell b){ return a.Row == b.Row;}
        private bool sameColumn(Cell a, Cell b){ return a.Column == b.Column; }
        private bool sameBox(Cell a, Cell b)
        {
            int aRowBox = ((int)(a.Row / 3)) * 3;
            int aColBox = ((int)(a.Column / 3)) * 3;

            int bRowBox = ((int)(b.Row / 3)) * 3;
            int bColBox = ((int)(b.Column / 3)) * 3;

            return (aRowBox == bRowBox) && (aColBox == bColBox);
        }


        //It prints the sudoku
        public override string ToString()
        {

            string s = "";

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (j % 3 == 0)
                        s += " ";
                    if (_sudoku[i, j].Value == 0)
                        s += ". ";
                    else
                        s += _sudoku[i, j].ToString() + " ";

                }

                if ((i + 1) % 3 == 0)
                    s += '\n';
                s += "\n";

            }
            return s;



        }

    }

    
   
    class Cell : IComparable<Cell>
    {

        public int Row { get; private set; }
        public int Column { get; private set; }
        public int Value { get; set; }

        //Backtracking, domain vector
        public bool[] Domain { get; set; }

        //Relaxation Labelling, probability vector
        public double[] Pr { get; set; }

        public Cell(int row, int column, int value)
        {
            Row = row;
            Column = column;
            Value = value;

            //Backtracking
            Domain = new bool[9] { true, true, true, true, true, true, true, true, true };
            
            //Relaxation
            Pr = new double[9];
            if (value != 0)
            {
                for (int i = 0; i < 9; i++)
                    Pr[i] = 0.0;

                Pr[value-1] = 1.0;
            }
            else
            { 
                for (int i = 0; i < 9; i++)
                    Pr[i] = 1.0/9.0;            // ATTENZIONE ai dobule 1/9 torna un int :)
            }
        }

        //Backtracking, it counts how many values are fesible
        public int DomainCount()
        {
            int count = 0;

            for (int i = 0; i < 9; i++)
                if (Domain[i] == true)
                    count++;

            return count;
        }

        //Relaxation Labelling, it finds the max probable label
        public int MaxPr()
        {
            int maxIdx=-1;
            double maxPr = 0;

            for (int i = 0; i < 9; i++)
            {
                if (Pr[i] > maxPr)
                {
                    maxPr = Pr[i];
                    maxIdx = i;
                }
            }

            return maxIdx;
        }


        public override string ToString()
        {
            return "" + Value;
        }

        public int CompareTo(Cell other)
        {
            return this.DomainCount().CompareTo(other.DomainCount());
        }
    }

}
