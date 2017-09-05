﻿using System;
using System.Collections;
using System.Text;
using Xamarin.Forms;
using System.Diagnostics;
using App_112GW;
using SkiaSharp;
using Xamarin.Forms.PlatformConfiguration;
using System.Collections.Generic;

namespace rMultiplatform
{
    class MathChart : StackLayout
    {
        private string x_label = "Time (s)";
        private string y_label = "Volts (V)";

        ChartAxis VerticalAxis = null;
        ChartAxis HorizontalAxis = null;

        public string VerticalLabel
        {
            set
            {
                if (VerticalAxis != null)
                    VerticalAxis.Label = value;
            }
        }
        public string HorozontalLabel
        {
            set
            {
                if (HorizontalAxis != null)
                    HorizontalAxis.Label = value;
            }
        }
        
        private static float SqrA_x_B(float A, float B)
        {
            return (A * A) * B;
        }
        private static float SqrA_Div_B(float A, float B)
        {
            return (A * A) / B;
        }
        private static float A_Div_SqrB(float A, float B)
        {
            return A / (B * B);
        }
        private static float Division(float A, float B)
        {
            return A / B;
        }
        private static float Addition(float A, float B)
        {
            return A + B;
        }
        private static float Subtraction(float A, float B)
        {
            return A - B;
        }
        private static float Multiplication(float A, float B)
        {
            return A * B;
        }
        public delegate float Operation(float A, float B);

        public class OperationItem
        {
            public string Label { get; set; }
            public Operation Function { get; set; }
            public Color Color { get; set; }

            public OperationItem(string pLabel, Operation pFunction)
            {
                Label = pLabel;
                Function = pFunction;

                Color = Globals.BackgroundColor;
            }
        }
        List<OperationItem> Operations = new List<OperationItem>()
        {
            new OperationItem( "A + B",              Addition        ),
            new OperationItem( "A - B",              Subtraction     ),
            new OperationItem( "A x B",              Multiplication  ),
            new OperationItem( "A / B",              Division        ),
            new OperationItem( "A\u00b2 x B",        SqrA_x_B        ),
            new OperationItem( "A\u00b2 / B",        SqrA_Div_B      ),
            new OperationItem( "A / B\u00b2",        A_Div_SqrB      )
        };
        enum Current
        {
            L1,
            L2
        };

        Operation Current_Operation = null;

        Picker A_List         = new Picker();
        Picker B_List         = new Picker();
        Picker Operation_List = new Picker();

        public IList SourceA
        {
            set
            {
                A_List.ItemsSource = value;
            }
            private get
            {
                return A_List.ItemsSource;
            }
        }
        public IList SourceB
        {
            set
            {
                B_List.ItemsSource = value;
            }
            private get
            {
                return B_List.ItemsSource;
            }
        }

       
        SKPoint Interpolate(SKPoint A, SKPoint B, float X)
        {
            var mx = (B.X - A.X);
            var my = (B.Y - A.Y);
            var bx = A.X;
            var by = A.Y;

            var dx = (X - A.X);
            var rx = dx / mx;

            var valx = mx * rx + bx;
            var valy = my * rx + by;
            return new SKPoint(valx, valy);
        }

        private Range VerticalRange = new Range(0 , 0);
        private int i1 = 1, i2 = 1;
        private float x_val, y_val1, y_val2;
        void Resample(List<SKPoint> L1, List<SKPoint> L2)
        {
            var l1_count = L1.Count;
            var l2_count = L2.Count;

            //
            if (Current_Operation != null)
            {
                if ((i1 >= l1_count) || (i2 >= l2_count))
                    Rerange();

                if ((l1_count > 1) && (l2_count > 1))
                {
                    while ((i1 < l1_count) && (i2 < l2_count))
                    {
                        var p1 = L1[i1];
                        var p2 = L2[i2];
                        var x1 = p1.X;
                        var x2 = p2.X;

                        if (x1 > x2)
                        {
                            x_val = x2;
                            y_val2 = p2.Y;

                            //Interpolate a point for L1
                            var P1 = L1[i1 - 1];
                            var P2 = L1[i1];
                            var pt = Interpolate(P1, P2, x2);
                            y_val1 = pt.Y;
                            ++i2;
                        }
                        else
                        {
                            x_val = x1;
                            y_val1 = p1.Y;

                            //Interpolate a point for L2
                            var P1 = L2[i2 - 1];
                            var P2 = L2[i2];
                            var pt = Interpolate(P1, P2, x1);
                            y_val2 = pt.Y;
                            ++i1;
                        }
                        var op_result = Current_Operation(y_val1, y_val2);
                        VerticalRange.RescaleRangeToFitValue(op_result);
                        Data.Add(new SKPoint(x_val, op_result));
                    }
                    ChartData.Set(Data, VerticalRange);
                }
            }
        }

        private void Rerange()
        {
            VerticalRange.Rescale();
            i1 = 1; i2 = 1;
            Data.Clear();
        }

        List<SKPoint> Data = new List<SKPoint>();
        Multimeter DeviceA = null, DeviceB = null;
        private void A_List_ItemSelected(object sender, EventArgs e)
        {
            Rerange();
            var item = (sender as Picker).SelectedItem;
            if (item != null)
            {
                if (DeviceA != null)
                    DeviceA.Plot.DataChanged -= DataA_Changed;
                DeviceA = item as Multimeter;
                DeviceA.Plot.DataChanged += DataA_Changed;
            }
        }
        private void B_List_ItemSelected(object sender, EventArgs e)
        {
            Rerange();
            var item = (sender as Picker).SelectedItem;
            if (item != null)
            {
                if (DeviceB != null)
                    DeviceB.Plot.DataChanged -= DataB_Changed;
                DeviceB = item as Multimeter;
                DeviceB.Plot.DataChanged += DataB_Changed;
            }
        }

        private void Operation_List_ItemSelected(object sender, EventArgs e)
        {
            Rerange();
            var sel_item = (sender as Picker).SelectedItem;
            var sel_obj = sel_item as Object;
            var sel_item_type = ((OperationItem)sel_item);
            Current_Operation = sel_item_type.Function;
            VerticalLabel = "(" + sel_item_type.Label + ")";
        }

        private void DataA_Changed(List<SKPoint> Data)
        {
            Resample(Data, DeviceB.Data.Data);
        }
        private void DataB_Changed(List<SKPoint> Data)
        {
            Resample(DeviceA.Data.Data, Data);
        }

        private Grid SelectGrid = new Grid();
        public ChartData ChartData;
        public Chart Plot;

        private void AddSelectView(View pInput, int pX, int pY, int pXSpan = 1, int pYSpan = 1)
        {
            SelectGrid.Children.Add(pInput, pX, pX + pXSpan, pY, pY + pYSpan);
        }

        static ColumnDefinition MakeMinimalColumn()
        {
            return new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) };
        }
        static ColumnDefinition MakeFillColumn()
        {
            return new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };
        }

        static RowDefinition MakeMinimalRow()
        {
            return new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) };
        }
        static RowDefinition MakeFillRow()
        {
            return new RowDefinition { Height = new GridLength(1, GridUnitType.Star) };
        }

        private int RowSpan
        {
            get
            {
                return SelectGrid.RowDefinitions.Count;
            }
        }
        private int ColumnSpan
        {
            get
            {
                return SelectGrid.ColumnDefinitions.Count;
            }
        }

        static LayoutOptions ColumnLayout = LayoutOptions.Fill;
        static Picker MakePicker( EventHandler SelectedHandler, string Title, string BindText)
        {
            var output = new Picker();
            output.Title = Title;
            output.VerticalOptions = LayoutOptions.StartAndExpand;
            output.HorizontalOptions = ColumnLayout;
            output.SelectedIndexChanged += SelectedHandler;

            output.ItemDisplayBinding = new Binding(BindText);

            //TODO : This is a bug in xamarin, row height need to be made automatic
            return output;
        }

        public MathChart()
        {
            //Basic default constants, these should be globalised eventually
            Padding             = 0;
            HorizontalOptions   = LayoutOptions.Fill;
            VerticalOptions     = LayoutOptions.Fill;
            BackgroundColor     = Globals.BackgroundColor;

            //Setup listviews
            A_List = MakePicker(A_List_ItemSelected, "Device A", "ShortId");
            B_List = MakePicker(B_List_ItemSelected, "Device B", "ShortId");

            //Operations list is bound to operations
            Operation_List = MakePicker(Operation_List_ItemSelected, "Operation", "Label");
            Operation_List.ItemsSource = Operations;

            ContentView OperationSelect;
            //Sets up operator table
            {
                OperationSelect = new ContentView();
                SelectGrid.ColumnDefinitions.Add(MakeFillColumn());
                SelectGrid.ColumnDefinitions.Add(MakeFillColumn());
                SelectGrid.ColumnDefinitions.Add(MakeFillColumn());
                SelectGrid.RowDefinitions.Add(MakeMinimalRow());

                int Column_Count    = 0;
                int A_Column        = Column_Count++;
                int OP_Column       = Column_Count++;
                int B_Column        = Column_Count++;
                int Current_Row     = 0;
                var Header_Row      = Current_Row++;
                var Data_Row        = Current_Row++;

                AddSelectView(A_List, A_Column, Data_Row);
                AddSelectView(Operation_List, OP_Column, Data_Row);
                AddSelectView(B_List, B_Column, Data_Row);

                OperationSelect.VerticalOptions = LayoutOptions.Start;
                OperationSelect.Content = SelectGrid;
            }

            //Sets up Plot
            {
                ChartData = new ChartData(ChartData.ChartDataMode.eRescaling, x_label, y_label, 10f);
                Plot = new Chart() { Padding = new ChartPadding(0.1f) };
                Plot.AddGrid(new ChartGrid());
                Plot.AddAxis(HorizontalAxis = new ChartAxis(5, 5, 0, 20) { Label = x_label, Orientation = ChartAxis.AxisOrientation.Horizontal, LockToAxisLabel = y_label, LockAlignment = ChartAxis.AxisLock.eEnd, ShowDataKey = false });
                Plot.AddAxis(VerticalAxis = new ChartAxis(5, 5, 0, 0) { Label = y_label, Orientation = ChartAxis.AxisOrientation.Vertical, LockToAxisLabel = x_label, LockAlignment = ChartAxis.AxisLock.eStart, ShowDataKey = false });
                Plot.AddData(ChartData);
                Plot.FullscreenClicked += Plot_FullscreenClicked;

                Plot.VerticalOptions = LayoutOptions.FillAndExpand;
            }

            //1 Column, 2 Rows
            Children.Add(OperationSelect);
            Children.Add(Plot);
        }

        private bool Fullscreen = true;
        private void Plot_FullscreenClicked(object sender, EventArgs e)
        {
            foreach (var item in Children)
            {
                if (Fullscreen)
                {
                    if (item.GetType() != typeof(Chart))
                        item.IsVisible = false;
                }
                else
                {
                    if (item.GetType() != typeof(Chart))
                        item.IsVisible = true;
                }
            }
            Fullscreen = !Fullscreen;
        }
    }
}