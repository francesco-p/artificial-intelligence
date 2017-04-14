# artificial-intelligence 

> This repository contains the assignments that I've done during the Artificial Intelligence : Knowledge Representation course @ Ca' Foscari University. Each folder contains the assignment with the respective problem statement, report and code.


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

### Folder  `assignment1/backtracking`:
  - Contains the backtracking algorithm in C#

### Folder  `assignment1/relaxationlabeling `:
  - Contains the  `relaxationlabeling.c` source for the relaxation labelling algorithm compilation with -O3 for best performances
  - Contains folder  `dataset` where there are all the sudoku that I tried to plot the figures in the report
  - Contains  `csv` folder, where there are all the csv files that I created from the dataset to plot the figures in the report


# Assignment 2 : Decision Tree Learning

Learn a **decision tree** for the [Heart Disease data set](http://archive.ics.uci.edu/ml/datasets/Heart+Disease) of the [UCI Machine Learning Repository](http://archive.ics.uci.edu/ml/)

Write a program that learns a tree with maximum depth d and choose the depth using cross-validation on the training set.
Evaluate the performance of the learned tree on a test set of at least 100 samples.
Data used for learning (even cross validation) should neve be used for testing!
Deliver the learning code as well as a report of the cross validation and training results and the lessons learned.

### Folder  `assignment2/learningtrees`:
  - Contains the C# code of an improved version of the ID3 algorithm (it resembles C4.5) 


# Assignment 3 : Dimensionality Reduction and Classification

Perform perform **Principal Component Analysis** of the Semeion Handwritten Digit data set and show a scatter plot of the first 2 principal components of all the images in the database.

Study the performance of a **Nearest Neighbor classifier** as you increase the number of the principal components from 0 (means only) to the full set of 256 components. Take 10 randomly selected images per class as learning samples for k-nn. Repeat the selection and classification several times.

Test also the performance of the Nearest Neighbor classifier using all the dimensions available, but computing the distances on the whitened data and the independet components. For ICA you can use the implemetnation here.


### Folder `assignment3/html`
- Published code with matlab, other parts of the report.

	Instead of running the code (task2 takes 10 mins, unless you load the .mat file and run the plot section) 
	you can give a look each task html, it's published directly within matlab

### Folder `assignment3/code`
- The code. 
	To run the code : include to the matlab workspace --> everything inside the code folder.


# Assignment 4 : Manifold Learning 

> I deleted the original problem statement, this is a brief description

Compare PCA with **k-Isomap** and **k-Laplacian Eigenmap** using the hand posture [Triesch Dataset](http://www.idiap.ch/resource/gestures/)


### Folder `assignment4/code`
- IsomapStanford : matlab files to compute isomap
- LaplacianBeikin: matlab files to compute laplacian eigenmaps
- `Triesch_Dataset`: the dataset 
- several files `<name>.m`: these are usiliary files 
- `pcaTask.m` `klemTask.m` `kisomapTask.m` : these are the main tasks to run in matlab. Before run them, we must import the three folders into Matlab. 

