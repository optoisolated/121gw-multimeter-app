﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using SkiaSharp;

namespace rMultiplatform
{
    public class SmartChartLogger
    {
        public enum LoggerMode
        {
            Rolling,
            Rescaling,
            Screen
        }

        private float FrameLength = 10; //in ms
        private LoggerMode Mode = LoggerMode.Rescaling;
        private DateTime DataStart = DateTime.Now;
        public ObservableCollection<SKPoint> Data;

        public void Reset()
        {
            Data.Clear();
            DataStart = DateTime.Now;
        }
        public void Sample(float pValue)
        {
            TimeSpan t_diff = DateTime.Now.Subtract(DataStart);
            float ms_diff = (float)t_diff.TotalMilliseconds / 1000;
            switch (Mode)
            {
                case LoggerMode.Rolling:
                    if (ms_diff > FrameLength)
                        if (Data.Count > 0)
                            Data.RemoveAt(0);
                    break;
                case LoggerMode.Screen:
                    if (ms_diff > FrameLength)
                        Data.Clear();
                    break;
                case LoggerMode.Rescaling:
                    break;
            };
            Data.Add(new SKPoint(ms_diff, pValue));
        }

        public SmartChartLogger(float pFrameLength, LoggerMode pMode)
        {
            FrameLength = pFrameLength;
            Mode = pMode;
            Data = new ObservableCollection<SKPoint>();
        }
    }
}