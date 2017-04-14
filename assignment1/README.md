# Assignment 1 : Sudoku Solver

Write a solver for sudoku puzzles using a **constraint satisfaction** approach based on constraint propagation and backtracking, and one based on **Relaxation Labelling**. compare the approaches, their strenghts and weaknesses.

A sudoku puzzle is composed of a square 9x9 board divided into 3 rows and 3 columns of smaller 3x3 boxes.
The goal is to fill the board with digits from 1 to 9 such that each number appears only once for each row column and 3x3 box;
Row, column, and 3x3 box should containg all 9 digits.

The solver should take as input a matrix withwhere empty squares are represented by a standars symbol (e.g., `.`, `_`, or `0`), while known square should be represented by the corresponding digit (1,...,9). For example:

```
3 7 .  5 . .  . . 6
. . .  3 6 .  . 1 2
. . .  . 9 1  7 5 .

. . .  1 5 4  . 7 .
. . 3  . 7 .  6 . .
. 5 .  6 3 8  . . .

. 6 4  9 8 .  . . .
5 9 .  . 2 6  . . .
2 . .  . . 5  . 6 4

```

**Hints for Constraint Propagation and Backtracking:**

- Each cell should be a variable that can take values in the domain (1,...,9).
- The two types of constraints in the definition form as many types of constraints:
    - Direct constraints impose that no two equal digits appear for each row, column, and box;
    - Indirect constrains impose that each digit must appear in each  row, column, and box.
- You can think of other types of (pairwise) constraints to further improve the constraint propagation phase.
- Note: most puzzles found in the magazines can be solved with only the constraint propagation step.

**Hints for Relaxation Labelling:**

Each cell should be an object, the values between 1 and 9 labels.
The compatibility r<sub>ij</sub>(&lambda;,&mu;) should be 1 if the assigments satisfy direct constraints, 0 otherwise.

### Folder  `backtracking`:
  - Contains the backtracking algorithm in C#

### Folder  `relaxationlabeling `:
  - Contains the  `relaxationlabeling.c` source for the relaxation labelling algorithm compilation with -O3 for best performances
  - Contains folder  `dataset` where there are all the sudoku that I tried to plot the figures in the report
  - Contains  `csv` folder, where there are all the csv files that I created from the dataset to plot the figures in the report

