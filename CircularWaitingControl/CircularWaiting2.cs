using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CircularWaitingControl
{
    public class CircularWaiting2 : Shape
    {
        private bool _isRegress;
        private readonly DispatcherTimer _timer;
        private int _startAngle;
        private int _endAngle;

        public CircularWaiting2()
        {
            _timer = new DispatcherTimer();
            _timer.Tick += TimerOnTick;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
  
            _startAngle = 0;
            _endAngle = 90;
        }

        #region Value

        private static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(bool), typeof(CircularWaiting2), 
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        private bool Value
        {
            get { return (bool)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        #endregion

        #region Start 

        private static readonly DependencyProperty StartProperty =
            DependencyProperty.Register(nameof(Start), typeof(bool), typeof(CircularWaiting2), new PropertyMetadata(false, StartChanged));

        public bool Start
        {
            get { return (bool)GetValue(StartProperty); }
            set { SetValue(StartProperty, value); }
        }

        private static void StartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cw = d as CircularWaiting2;
            if (cw == null) return;
            if ((bool)e.NewValue)
                cw._timer.Start();
            else
                cw._timer.Stop();
        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            Value = !Value;
        }

        private int StartAngle
        {
            get { return _startAngle;}
            set { _startAngle = value % 360; }
        }

        private int EndAngle
        {
            get { return _endAngle; }
            set { _endAngle = value % 360; }
        }

        #endregion

        protected override Geometry DefiningGeometry
        {
            get
            {
                if (!_isRegress)
                {
                    StartAngle = StartAngle - 5;
                    EndAngle = EndAngle - 2;
                    if (ArcLength() > 350)
                        _isRegress = !_isRegress;
                }
                else
                {
                    EndAngle = EndAngle - 5;
                    StartAngle = StartAngle - 2;
                    if (ArcLength() < 30)
                        _isRegress = !_isRegress;
                }

                double maxWidth = Math.Max(0.0, RenderSize.Width - StrokeThickness);
                double maxHeight = Math.Max(0.0, RenderSize.Height - StrokeThickness);

                double xStart = maxWidth / 2.0 * Math.Cos(ConvertToRadian(StartAngle));
                double yStart = maxHeight / 2.0 * Math.Sin(ConvertToRadian(StartAngle));

                double xEnd = maxWidth / 2.0 * Math.Cos(ConvertToRadian(EndAngle));
                double yEnd = maxHeight / 2.0 * Math.Sin(ConvertToRadian(EndAngle));
                
                var startPoint = new Point((RenderSize.Width / 2.0) + xStart, (RenderSize.Height / 2.0) - yStart);
                var endPoint = new Point((RenderSize.Width / 2.0) + xEnd, (RenderSize.Height / 2.0) - yEnd);
                var arcSize = new Size(maxWidth / 2.0, maxHeight / 2.0);

                StreamGeometry geometry = new StreamGeometry();
                using (StreamGeometryContext ctx = geometry.Open())
                {
                    ctx.BeginFigure(endPoint, true, false);
                    ctx.ArcTo(startPoint, arcSize, 0.0, ArcLength() > 180, SweepDirection.Clockwise, true, false);
                }

                return geometry;
            }
        }

        private double ConvertToRadian(double angle)
        {
            return angle * Math.PI / 180.0;
        }

        private double ArcLength()
        {
            return (EndAngle - StartAngle + 360) % 360;
        }
    }
}
