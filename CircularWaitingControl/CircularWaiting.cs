using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CircularWaitingControl
{
    public class CircularWaiting : Shape
    {
        private bool _isRegress;
        private readonly DispatcherTimer _timer;

        public CircularWaiting()
        {
            _timer = new DispatcherTimer();
            _timer.Tick += TimerOnTick;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
        }

        #region Value

        private static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(CircularWaiting),
                new FrameworkPropertyMetadata(99.999, FrameworkPropertyMetadataOptions.AffectsRender, null, CoerceValue));

        private double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static object CoerceValue(DependencyObject depObj, object baseVal)
        {
            double val = (double)baseVal;
            //val = Math.Min(val, 99.999);
            //val = Math.Max(val, 0.0);
            return val;
        }

        #endregion

        #region Start 

        private static readonly DependencyProperty StartProperty =
            DependencyProperty.Register(nameof(Start), typeof(bool), typeof(CircularWaiting), new PropertyMetadata(false, StartChanged));

        public bool Start
        {
            get { return (bool)GetValue(StartProperty); }
            set { SetValue(StartProperty, value); }
        }

        private static void StartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cw = d as CircularWaiting;
            if (cw == null) return;
            if ((bool) e.NewValue)
            {
                cw.Value = 0.0;
                cw._timer.Start();
            }
            else
            {
                cw._timer.Stop();
                cw.Value = 99.999;
            }
        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            if (Value + 1.0 >= 100.0)
            {
                _isRegress = !_isRegress;
                Value = 0.0;
                return;
            }

            Value += 1.0;
        }

        #endregion

        #region Speed

        public static readonly DependencyProperty SpeedProperty =
            DependencyProperty.Register(nameof(Speed), typeof(int), typeof(CircularWaiting), new PropertyMetadata(0, SpeedChanged));
        
        public int Speed
        {
            get { return (int)GetValue(SpeedProperty); }
            set { SetValue(SpeedProperty, value); }
        }

        private static void SpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        #endregion

        protected override Geometry DefiningGeometry
        {
            get
            {
                double startAngle;
                double endAngle;
                if (_isRegress)
                {
                    startAngle = 90.0 - (Value / 100.0) * 360.0;
                    endAngle = -270.0;
                }
                else
                {
                    startAngle = 90.0;
                    endAngle = 90.0 - (Value / 100.0) * 360.0;
                }

                double maxWidth = Math.Max(0.0, RenderSize.Width - StrokeThickness);
                double maxHeight = Math.Max(0.0, RenderSize.Height - StrokeThickness);

                double xStart = maxWidth / 2.0 * Math.Cos(startAngle * Math.PI / 180.0);
                double yStart = maxHeight / 2.0 * Math.Sin(startAngle * Math.PI / 180.0);

                double xEnd = maxWidth / 2.0 * Math.Cos(endAngle * Math.PI / 180.0);
                double yEnd = maxHeight / 2.0 * Math.Sin(endAngle * Math.PI / 180.0);

                StreamGeometry geometry = new StreamGeometry();
                using (StreamGeometryContext ctx = geometry.Open())
                {
                    ctx.BeginFigure(
                        new Point((RenderSize.Width / 2.0) + xStart, (RenderSize.Height / 2.0) - yStart), true, false);
                    ctx.ArcTo(
                        new Point((RenderSize.Width / 2.0) + xEnd, (RenderSize.Height / 2.0) - yEnd),
                        new Size(maxWidth / 2.0, maxHeight / 2), 0.0, (startAngle - endAngle) > 180, SweepDirection.Clockwise, true, false);
                }

                return geometry;
            }
        }
    }
}
