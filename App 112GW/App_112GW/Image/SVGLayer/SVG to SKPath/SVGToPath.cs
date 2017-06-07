﻿using System.Reflection;
using System.IO;
using SkiaSharp;
using System.Xml;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

using Xamarin;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml.Internals;
using System.Xml.Serialization;
using System.Text;

namespace App_112GW
{

    public class SVGToPath
    {
        public List<Polycurve> Curves;
        Polycurve LastCurve
        {
            get
            {
                if (Curves == null)
                    return null;
                if (Curves.Count == 0)
                    return null;

                return Curves[Curves.Count - 1];
            }
        }

        bool ValidCommand(char input)
        {
            char[] cmds = { 'z', 'Z', 'l', 'L', 'h', 'H', 'v', 'V',  'c', 'C', 's', 'S', 'm', 'M', 'q', 'Q', 't', 'T', 'a', 'A'};
            foreach(var cmd in cmds)
            {
                if (cmd == input)
                    return true;
            }
            return false;
        }
        bool HasParameters(char input)
        {
            return (!(input == 'z' || input == 'Z'));
        }


        struct SVGCommand
        {
            static bool     _IsAbsolute(char input)
            {
                return Char.IsUpper(input);
            }
            public bool     IsAbsolute
            {
                get
                {
                    return _IsAbsolute(Command);
                }
            }

            char            Command;
            public char     GetCommand
            {
                get
                {
                    return Char.ToLower(Command);
                }
            }

            public int      GetCommandLength
            {
                get
                {
                    switch(GetCommand)
                    {
                        case 'v':
                        case 'h':
                            return 1;
                        case 'm':
                        case 'l':
                            return 2;
                        case 's':
                            return 4;
                        case 'c':
                            return 6;
                    }
                    return 0;
                }
            }

            int Index;
            public List<float>  GetParameterSet
            {
                get
                {
                    var output = new List<float>();

                    var cmdlen = GetCommandLength;
                    var SetIndex = Index * cmdlen;
                    var SetEnd = SetIndex + cmdlen;

                    if (SetEnd > Parameters.Count)
                        return null;

                    for (int i = SetIndex; i < SetEnd; i++)
                        output.Add(float.Parse(Parameters[i]));

                    if (output.Count == 0)
                        return null;
                    Index++;

                    return output;
                }
            }


            public List<string>    Parameters;
            public SVGCommand(string input)
            {
                Index = 0;
                Command = '\0';
                Parameters = new List<string>();

                if (input.Length > 0)
                {
                    Command = input[0];

                    //Remove first character
                    input = input.Remove(0, 1);

                    //Remove leading space
                    input = input.TrimStart();

                    //Seperate on commas
                    char[] delim = { ',', ' ' };
                    var param = input.Split(delim, StringSplitOptions.RemoveEmptyEntries);

                    //Remove trailing and leading spaces for parameter seperations
                    for (int i = 0; i < param.Length; i++)
                        param[i] = param[i].Trim();

                    //
                    Parameters.AddRange(param);

                    //

                }
                else
                {
                    Debug.WriteLine("Malform svg command");
                }
            }
        }


        SKPoint LastPoint;
        float RepointX(float pPoint, bool pAbsolute)
        {
            if (!pAbsolute)
                pPoint += LastPoint.X;

            LastPoint.X = pPoint;

            pPoint *= 2;

            return pPoint;
        }
        float RepointY(float pPoint, bool pAbsolute)
        {
            if (!pAbsolute)
                pPoint += LastPoint.Y;

            LastPoint.Y = pPoint;

            pPoint *= 2;

            return pPoint;
        }
        SKPoint Repoint(SKPoint pPoint, bool pAbsolute)
        {
            if (!pAbsolute)
            {
                pPoint.X += LastPoint.X;
                pPoint.Y += LastPoint.Y;
            }

            LastPoint = pPoint;

            pPoint.X *= 2;
            pPoint.Y *= 2;
            return pPoint;
        }



        bool ParsePath(string Path)
        {
            var Commands = new List<SVGCommand>();

            int curindex = 0;
            string command = "";
            foreach (var a in Path)
            {
                if ((ValidCommand(a) && curindex != 0))
                {
                    if (command.Length > 0)
                    {
                        Commands.Add(new SVGCommand(command.TrimEnd()));
                        command = "";
                    }
                    else if (HasParameters(a))
                    {
                        throw new Exception("CRAP");
                    }
                }

                command += a;
                curindex++;
                if (Path.Length == curindex)
                {
                    Commands.Add(new SVGCommand(command.TrimEnd()));
                    command = "";
                }
            }



            LastPoint = new SKPoint(0, 0);
            foreach (var cmd in Commands)
            {
                var key = cmd.GetCommand;
                var pars = cmd.Parameters;

                Char splitter = ' ';
                var floatcommands = new List<List<float>>();
                foreach (var par in pars)
                {
                    var parts       = par.Split(splitter);
                    var floatparts  = new List<float>();

                    foreach (var fppart in parts)
                        floatparts.Add(float.Parse(fppart));

                    floatcommands.Add(floatparts);
                }

                //Key boat
                List<float> param;
                switch (key)
                {
                    case 'm':
                        //Process initial move command (It is treated differently)
                        if ((param = cmd.GetParameterSet) != null)
                            Curves.Add(new Polycurve(Repoint(new SKPoint(param[0], param[1]), cmd.IsAbsolute)));

                        //Process subsequent move commands they are treated identically
                        while ((param = cmd.GetParameterSet) != null)
                        {
                            var pt1 = Repoint(new SKPoint(param[0], param[1]), cmd.IsAbsolute);
                            LastCurve.AddLine(pt1);
                        }
                        break;
                    case 'z':
                        LastCurve.CloseCurve();
                        break;
                    case 'l':
                        //Process subsequent move commands they are treated identically
                        while ((param = cmd.GetParameterSet) != null)
                        {
                            var pt1 = Repoint(new SKPoint(param[0], param[1]), cmd.IsAbsolute);
                            LastCurve.AddLine(pt1);
                        }
                        break;
                    case 'h':
                        var y = LastCurve.End.Y;
                        //Process subsequent move commands they are treated identically
                        while ((param = cmd.GetParameterSet) != null)
                        {
                            var pt1 = new SKPoint(RepointX(param[0], cmd.IsAbsolute), y);
                            LastCurve.AddLine(pt1);
                        }
                        
                        break;
                    case 'v':
                        var x = LastCurve.End.X;
                        //Process subsequent move commands they are treated identically
                        while ((param = cmd.GetParameterSet) != null)
                        {
                            var pt1 = new SKPoint(x, RepointY(param[0], cmd.IsAbsolute));
                            LastCurve.AddLine(pt1);
                        }
                        break;
                    case 'c':
                        //Process subsequent move commands they are treated identically
                        var ind = 0;
                        while ((param = cmd.GetParameterSet) != null)
                        {
                            var pt1 = Repoint(new SKPoint(param[ind++], param[ind++]), cmd.IsAbsolute);
                            var pt2 = Repoint(new SKPoint(param[ind++], param[ind++]), cmd.IsAbsolute);
                            var pt3 = Repoint(new SKPoint(param[ind++], param[ind++]), cmd.IsAbsolute);
                            LastCurve.AddCubic(pt1, pt2, pt3);
                            ind = 0;
                        }
                        break;
                    case 's':

                        break;
                }
            }
            return true;
        }

        int runcount = 0;
        bool ProcessSVG(string Name, Stream Stream)
        {
            if (runcount++ == 41)
            {
                var xdoc = new System.Xml.Linq.XDocument();
                xdoc = System.Xml.Linq.XDocument.Load(Stream);
                var temp = xdoc.Descendants();

                foreach (var t in temp)
                {
                    if (t.Name.ToString().Contains("path"))
                        foreach (var at in t.Attributes())
                            if (at.Name == "d")
                                ParsePath(at.Value);
                }
            }
            return true;
        }


        enum Coordinate
        {
            Absolute,
            Relative
        }
        public  SVGToPath(string pInput)
        {
            //Convert commands to a path
            Curves = new List<Polycurve>();
            var loader = new GeneralLoader(ProcessSVG, "svg");
        }
    }
}