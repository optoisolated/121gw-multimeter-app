﻿using System;

using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views;
using SkiaSharp.Views.Forms;
using System.Collections.Generic;
using System.Text;
using App_112GW;

namespace rMultiplatform
{
    public class ChartDataEventArgs: EventArgs
    {
        public ChartAxis.AxisOrientation   Orientation;
        public Range NewRange;
        public ChartDataEventArgs(ChartAxis.AxisOrientation pOrientation, Range pNewRange)
        {
            Orientation = pOrientation;
            NewRange = pNewRange;
        }
    };
    public class ChartDataEventReturn
    {
        public delegate double GetCoordinate(double Value);
        public GetCoordinate Function;
        public ChartDataEventReturn(GetCoordinate pFunction)
        {
            Function = pFunction;
        }
    }
    public class ChartData : IChartRenderer
    {
        public delegate ChartDataEventReturn ChartDataEvent(ChartDataEventArgs e);
        public          List<ChartDataEvent> Registrants;

        //
        public void ToCSV()
        {
            if (Data.Count > 1)
            {
                string output = "";
                foreach (var item in Data)
                    output += item.X.ToString() + ", " + item.Y.ToString() + "\r\n";
                Files.SaveFile(output);
            }
        }

        //
        public int      Layer
        {
            get
            {
                return 2;
            }
        }
        public enum     ChartDataMode
        {
            eRolling,
            eRescaling,
            eScreen
        }

        //
        private string  _HorizontalUnits;
        public string   HorizontalUnits
        {
            get { return _HorizontalUnits; }
        }
        private string  _HorizontalLabel;
        public string   HorizontalLabel
{
            get
            {
                return _HorizontalLabel + " ( " + _HorizontalUnits + " )";
            }
            set
            {
                var scts = value.Split('(', ')');
                var txt = scts[0];

                if (scts.Length == 1)
                    throw (new Exception("Must contain units in the following format 'Label(units)'."));

                var units = scts[1].Replace(" ", "");

                if (units.Length == 0)
                    throw (new Exception("Must contain units in the following format 'Label(units)'."));

                _HorizontalUnits = units;
                _HorizontalLabel = txt;
            }
        }

        //
        private string  _VerticalUnits;
        public string   VerticalUnits
        {
            get { return _VerticalUnits; }
        }
        private string  _VerticalLabel;
        public string   VerticalLabel
        {
            get
            {
                return _VerticalLabel + " ( " + _VerticalUnits + " )";
            }
            set
            {
                var scts = value.Split('(', ')');
                var txt = scts[0];

                if (scts.Length == 1)
                    throw (new Exception("Must contain units in the following format 'Label(units)'."));

                var units = scts[1].Replace(" ", "");

                if (units.Length == 0)
                    throw (new Exception("Must contain units in the following format 'Label(units)'."));

                _VerticalUnits = units;
                _VerticalLabel = txt;
            }
        }

        //
        private SKPaint DrawPaint;
        public float    LineWidth
        {
            set{ DrawPaint.StrokeWidth = value; }
        }
        public SKColor  LineColor
        {
            set { DrawPaint.Color = value; }
            get { return DrawPaint.Color; }
        }

        //
        List<SKPoint>   Data;
        public Range    HorozontalSpan;
        public Range    VerticalSpan;
        
        //
        float           Time;
        float           SampleTime;
        float           TimeSpan;
        ChartDataMode   Mode;

        //
        public      ChartData (ChartDataMode pMode, string pHorzLabel, string pVertLabel, float pSampleTime, float pTimeSpan)
        {
            Mode = pMode;
            TimeSpan = pTimeSpan;
            SampleTime = pSampleTime;

            //
            HorizontalLabel = pHorzLabel;
            VerticalLabel = pVertLabel;

            //
            Data = new List<SKPoint>();
            Registrants = new List<ChartDataEvent>();

            //
            var col = Globals.UniqueColor;
            DrawPaint = new SKPaint () { Color = col.ToSKColor(), IsStroke = true, StrokeWidth = 2, IsAntialias = true };

            //
            HorozontalSpan = new Range (0, pTimeSpan);
            VerticalSpan = new Range (0, 0);
        }
        public bool Draw (SKCanvas c)
        {
            if (Data.Count == 0)
                return false;

            if (VerticalSpan == null)
                return false;


                //Scale vertical axis
            var vert = VerticalSpan;
            var horz = HorozontalSpan;

            //Rescale axis
            ChartDataEventReturn x = null, y = null, temp;
            foreach (var axis in Registrants)
            {
                if((temp = axis(new ChartDataEventArgs(ChartAxis.AxisOrientation.Horizontal, horz))) != null)
                    x = temp;
                if ((temp = axis(new ChartDataEventArgs(ChartAxis.AxisOrientation.Vertical, vert))) != null)
                    y = temp;
            }

            //
            if (x == null || y == null)
                throw (new Exception("Graph object must contain an horizontal and vertical axis to plot data."));

            //
            var data = Data.ToArray();
            for (int i = 0; i < data.Length; i++)
            {
                data[i].X = (float)x.Function(data[i].X);
                data[i].Y = (float)y.Function(data[i].Y);
            }

            //
            var path = new SKPath();
            path.AddPoly(data, false);
            c.DrawPath(path, DrawPaint);
            return false;
        }
        public void Sample (float pPoint)
        {
            switch (Mode)
            {
                case ChartDataMode.eRolling:
                    if (Time > HorozontalSpan.Maximum)
                    {
                        Data.RemoveAt(0);
                        HorozontalSpan.ShiftRange(SampleTime);
                    }
                    break;
                case ChartDataMode.eRescaling:
                    HorozontalSpan.RescaleRangeToFitValue(Time);
                    break;
                case ChartDataMode.eScreen:
                    if (Time > HorozontalSpan.Maximum)
                    {
                        Data.Clear();
                        VerticalSpan.Set(0, 0);
                        HorozontalSpan.ShiftRange(HorozontalSpan.Distance);
                    }
                    break;
            };

            //Rescale vertical
            VerticalSpan.RescaleRangeToFitValue(pPoint);

            //
            Data.Add(new SKPoint(Time, pPoint));

            InvalidateParent();
            Time += SampleTime;
        }

        //Required functions by interface defition
        public bool Register (object o)
        {
            if (o.GetType() != typeof(ChartAxis))
                return false;

            //Add the object to registrants after testing whether axis is of relevant units
            var axis = o as ChartAxis;

            bool reg = false;
            if (axis.Orientation == ChartAxis.AxisOrientation.Horizontal)
                if (axis.Label == HorizontalLabel)
                    reg = true;

            if (axis.Orientation == ChartAxis.AxisOrientation.Vertical)
                if (axis.Label == VerticalLabel)
                    reg = true;

            if (reg)
                Registrants.Add(axis.ChartDataEvent);
            return true;
        }
        public List<Type> RequireRegistration()
        {
            var Types = new List<Type>();
            Types.Add(typeof(ChartAxis));
            return Types;
        }
        public void SetParentSize(double w, double h)
        {
            //Doesn't do anything
        }

        //For the sortability of layers
        public int CompareTo(object obj)
        {
            if (obj is IChartRenderer)
            {
                var ob = obj as IChartRenderer;
                var layer = ob.Layer;

                if (layer > Layer)
                    return -1;
                else if (layer < Layer)
                    return 1;
                else
                    return 0;
            }
            return 0;
        }

        private Chart Parent;
        public bool RegisterParent(Object c)
        {
            if (c is Chart)
            {
                Parent = c as Chart;
                return true;
            }
            return false;
        }
        public void InvalidateParent()
        {
            if (Parent != null)
                Parent.InvalidateSurface();
        }
    }
}
