﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;
using System.Numerics;

namespace ML_DecisionTreeClassifier
{
    public partial class MainWindow : Window
    {
        private string filedir = Directory.GetCurrentDirectory() + "\\.." + "\\.." + "\\..";
        private string decisionTreeOutput;

        public MainWindow()
        {
            InitializeComponent();
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void RunA_Click(object sender, RoutedEventArgs e)
        {
            string filepath = filedir + "\\testDataA4\\a.in";
            ReadFile(filepath);
        }

        private void ReadFile(string filePath)
        {
            try
            { 
                //Open file
                StreamReader reader = new StreamReader(filePath);

                MessageBox.Show("File opened");

                //Read in number of classes
                int numberOfClasses = Convert.ToInt32(reader.ReadLine());
                Display.Text = "Number of classes: " + numberOfClasses.ToString() + "\n";
                Test.Text = "";

                //List to keep track of what classes this file contains
                List<string> classes = new List<string>();

                //List to keep trakc of what types of data each class is 
                List<char> dataTypes = new List<char>();

                //List to keep track of what possible attributes there are 
                List<string> attributeList = new List<string>();

                //Parse the next few lines to get classes and possible values
                for (int i = 1; i <= numberOfClasses + 1; i++)
                {
                    //Read entire line
                    string line = reader.ReadLine();
                    
                    //Split it into parts
                    string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    
                    //First partition is the class name -> called type
                    string type = parts[0];


                    //if the class is the answer, check to see if it has already been added to the list
                    if (type.ToLower() == "ans")
                    {
                        classes.Add(type);
                        dataTypes.Add('S');
                        for (int a = 1; a < parts.Length; a++)
                        {
                            attributeList.Add(parts[a]);
                        }
                    }
                    else
                    {
                        classes.Add(type);

                        //check if the data is continuous
                        if (parts[1] == "continuous")
                        {
                            dataTypes.Add('C');
                        }

                        //otherwise, consider it to be nominal
                        else
                        {
                            dataTypes.Add('S');
                        }
                    }


                    
                }

                //Print what each classes data type is to the display window
                for (int d = 0; d < classes.Count; d++)
                {
                    if (dataTypes[d] == 'C')
                        Display.Text += classes[d] + " is continuous\n";
                    else if (dataTypes[d] == 'S')
                        Display.Text += classes[d] + " is nominal\n";
                }

                //Create a list that contains all the lines of data
                List<List<AttributeNode>> tuples = new List<List<AttributeNode>>();


                //Read in remaining values
                Display.Text += "\nTuples\n";
                while (!reader.EndOfStream)
                {
                    //Read the line and split into partitions based on spaces
                    string line = reader.ReadLine();
                    string [] parts = line.Split(new char[] {' ' }, StringSplitOptions.RemoveEmptyEntries);
                    
                    
                    //Create a list that contains all the nodes on one line
                    List<AttributeNode> currentLine = new List<AttributeNode>();



                    for (int i = 0; i < parts.Length; i++)
                    {
                        //get current class and data type
                        string currentClass = classes[i];
                        char currentType = dataTypes[i];
                        

                        if (currentType == 'C')
                        {
                            double currentValue = double.Parse(parts[i]);
                            //Display.Text += currentValue + " ";
                            AttributeNode node = new AttributeNode(currentClass, currentType, currentValue);
                            currentLine.Add(node);
                        }

                        if (currentType == 'S')
                        {
                            string currentValue = parts[i];
                            //Display.Text += currentValue + " ";
                            AttributeNode node = new AttributeNode(currentClass, currentType, currentValue);
                            currentLine.Add(node);
                        }
                    }


                    //Test to see if line is being read in correctly
                    foreach(AttributeNode node in currentLine)
                    {
                        if(node.dataType == 'C')
                            Display.Text += node.continuous + " ";
                        if (node.dataType == 'S')
                            Display.Text += node.word + " ";
                    }


                    //Add the data from each line to a list of tuples
                    tuples.Add(currentLine);
                    Display.Text += "\n";
                }


                //display possible answers
                int possibleNumberOfAnswers = 0;
                Display.Text += "\nAnswers\n";
                foreach (string answerPossibility in attributeList)
                {
                    Display.Text += answerPossibility + " ";
                }
                Display.Text += "\n";


                //close file
                reader.Close();


                //quick read for easy comparision
                SpeedRead(filePath);
                string outfile = filePath.Substring(0, filePath.Length - 3) + ".out";
                try { SpeedOut(outfile); }
                catch (Exception ex) { OutfileBox.Text = "no .out file for this dataset"; }
                


                //before building the decision tree, call a function that will calculate a split point for each continuous class of data
                //then convert the continous data to a string based on the split point
                List<List<AttributeNode>> finalTuples = new List<List<AttributeNode>>();

                //check the first line in the data matrix to see how many continuous attributes there are
                List<int> continuousAttributeIndexes = new List<int>();   
                for(int i = 0; i < tuples[0].Count - 1; i++)
                {                  
                    if(tuples[0].ElementAt(i).dataType == 'C')
                    {
                        continuousAttributeIndexes.Add(i);
                    }
                }

                //now that we know how many contininous attributes there are and the indexes, we can build a list possible split points
                List<List<double>> allPossibleSplitPoints = new List<List<double>>();
                foreach(int continuousIndex in continuousAttributeIndexes)
                {
                    List<double> possibleSplitPoints = new List<double>();
                    foreach(List<AttributeNode> tuple in tuples)
                    {
                        possibleSplitPoints.Add(tuple[0].continuous);
                    }
                    
                    allPossibleSplitPoints.Add(possibleSplitPoints);
                }


                //build decision tree
                Tree DecisionTree = new Tree(tuples, attributeList, numberOfClasses);
                DecisionTree.removeAllContinuous(continuousAttributeIndexes, allPossibleSplitPoints);
                DecisionTree.StartTree();
                decisionTreeOutput = DecisionTree.PrintTree();
                Test.Text = decisionTreeOutput;

            }
            catch (Exception ex)
            {
                Display.Text = ex.Message;
            }
        }

        


       

        //This function is for reading the file in one line and displaying the contents to the window
        private void SpeedRead(string filepath)
        {
            StreamReader stream = new StreamReader(filepath);
            var content = stream.ReadToEnd();
            FileInput.Text = content;
            stream.Close();
        }

        private void SpeedOut(string filepath)
        {
            StreamReader stream = new StreamReader(filepath);
            var content = stream.ReadToEnd();
            OutfileBox.Text = content;
            stream.Close();
        }

        private void CircuitButton_Click(object sender, RoutedEventArgs e)
        {
            string filepath = filedir + "\\testDataA4\\circuit.in";
            ReadFile(filepath);
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            string filepath = filedir + "\\testDataA4\\continue.in";
            ReadFile(filepath);
        }

        private void Continue0Button_Click(object sender, RoutedEventArgs e)
        {
            string filepath = filedir + "\\testDataA4\\continue0.in";
            ReadFile(filepath);
        }

        private void Continue2Button_Click(object sender, RoutedEventArgs e)
        {
            string filepath = filedir + "\\testDataA4\\continue2.in";
            ReadFile(filepath);
        }

        private void GolfButton_Click(object sender, RoutedEventArgs e)
        {
            string filepath = filedir + "\\testDataA4\\golf.in";
            ReadFile(filepath);
        }

        private void GolfcButton_Click(object sender, RoutedEventArgs e)
        {
            string filepath = filedir + "\\testDataA4\\golfc.in";
            ReadFile(filepath);
        }

        private void NotaButton_Click(object sender, RoutedEventArgs e)
        {
            string filepath = filedir + "\\testDataA4\\nota.in";
            ReadFile(filepath);
        }

        private void OrButton_Click(object sender, RoutedEventArgs e)
        {
            string filepath = filedir + "\\testDataA4\\or.in";
            ReadFile(filepath);
        }

        private void Parity3Button_Click(object sender, RoutedEventArgs e)
        {
            string filepath = filedir + "\\testDataA4\\parity3.in";
            ReadFile(filepath);
        }

        private void RestaurantButton_Click(object sender, RoutedEventArgs e)
        {
            string filepath = filedir + "\\testDataA4\\restaurantDecisionTree.in";
            ReadFile(filepath);
        }

        private void SimpleButton_Click(object sender, RoutedEventArgs e)
        {
            string filepath = filedir + "\\testDataA4\\simple.in";
            ReadFile(filepath);
        }

        private void SomeParity3_Click(object sender, RoutedEventArgs e)
        {
            string filepath =filedir + "\\testDataA4\\someparity3.in";
            ReadFile(filepath);
        }

        private void Split_Click(object sender, RoutedEventArgs e)
        {
            string filepath = filedir + "\\testDataA4\\split.in";
            ReadFile(filepath);
        }

        private void TooLittleButton_Click(object sender, RoutedEventArgs e)
        {
            string filepath = filedir + "\\testDataA4\\toolittle.in";
            ReadFile(filepath);
        }

        private void XorButton_Click(object sender, RoutedEventArgs e)
        {
            string filepath = filedir + "\\testDataA4\\xor.in";
            ReadFile(filepath);
        }

        private void XorcButton_Click(object sender, RoutedEventArgs e)
        {
            string filepath = filedir + "\\testDataA4\\xorc.in";
            ReadFile(filepath);
        }

        private void testTree_Click(object sender, RoutedEventArgs e)
        {
            string filepath = filedir + "\\testDataA4\\testTree.in";
            ReadFile(filepath);
        }

        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            //Process.Start("explorer.exe", @"C:\Users");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string filepath = openFileDialog.FileName;
                FileSelected.Text = filepath;
                ReadFile(filepath);
            }


            // FileSelected.Text = openFileDialog1.FileName;

        }

        private void FileSelected_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //get the name of the file from the text box
                string outputFileName = filedir + "\\outputs\\" + OutputFileName.Text;

                //create a file stream and open a file to start writing
                StreamWriter outputStreamWriter = new StreamWriter(outputFileName);


                //try to write the output from tree to a file
                outputStreamWriter.Write(decisionTreeOutput);
                MessageBox.Show(OutputFileName.Text + " successful");
                outputStreamWriter.Close();
                


            }
            catch (Exception ex)
            {
                MessageBox.Show("Failure - file either could not be opened or could not print contents to file");
            }
        }
    }
}
