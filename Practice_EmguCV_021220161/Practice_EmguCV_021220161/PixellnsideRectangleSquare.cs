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
    class PixellnsideRectangleSquare
    {
        public void answerApplyingPixelValue(VectorOfPoint crossPoints, Mat CannyImageAnswer, Mat originalImage, Mat cropImage, Mat smallRectangleCropImage)
        {
            int size = new int();//crossPoints.Size;

            // outpur of corss point group by four
            TextWriter fourCrossPoint = new StreamWriter(@"C:\Users\kings\Desktop\Paper\fourCrossPoint.txt");
            // if cross point is not in order rearrange them and output in file
            TextWriter rearrangeCrossPoint = new StreamWriter("C:\\Users\\Anish\\Desktop\\PaperGradingData\\rearrangeCrossPoint.txt");
            // remove point if its empty and rearrange them by group of four
            TextWriter removeZeroPointFile = new StreamWriter("C:\\Users\\Anish\\Desktop\\PaperGradingData\\removeZeroPointFile.txt");
            // small rectangle / square data file with pixel value and average number of pixel in each rectangle / square 
            TextWriter smallRectangleDataFile = new StreamWriter("C:\\Users\\Anish\\Desktop\\PaperGradingData\\smallRectangleDataFile.txt");
            AnswerUsingPixelValue forwardPixelValueForAnswer = new AnswerUsingPixelValue(); // create object using another file, so we can pass data to that
            // countrer to increase the value or change the value
            int k, z, l = new int();
            k = 0;
            z = 4;
            l = 0;
            fourCrossPoint.WriteLine(Convert.ToString("Total Points = " + crossPoints.Size));
            Point[] removeZeroPoint = new Point[32];

            // check which point is empty and remove it from array and save it again in new array 
            for (int i = 0; i < crossPoints.Size; i++)
            {
                if (crossPoints[i].IsEmpty)
                {
                    removeZeroPointFile.WriteLine(Convert.ToString("Point # i - " + i + " empty"));
                }
                else
                {
                    removeZeroPoint[l].X = crossPoints[i].X;
                    removeZeroPoint[l].Y = crossPoints[i].Y;
                    removeZeroPointFile.WriteLine(Convert.ToString("Point # i - " + i + " , L - " + l + " = " + removeZeroPoint[l]));
                    l++;
                }
            }
            removeZeroPointFile.WriteLine(Convert.ToString("Resaving and number of Array element L = " + l));

            // l = number of point saved in array
            size = l + 2;

            int[,] smallRectanglePixelValue = new int[(size / 4), 5];
            int[] smallRectanglePixelValueAverage = new int[(size / 4)];

            #region rearrange point group by four (to draw rectanlge),  flip array content if possible points is not in specific order

            Point[,] groupByfour = new Point[(size / 4), 4];
            Point[,] rearrangeFourPoint = new Point[(size / 4), 4];

            // group the cross by four and save it in another array of point
            for (int i = 0; i < (size / 4); i++)
            {
                //fourCrossPoint.WriteLine(Convert.ToString("#" + (i + 1)));
                for (int j = 0; k < z | j < 4; k++, j++)
                {
                    //fourCrossPoint.WriteLine(Convert.ToString("Point #" + (k + 1) + " " + crossPoints[k]));
                    groupByfour[i, j] = removeZeroPoint[k];
                }
                //fourCrossPoint.WriteLine(Convert.ToString("k = " + k));
                k = k + 0;
                z = z + 4;
            }


            // rearrange point of crosses because at some moment it change the position, which lead to error in pixel caclcuation
            for (int i = 0; i < (size / 4); i++)
            {
                // if the point is not in sequence, then flip the content of every array element
                if ((groupByfour[i, 0].X > groupByfour[i, 1].X) || (groupByfour[i, 2].X > groupByfour[i, 3].X))
                {
                    rearrangeFourPoint[i, 0] = groupByfour[i, 1];
                    rearrangeFourPoint[i, 1] = groupByfour[i, 0];
                    rearrangeFourPoint[i, 2] = groupByfour[i, 3];
                    rearrangeFourPoint[i, 3] = groupByfour[i, 2];
                }
                else // process the loop to save the same content of array to new array
                {
                    for (int j = 0; j < 4; j++)
                    {
                        rearrangeFourPoint[i, j] = groupByfour[i, j];
                    }
                }
            }
            /*
            // to check the output of saved point in an 2D array of points
            for (int i = 0; i < (size / 4); i++)
            {
                fourCrossPoint.WriteLine(Convert.ToString("New #" + (i + 1)));
                rearrangeCrossPoint.WriteLine(Convert.ToString("New #" + (i + 1)));

                for (int j = 0 ; j < 4; j++)
                {
                    fourCrossPoint.WriteLine(Convert.ToString("Point #" + (j + 1) + " " + groupByfour[i,j]));
                    rearrangeCrossPoint.WriteLine(Convert.ToString("Rearragne Point #" + (j + 1) + " " + rearrangeFourPoint[i, j]));
                }
            }*/
            #endregion

            #region draw big rectanlge, draw five sub-rectangle inside big one, get pixel data of both kind of rectanlge and save it in array

            Matrix<byte> matrixImage = new Matrix<byte>(CannyImageAnswer.Rows, CannyImageAnswer.Cols, CannyImageAnswer.NumberOfChannels);
            //Matrix<byte> smallRectangleCropMatrix = new Matrix<byte>(CannyImageAnswer.Rows, CannyImageAnswer.Cols, CannyImageAnswer.NumberOfChannels);

           // int totalBlackPixel = new int();
            //int totalWhitePixel = new int();
            int totalPixcelDataUsingFunction = new int();
            int totalSmallCropPixelData = new int();
            int AverageSmallCropPixelData = new int();
            totalSmallCropPixelData = 0;
            AverageSmallCropPixelData = 0;
           // totalBlackPixel = 0;
            //totalWhitePixel = 0;
            totalPixcelDataUsingFunction = 0;

            for (int i = 0; i < size / 4; i++)
            {
                fourCrossPoint.WriteLine(Convert.ToString("Rectangle #" + (i + 1)));

                for (int j = 0; j < 1; j++)
                {
                    int rowX1, rowX2, rowX3, rowX4 = new int();
                    int colY1, colY2, colY3, colY4 = new int();
                    // save x-axis, row in four different variable, value change with every loop
                    rowX1 = rearrangeFourPoint[i, j].X;
                    rowX2 = rearrangeFourPoint[i, j + 1].X;
                    rowX3 = rearrangeFourPoint[i, j + 2].X;
                    rowX4 = rearrangeFourPoint[i, j + 3].X;

                    // save y-axis, column in four different variable, value change with every loop
                    colY1 = rearrangeFourPoint[i, j].Y;
                    colY2 = rearrangeFourPoint[i, j + 1].Y;
                    colY3 = rearrangeFourPoint[i, j + 2].Y;
                    colY4 = rearrangeFourPoint[i, j + 3].Y;

                    // display the point in file 
                    fourCrossPoint.WriteLine(Convert.ToString("RowX1 = " + rowX1 + " " + "ColY1 = " + colY1));
                    fourCrossPoint.WriteLine(Convert.ToString("RowX2 = " + rowX2 + " " + "ColY2 = " + colY2));
                    fourCrossPoint.WriteLine(Convert.ToString("RowX3 = " + rowX3 + " " + "ColY3 = " + colY3));
                    fourCrossPoint.WriteLine(Convert.ToString("RowX4 = " + rowX4 + " " + "ColY4 = " + colY4));

                    // to draw the big rectangle around the four points of four cross
                    Rectangle rect = new Rectangle(rearrangeFourPoint[i, j + 2].X, rearrangeFourPoint[i, j + 2].Y, Math.Abs(rowX2 - rowX1), Math.Abs(colY1 - colY4));
                    fourCrossPoint.WriteLine(Convert.ToString("Rectangle Points #" + (i + 1) + " = " + rect.Location));
                    // draw rectangle by using "rect"
                    CvInvoke.Rectangle(originalImage, rect, new MCvScalar(200, 0, 0), 4, LineType.EightConnected, 0);
                    // get the center of rectangle
                    PointF center = new PointF(groupByfour[i, j + 2].X + 224, groupByfour[i, j + 2].Y + 45); // center of rectangle

                    // crop each rectangle to get the total number of pixel in next step
                    CvInvoke.GetRectSubPix(CannyImageAnswer, rect.Size, center, cropImage, DepthType.Default); // crop the image using the co-oridinate point
                    // to get the non-zero pixel (mean all pixel other than black), using function 
                    cropImage.CopyTo(matrixImage);
                    VectorOfPoint pixelData = new VectorOfPoint();
                    CvInvoke.FindNonZero(cropImage, pixelData); // to find total number of white pixel from image

                    fourCrossPoint.WriteLine(Convert.ToString("White Pixel Inside Rectangle = " + pixelData.Size)); // display in file
                    totalPixcelDataUsingFunction += pixelData.Size; // counter to add the all white pixel of all the recatangle

                    fourCrossPoint.WriteLine(Convert.ToString("Difference in Row  of Rectangle = " + Math.Abs(rowX2 - rowX1)));// display in file
                    fourCrossPoint.WriteLine(Convert.ToString("Difference in Cols Of Rectangle = " + Math.Abs(colY1 - colY4)));// display in file

                    int w = new int();
                    w = 0;
                    // to output all data in small rectangle datafile
                    smallRectangleDataFile.WriteLine(Convert.ToString("Rectangle #" + (i + 1)));// display in file
                    smallRectangleDataFile.WriteLine(Convert.ToString("\n"));// display in file
                    smallRectangleDataFile.WriteLine(Convert.ToString("RowX1 = " + rowX1 + " " + "ColY1 = " + colY1));// display in file
                    smallRectangleDataFile.WriteLine(Convert.ToString("RowX2 = " + rowX2 + " " + "ColY2 = " + colY2));// display in file
                    smallRectangleDataFile.WriteLine(Convert.ToString("RowX3 = " + rowX3 + " " + "ColY3 = " + colY3));// display in file
                    smallRectangleDataFile.WriteLine(Convert.ToString("RowX4 = " + rowX4 + " " + "ColY4 = " + colY4));// display in file
                    smallRectangleDataFile.WriteLine(Convert.ToString("\n"));// display in file
                    smallRectangleDataFile.WriteLine(Convert.ToString("White Pixel Inside Rectangle = " + pixelData.Size));// display in file
                    smallRectangleDataFile.WriteLine(Convert.ToString("\n"));// display in file

                    // to get small five rectangle inside every big rectangle detected
                    for (int t = 0; t < 361; t = t + 80)
                    {
                        Mat grayImage = new Mat();
                        //Mat smallRectangleCropImage = new Mat();
                        Matrix<byte> smallRectangleCropMatrix = new Matrix<byte>(CannyImageAnswer.Rows, CannyImageAnswer.Cols, CannyImageAnswer.NumberOfChannels);

                        // draw five sub-rectangle inside big rectangle
                        Rectangle smallRect = new Rectangle(rearrangeFourPoint[i, j + 2].X + t + 15, rearrangeFourPoint[i, j + 2].Y, Math.Abs((rowX2 - rowX1) / 5), Math.Abs(colY1 - colY4));
                        // draw rectangle / square by using "smallRect"
                        CvInvoke.Rectangle(originalImage, smallRect, new MCvScalar(200, 0, 255), 1, LineType.EightConnected, 0);

                        // get rectangle / square center, here i shift the point because to get the perfect small square from image
                        PointF smallCenter = new PointF(rearrangeFourPoint[i, j + 2].X + t + 65, rearrangeFourPoint[i, j + 2].Y + 36); // center of rectangle
                        // draw circle at smallCenter point
                        CvInvoke.Circle(originalImage, new Point((int)smallCenter.X, (int)smallCenter.Y), 3, new MCvScalar(0, 0, 255), -1, LineType.EightConnected, 0);
                        CvInvoke.PutText(originalImage, Convert.ToString("  " + (w + 1)), new Point((int)smallCenter.X, (int)smallCenter.Y), FontFace.HersheyComplex, 0.5, new MCvScalar(255, 0, 150), 1, LineType.FourConnected, false);
                        smallRectangleDataFile.WriteLine(Convert.ToString("Small Center #" + (w + 1) + " = " + smallCenter));

                        // crop rectangle into five sub-square / rectangle
                        CvInvoke.GetRectSubPix(CannyImageAnswer, smallRect.Size, smallCenter, smallRectangleCropImage, DepthType.Default); // crop the image using the co-oridinate point
                        smallRectangleDataFile.WriteLine(Convert.ToString("Small Crop Rectangle Size #" + (w + 1) + " = " + smallRectangleCropImage.Size));
                        // get all non-zero pixel of five sub-square / rectangle
                        smallRectangleCropImage.CopyTo(smallRectangleCropMatrix);
                        VectorOfPoint smallCropPixelData = new VectorOfPoint();
                        CvInvoke.FindNonZero(smallRectangleCropImage, smallCropPixelData);

                        // display data
                        smallRectangleDataFile.WriteLine(Convert.ToString("White Pixel Inside Small Rectangle #" + (w + 1) + " = " + smallCropPixelData.Size));
                        smallRectangleDataFile.WriteLine(Convert.ToString("\n"));

                        // save data in array, which we use in next file to get the answer
                        // i = number of rectangle, w = number of sub-square / rectangle
                        smallRectanglePixelValue[i, w] = smallCropPixelData.Size;
                        totalSmallCropPixelData += smallCropPixelData.Size;  // add all sub-square / rectangle pixel value
                        w++;
                    }
                    AverageSmallCropPixelData = totalSmallCropPixelData / 5; // average number of pixel in sub-square / rectangle
                    smallRectanglePixelValueAverage[i] = AverageSmallCropPixelData + 70; // increase the pixel value of sub-square / rectangle by 70

                    //display data in file
                    smallRectangleDataFile.WriteLine(Convert.ToString("Average Number of Pixel inside all small Rectangle #" + i + " = " + (AverageSmallCropPixelData + 70)));
                    smallRectangleDataFile.WriteLine(Convert.ToString("\n" + "------------------------------------------------------------------------------------" + "\n\n\n"));

                    // assign zero because we the program will go for next rectangle calculation
                    AverageSmallCropPixelData = 0;
                    totalSmallCropPixelData = 0;
                    // right here i have to create another another rectsubpix to get the subpixel value of sub five region of inside the rectangle

                    int row = cropImage.Rows; // total rows of crop image
                    int col = cropImage.Cols; // total columns of crop image                   
                }
                fourCrossPoint.WriteLine(Convert.ToString("\n"));
            }

            #endregion

            // forward the pixel data to another file for calculation
            forwardPixelValueForAnswer.answerByAccessingPixel(smallRectanglePixelValue, smallRectanglePixelValueAverage, size);

            fourCrossPoint.WriteLine(Convert.ToString("Nubmber of Channels = " + CannyImageAnswer.NumberOfChannels));
            fourCrossPoint.WriteLine(Convert.ToString("Total Rows in Image= " + CannyImageAnswer.Rows));
            fourCrossPoint.WriteLine(Convert.ToString("Total Cols in Image=  " + CannyImageAnswer.Cols));
            //fourCrossPoint.WriteLine(Convert.ToString("Total White Pixel = " + totalWhitePixel));
            //fourCrossPoint.WriteLine(Convert.ToString("Total Black Pixel = " + totalBlackPixel));
            fourCrossPoint.WriteLine(Convert.ToString("Total  Pixel Inside Rectangle = " + (CannyImageAnswer.Rows * CannyImageAnswer.Cols)));
            fourCrossPoint.WriteLine(Convert.ToString("Total White Pixel of all the Rectangle = " + totalPixcelDataUsingFunction));
            fourCrossPoint.WriteLine(Convert.ToString("Total Black Pixel outside Rectangle = " + ((CannyImageAnswer.Rows * CannyImageAnswer.Cols) - totalPixcelDataUsingFunction)));

            // close all file
            fourCrossPoint.Close();
            removeZeroPointFile.Close();
            smallRectangleDataFile.Close();
            rearrangeCrossPoint.Close();

        }


    }
}

