using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;

using Emgu.Util;
using Emgu.Util.TypeEnum;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;

namespace Practice_EmguCV_021220161
{
    class Calculation
    {
        public void detectedPointsInPaper(VectorOfPoint foundPointAnswer)
        {
            TextWriter PointReceivedForCalculation = new StreamWriter("C:\\Users\\Anish\\Desktop\\PaperGradingData\\PointReceivedForCalculation.txt");
            TextWriter answersInFile = new StreamWriter("C:\\Users\\Anish\\Desktop\\PaperGradingData\\AnswersInFile.txt");

            int size = foundPointAnswer.Size;
            int XchangeableValue = new int();
            int X1 = new int();
            int X2 = new int();
            int X4 = new int();
            int X5 = new int();
            int X1X2Diffs = new int();
            int X4X5Diffs = new int();
            int XAvg = new int();
            int XAvgFiveBoxes = new int();
            /*detect five point excluding point numebr three
             take all x-axis value of all the point and save it 
             to one variable as shown below
             */
            X1 = foundPointAnswer[0].X;
            X2 = foundPointAnswer[1].X;
            X4 = foundPointAnswer[3].X;
            X5 = foundPointAnswer[4].X;

            ///now take the difference of first and second point, third and fift point
            X1X2Diffs = Math.Abs(X1 - X2);
            X4X5Diffs = Math.Abs(X4 - X5);

            // take the average of above two difference point
            XAvg = (X1X2Diffs + X4X5Diffs) / 2;

            // average difference between two square, which is total five square ~89
            XAvgFiveBoxes = XAvg / 5;

            // if there is total 35 objects detected in the paper then guess the answer, else print the else statement
            if (size == 35)
            {
                #region to guess the answer
                for (int i = 0; i < size; i++)
                {

                    // to get the x-axis value of first cross out of total four cross
                    if (i == 0 || i == 5 || i == 10 || i == 15 || i == 20 || i == 25 || i == 30)
                    {
                        XchangeableValue = foundPointAnswer[i].X;
                    }

                    // array where the answer is saved
                    if (i == 2 || i == 7 || i == 12 || i == 17 || i == 22 || i == 27 || i == 32)
                    {
                        if ((foundPointAnswer[i].X > XchangeableValue) && (foundPointAnswer[i].X < (XchangeableValue + XAvgFiveBoxes))) /*answer #A Strongly Disagree */
                        {
                            answersInFile.WriteLine(Convert.ToString("Answers #" + i + " = Strongly Disagree" + " ,XchangeableValue = " + XchangeableValue + " ,XAvg = " + XAvg + " ,XAvgFiveBoxes = " + XAvgFiveBoxes + ", foundPointAnswer = " + foundPointAnswer[i].X));

                        }
                        else if ((foundPointAnswer[i].X > (XchangeableValue + XAvgFiveBoxes)) && (foundPointAnswer[i].X < (XchangeableValue + (XAvgFiveBoxes * 2))))/*answer #B Disagree*/
                        {
                            answersInFile.WriteLine(Convert.ToString("Answers #" + i + " = Disagree"));
                        }
                        else if ((foundPointAnswer[i].X > (XchangeableValue + (XAvgFiveBoxes * 2))) && (foundPointAnswer[i].X < (XchangeableValue + (XAvgFiveBoxes * 3))))/*answer #C Unsure*/
                        {
                            answersInFile.WriteLine(Convert.ToString("Answers #" + i + " = Unsure"));
                        }
                        else if ((foundPointAnswer[i].X > (XchangeableValue + (XAvgFiveBoxes * 3))) && (foundPointAnswer[i].X < (XchangeableValue + (XAvgFiveBoxes * 4))))/*answer #D Agree*/
                        {
                            answersInFile.WriteLine(Convert.ToString("Answers #" + i + " = Agree"));
                        }
                        else if ((foundPointAnswer[i].X > (XchangeableValue + (XAvgFiveBoxes * 4))) && (foundPointAnswer[i].X < (XchangeableValue + (XAvgFiveBoxes * 5))))/*answer #E Strongly Agree */
                        {
                            answersInFile.WriteLine(Convert.ToString("Answers #" + i + " = Strongly Agree"));
                        }

                    }
                }

                #endregion
            }
            else
            {
                answersInFile.WriteLine(Convert.ToString("The Answer cannot be recognize due to some kind of error."));
            }
            answersInFile.Close();
            PointReceivedForCalculation.Close();
        }
    }
}
