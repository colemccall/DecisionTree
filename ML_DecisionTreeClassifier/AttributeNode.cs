﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML_DecisionTreeClassifier
{
    public class AttributeNode
    {
        //Variables within each node
        public string classLabel;
        public char dataType;
        public string word;
        public int integer;
        public double continuous;

        public AttributeNode(string classLabel, char dataType, string data)
        {
            this.classLabel = classLabel;
            this.dataType = dataType;
            this.word = data;
        }

        public AttributeNode(string classLabel, char dataType, int data)
        {
            this.classLabel = classLabel;
            this.dataType = dataType;
            this.integer = data;
        }

        public AttributeNode(string classLabel, char dataType, double data)
        {
            this.classLabel = classLabel;
            this.dataType = dataType;
            this.continuous = data;
        }

        public AttributeNode()
        {
            classLabel = null;
            word = null;
           
        }
    }
}
