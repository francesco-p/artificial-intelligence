using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class Program
    {
        static void Main(string[] args) {





            

            int[,] professorSudoku = new int[9, 9] {
{3,7,0,5,0,0,0,0,6},
{0,0,0,3,6,0,0,1,2},
{0,0,0,0,9,1,7,5,0},
{0,0,0,1,5,4,0,7,0},
{0,0,3,0,7,0,6,0,0},
{0,5,0,6,3,8,0,0,0},
{0,6,4,9,8,0,0,0,0},
{5,9,0,0,2,6,0,0,0},
{2,0,0,0,0,5,0,6,4}
                                            };
            // World most difficult sudoku
            // http://aisudoku.com/index_en.html
            // http://www.telegraph.co.uk/news/science/science-news/9359579/Worlds-hardest-sudoku-can-you-crack-it.html
            // http://www.mirror.co.uk/news/weird-news/worlds-hardest-sudoku-puzzle-ever-942299


            int[,] worldSudoku = new int[9, 9] {
{8,0,0,0,0,0,0,0,0},
{0,0,3,6,0,0,0,0,0},
{0,7,0,0,9,0,2,0,0},
{0,5,0,0,0,7,0,0,0},
{0,0,0,0,4,5,7,0,0},
{0,0,0,1,0,0,0,3,0},
{0,0,1,0,0,0,0,6,8},
{0,0,8,5,0,0,0,1,0},
{0,9,0,0,0,0,4,0,0}
                                            };



            SudokuSolver ss = new SudokuSolver(professorSudoku);


                  Console.WriteLine("--------------- ProfessorSudoku ---------------\n{0}", ss);
                  if (ss.Backtracking())
                      Console.WriteLine("ProfessorSudoku - Simple Backtracking SOLUTION in {0} steps :\n{1}", SudokuSolver.NCALL, ss);
                  else
                      Console.WriteLine("ProfessorSudoku - Simple Backtracking NO SOLUTION");

                  ss.SetSudoku(professorSudoku);
                  ss.PreliminaryCheck();
                  if (ss.BacktrackingConstr())
                      Console.WriteLine("Sudoku - Constraint Propagation SOLUTION in {0} steps :\n{1}", SudokuSolver.NCALL, ss);
                  else
                      Console.WriteLine("Sudoku - Constraint Propagation NO SOLUTION");


            ss.SetSudoku(worldSudoku);
            Console.WriteLine("--------------- World most difficult Sudoku ---------------\n{0}", ss);
            if (ss.Backtracking())
                Console.WriteLine("World most difficult - Simple Backtracking SOLUTION in {0} steps :\n{1}", SudokuSolver.NCALL, ss);
            else
                Console.WriteLine("World most difficult  - Simple Backtracking NO SOLUTION");

            ss.SetSudoku(worldSudoku);
            ss.PreliminaryCheck();
            if (ss.BacktrackingConstr())
                Console.WriteLine("Sudoku - Constraint Propagation SOLUTION in {0} steps :\n{1}", SudokuSolver.NCALL, ss);
            else
                Console.WriteLine("Sudoku - Constraint Propagation NO SOLUTION");




            /*
          //TIC
          var watch = Stopwatch.StartNew();

          ss.SetSudoku(sudoku);

          if (ss.RelaxationLabelingALC())
              Console.WriteLine("Sudoku - Relaxation Labeling SOLUTION in {0} steps :\n{1}", SudokuSolver.NCALL, ss);
          else
              Console.WriteLine("Sudoku - Relaxation Labeling NO SOLUTION");

          //TOC
          watch.Stop();
          var elapsedMs = watch.ElapsedMilliseconds;
              Console.WriteLine("[DEBUG]> Elapsed time: {0}", elapsedMs);
      */

            Console.ReadLine();




        }


    }

}

