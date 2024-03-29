﻿using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace CurvePlus
{
    public class CurvePlusInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "CurvePlus";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return Properties.Resources.CurvePlusLogo24;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "Curve Plus is a collection of components that both expose Rhino curve methods and provides a selection of new methods.";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("ab81fea9-8d16-4caf-af89-2736c660f36d");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "David Mans";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }

        public override string AssemblyVersion
        {
            get
            {
                return "1.7.0.0";
            }
        }
    }
}
