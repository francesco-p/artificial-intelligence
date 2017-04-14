/*
* TO COMPILE : native.c -lm  to link the math lib
* BUT MORE IMPORTANT: compile with the -O3 flag to substantial optimizations
*/
#include <stdio.h>
#include <time.h>
#include "math.h"


//DEBUGGING STUFF
int NCALL=0;

struct cell
{
    int value;
    int row;
    int column;
    float pr[9];
};

struct cell sudoku[9][9];

// DEBUG Print probabilities
void printProbabilities()
{
  int i,j,k;
  for (i = 0; i < 9; i++)
  {
    for (j = 0; j < 9; j++)
    {
      printf("\n[%d][%d] val : %d\n",sudoku[i][j].row,sudoku[i][j].column,sudoku[i][j].value);
      for (k = 0; k < 9; k++)
      {
        printf("%f,",sudoku[i][j].pr[k]);
      }
    }
  }
}

// DEBUG Print sudoku
void printSudoku()
{

  int i,j;
  printf("\n");
  for (i = 0; i < 9; i++)
  {
    if(i%3 == 0 )
      printf("\n");

    for (j = 0; j < 9; j++)
    {
      if(j%3 == 0)
        printf(" ");

      if(sudoku[i][j].value==0)
        printf(". ");
      else
        printf("%d ",sudoku[i][j].value);
    }
    printf("\n");
  }
}

// Takes a matrix of int and create a matrix of struct cell
void initializeSudoku(int inputSudoku[9][9])
{
      int i,j,k,index=0;
      for(i=0;i<9;i++)
      {
          for(j=0;j<9;j++)
          {
              sudoku[i][j].row = i;
              sudoku[i][j].column = j;
              sudoku[i][j].value = inputSudoku[i][j];

              if(sudoku[i][j].value != 0)
              {
                for(k=0;k<9;k++)
                  sudoku[i][j].pr[k]=0.0;

                index = sudoku[i][j].value - 1;
                sudoku[i][j].pr[index] = 1.0;
              }
              else
              {
                for(k=0;k<9;k++)
                  sudoku[i][j].pr[k]=1.0/9.0;
              }
          }
      }


}

// Find the max probable label of a given cell
int maxPr(int row, int column)
{
  int k,maxIdx = 0;
  float tmpr, maxPr = 0.0;
  for (k = 0; k < 9; k++)
  {
    tmpr = sudoku[row][column].pr[k];
    if(tmpr>maxPr)
    {
      maxPr = tmpr;
      maxIdx = k;
    }
  }
  return maxIdx;
}

// For each cell find the label with highest probability and set it in the sudoku
void setCandidates()
{
  int x,y=0;
  for (x = 0; x < 9; x++)
    for (y = 0; y < 9; y++)
      sudoku[x][y].value = maxPr(x,y)+1;
}

int sameRow(struct cell i, struct cell j){ return i.row == j.row; }
int sameColumn(struct cell i, struct cell j){ return i.column == j.column; }
int sameBox(struct cell i, struct cell j)
{
  int iRowBox = (i.row / 3) * 3;
  int iColBox = (i.column / 3) * 3;

  int jRowBox = (j.row / 3) * 3;
  int jColBox = (j.column / 3) * 3;

  return (iRowBox == jRowBox) && (iColBox == jColBox);
}


/***************************************************************************/
/********************** Relaxation Labeling Algorithm **********************/
/***************************************************************************/

int compatibility(struct cell i, int lambda, struct cell j, int mu)
{
  if(!sameRow(i,j) && !sameColumn(i,j) && !sameBox(i,j))
    return 1;

  if (lambda != mu)
    return 1;

  return 0;


}

float support(struct cell i, int lambda)
{
  float sum=0;
  struct cell j;
  int x,y,mu=0;

  for (x = 0; x < 9; x++)
  {
    for (y = 0; y < 9; y++)
    {
      j=sudoku[x][y];

      for (mu = 0; mu < 9; mu++)
        sum += compatibility(i, lambda, j, mu) * j.pr[mu];

    }
  }
  return sum;
}

// Helping function
float denominator(struct cell i )
{
  float sum=0;
  int mu = 0 ;

  for (mu = 0; mu < 9; mu++)
    sum+=i.pr[mu] * support(i,mu);

  return sum;
}

void updatePr()
{
  int x,y,lambda=0;
  struct cell* i;

  for (x = 0; x < 9; x++)
  {
      for (y = 0; y < 9; y++)
      {
          i = &sudoku[x][y];

          for (lambda = 0; lambda < 9; lambda++)
          {
              if (i->value == 0)
                  i->pr[lambda] = (i->pr[lambda]*support(*i,lambda))/denominator(*i) ;
          }

      }
  }
}

/***************************************************************************/
/************************* Relaxation Labeling functions *******************/
/***************************************************************************/
float avgLocalConsistency()
{
  struct cell* i;
  float sum = 0;
  int x,y,lambda=0;

  for (x = 0; x < 9; x++)
  {
      for ( y = 0; y < 9; y++)
      {
          i = &sudoku[x][y];
          for (lambda = 0; lambda < 9; lambda++)
              sum += i->pr[lambda] * support(*i, lambda);
      }
  }
  return sum;
}

int RelaxationLabelingALC()
{
  float old_alc, new_alc, alcDiff=0.0;

  old_alc= avgLocalConsistency();
  updatePr();
  new_alc=avgLocalConsistency();

  alcDiff=fabs(old_alc-new_alc);
  printf("[DEBUG %d]> ALC: %f \n",NCALL++,alcDiff);

  while (alcDiff>0.0001)
  {
    old_alc=new_alc;
    updatePr();
    new_alc=avgLocalConsistency();
    alcDiff=fabs(old_alc-new_alc);
    printf("[DEBUG %d]> oldALC: %f  newALC: %f ALCdiff:%f\n",NCALL++,old_alc,new_alc,alcDiff);

  }
  setCandidates();
  return 1;
}


int euclideanTermination(float oldProb[9][9][9], float tollerance)
{
  int x,y,k=0;
  float diff=0;
  float eucl=0;

  for (x = 0; x < 9; x++)
    for (y = 0; y < 9; y++)
      for (k = 0; k < 9 ; k++)
      {
        diff=fabs(oldProb[x][y][k]-sudoku[x][y].pr[k]);
        eucl+=pow(diff,2);
      }

eucl=sqrt(eucl);
printf("[DEBUG %d] euclidean distance : %f \n",NCALL,eucl);

if(eucl>tollerance)
  return 1;

return 0;
}

int RelaxationLabelingEUC(float tollerance)
{
  int x,y,k;
  float oldProb[9][9][9];

  for (x = 0; x < 9; x++)
    for (y = 0; y < 9; y++)
      for (k = 0; k < 9 ; k++)
        oldProb[x][y][k]=sudoku[x][y].pr[k];

  updatePr();

while(euclideanTermination(oldProb,tollerance))
  {
    NCALL++;
    for (x = 0; x < 9; x++)
      for (y = 0; y < 9; y++)
        for (k = 0; k < 9 ; k++)
          oldProb[x][y][k]=sudoku[x][y].pr[k];

    updatePr();
  }
setCandidates();
return 1;
}

int myTermination(float oldProb[9][9][9], float tollerance)
{
  int x,y,k=0;
  float diff=0;

  for (x = 0; x < 9; x++)
    for (y = 0; y < 9; y++)
      for (k = 0; k < 9 ; k++)
      {
        diff=fabs(oldProb[x][y][k]-sudoku[x][y].pr[k]);
        if(diff>tollerance)
        {
          printf("[DEBUG %d] stuck @ [%d][%d][%d] oldProb:%f newProb:%f diff:%f \n",NCALL,x,y,k,oldProb[x][y][k], sudoku[x][y].pr[k], diff);
          return 1;
        }
      }

return 0;
}

int RelaxationLabelingMyTermination(float tollerance)
{
  int x,y,k;
  float oldProb[9][9][9];

  for (x = 0; x < 9; x++)
    for (y = 0; y < 9; y++)
      for (k = 0; k < 9 ; k++)
        oldProb[x][y][k]=sudoku[x][y].pr[k];

  updatePr();

while(myTermination(oldProb,tollerance))
  {
    NCALL++;

    for (x = 0; x < 9; x++)
      for (y = 0; y < 9; y++)
        for (k = 0; k < 9 ; k++)
          oldProb[x][y][k]=sudoku[x][y].pr[k];

    updatePr();

  }
setCandidates();
return 1;
}

int RelaxationLabeling(int termination, float tollerance)
{
  if(termination == 0)
    return RelaxationLabelingMyTermination(tollerance);

  if(termination == 1)
    return RelaxationLabelingEUC(tollerance);
  else
    return RelaxationLabelingALC(tollerance);
}

/***************************************************************************/
/************************** Error Count Functions **************************/
/***************************************************************************/
int rowConstrain(struct cell cell)
{
  int j = 0;

  int i = cell.row;

  for (j=0; j < 9; j++)
  {
    if(i==cell.row && j==cell.column)
      continue;

    if(sudoku[i][j].value == cell.value)
    {
      printf("Error in row %d between [%d][%d] and [%d][%d] for value: %d \n",i,i,j,cell.row,cell.column,cell.value);
      return 1;
    }
  }
return 0;
}

int columnConstrain(struct cell cell)
{
  int i = 0;

  int j = cell.column;

  for (i=0; i < 9; i++)
  {
    if(i==cell.row && j==cell.column)
      continue;

    if(sudoku[i][j].value == cell.value)
    {
      printf("Error in column %d between [%d][%d] and [%d][%d] for value: %d \n",j,i,j,cell.row,cell.column,cell.value);
      return 1;
    }
  }
return 0;
}

int boxConstrain(struct cell cell)
{

  int i,j=0;
  int iBox = (cell.row / 3)*3;
  int jBox = (cell.column / 3)*3;

  for ( i = iBox; i < 3; i++)
  {
    for ( j = jBox; j < 3; j++)
    {
      if(i==cell.row && j==cell.column)
        continue;

      if(sudoku[i][j].value == cell.value)
      {
        printf("Box Incompatibility between [%d][%d] and [%d][%d] for value: %d \n",i,j,cell.row,cell.column,cell.value);
        return 1;
      }
    }
  }
return 0;
}

int respectConstrains(struct cell cell)
{
  return rowConstrain(cell) + columnConstrain(cell) + boxConstrain(cell);
}

// Find if there are errors in the sudoku
int countErrors()
{
  int i,j,err=0;
  for (i = 0; i < 9; i++)
    for (j = 0; j < 9; j++)
       err+=respectConstrains(sudoku[i][j]);

  return err/2;
}


/***************************************************************************/
/********************************** Main ***********************************/
/***************************************************************************/

int main()
{
/*
 * Professor's sudoku - 9 seconds with double, 3 seconds with float
 * World most difficult sudoku - does NOT CONVERGE
        http://aisudoku.com/index_en.html
        http://www.telegraph.co.uk/news/science/science-news/9359579/Worlds-hardest-sudoku-can-you-crack-it.html
        http://www.mirror.co.uk/news/weird-news/worlds-hardest-sudoku-puzzle-ever-942299
        {8,0,0,0,0,0,0,0,0},
        {0,0,3,6,0,0,0,0,0},
        {0,7,0,0,9,0,2,0,0},
        {0,5,0,0,0,7,0,0,0},
        {0,0,0,0,4,5,7,0,0},
        {0,0,0,1,0,0,0,3,0},
        {0,0,1,0,0,0,0,6,8},
        {0,0,8,5,0,0,0,1,0},
        {0,9,0,0,0,0,4,0,0}

* Empty sudoku - trivally does NOT CONVERGE
*/


    int inputSudoku[9][9] = {

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


initializeSudoku(inputSudoku);
printSudoku();

time_t start,end; // TIC
time (&start); // TIC

// First parameter: 0 for mytermination (third described in the report),
//                  1 for euclidean termination (second described),
//                  average local consistency otherwise (first described)
// Second parameter: tollerance
RelaxationLabeling(0,0.0001);

time (&end); // TOC
float dif = difftime (end,start); // TOC
printf ("\nElapsed time %.2lf seconds.\n", dif );//TOC

printSudoku();

int errors=countErrors();
if(!errors)
  printf("No errors!\n" );
else
  printf("There are %d errors\n",errors );

return 0;
}
