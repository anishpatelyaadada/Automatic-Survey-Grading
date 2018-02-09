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
    class AnswerUsingPixelValue
    {
        public void answerByAccessingPixel(int[,] smallRectanglePixelValue, int[] smallRectanglePixelValueAverage, int size)
        {
            TextWriter PixelSaveInArrayDataFile = new StreamWriter("C:\\Users\\Anish\\Desktop\\PaperGradingData\\PixelSaveInArrayDataFile.txt");
            TextWriter finalAnswerAccessingPixelValue = new StreamWriter("C:\\Users\\Anish\\Desktop\\PaperGradingData\\finalAnswerAccessingPixelValue.txt");
            int count = new int();
            count = 0;
            // loop start with last element of array and goes to first
            for (int i = (size / 4) - 1; i >= 0; i--)
            {
                PixelSaveInArrayDataFile.WriteLine(Convert.ToString(" Rectanlge #" + (count + 1)));
                for (int j = 0; j < 5; j++)
                {
                    PixelSaveInArrayDataFile.WriteLine(Convert.ToString(" i - " + (j + 1) + " = " + smallRectanglePixelValue[i, j]));
                }

                // all below "if" condition display the answer in the file
                if((smallRectanglePixelValue[i,0] > smallRectanglePixelValueAverage[i])) // strongly disagree
                {
                    finalAnswerAccessingPixelValue.WriteLine(Convert.ToString("Answer #" + (count + 1) + " - " + "Strongly Disagree. - 1"));
                }

                if ((smallRectanglePixelValue[i, 1] > smallRectanglePixelValueAverage[i])) // disagree
                {
                    finalAnswerAccessingPixelValue.WriteLine(Convert.ToString("Answer #" + (count + 1) + " - " + "Disagree. - 2"));
                }
                
                if (smallRectanglePixelValue[i, 2] > smallRectanglePixelValueAverage[i]) // unsure
                {
                    finalAnswerAccessingPixelValue.WriteLine(Convert.ToString("Answer #" + (count + 1) + " - " + "Unsure. - 3"));
                }
                if (smallRectanglePixelValue[i, 3] > smallRectanglePixelValueAverage[i]) // agree
                {
                    finalAnswerAccessingPixelValue.WriteLine(Convert.ToString("Answer #" + (count + 1) + " - " + "Agree. - 4"));
                }
                if (smallRectanglePixelValue[i, 4] > smallRectanglePixelValueAverage[i]) // strongly agree
                {
                    finalAnswerAccessingPixelValue.WriteLine(Convert.ToString("Answer #" + (count + 1) + " - " + "Strongly Agree. - 5"));
                }

                PixelSaveInArrayDataFile.WriteLine(Convert.ToString(" Average Value #" + (count + 1) + " = " + smallRectanglePixelValueAverage[i]));
                PixelSaveInArrayDataFile.WriteLine(Convert.ToString("\n" + " ----------------------------------------------------------" + "\n"));
                count++;
            }
            PixelSaveInArrayDataFile.Close();
            finalAnswerAccessingPixelValue.Close();
        }
    }
}
