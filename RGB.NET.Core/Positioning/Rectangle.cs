﻿// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RGB.NET.Core
{
    /// <summary>
    /// Represents a rectangle defined by it's position and it's size.
    /// </summary>
    [DebuggerDisplay("[Location: {Location}, Size: {Size}]")]
    public class Rectangle : AbstractBindable
    {
        #region Properties & Fields

        private Point _location;
        /// <summary>
        /// Gets or sets the <see cref="Point"/> representing the top-left corner of the <see cref="Rectangle"/>.
        /// </summary>
        public Point Location
        {
            get { return _location; }
            set
            {
                if (SetProperty(ref _location, value))
                    // ReSharper disable once ExplicitCallerInfoArgument
                    OnPropertyChanged(nameof(Center));
            }
        }

        private Size _size;
        /// <summary>
        /// Gets or sets the <see cref="Size"/> of the <see cref="Rectangle"/>.
        /// </summary>
        public Size Size
        {
            get { return _size; }
            set
            {
                if (SetProperty(ref _size, value))
                    // ReSharper disable once ExplicitCallerInfoArgument
                    OnPropertyChanged(nameof(Center));
            }
        }

        /// <summary>
        /// Gets a new <see cref="Point"/> representing the center-point of the <see cref="Rectangle"/>.
        /// </summary>
        public Point Center => new Point(Location.X + (Size.Width / 2.0), Location.Y + (Size.Height / 2.0));

        /// <summary>
        /// Gets a bool indicating if both, the width and the height of the rectangle is greater than zero.
        /// </summary>
        public bool IsEmpty => (Size.Width > DoubleExtensions.TOLERANCE) && (Size.Height > DoubleExtensions.TOLERANCE);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> class.
        /// </summary>
        public Rectangle()
            : this(new Point(), new Size())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> class using the provided values for <see cref="Location"/> ans <see cref="Size"/>.
        /// </summary>
        /// <param name="x">The <see cref="Point.X"/>-position of this <see cref="Rectangle"/>.</param>
        /// <param name="y">The <see cref="Point.Y"/>-position of this <see cref="Rectangle"/>.</param>
        /// <param name="width">The <see cref="Core.Size.Width"/> of this <see cref="Rectangle"/>.</param>
        /// <param name="height">The <see cref="Core.Size.Height"/> of this <see cref="Rectangle"/>.</param>
        public Rectangle(double x, double y, double width, double height)
            : this(new Point(x, y), new Size(width, height))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> class using the given <see cref="Point"/> and <see cref="Core.Size"/>.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="size"></param>
        public Rectangle(Point location, Size size)
        {
            this.Location = location;
            this.Size = size;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> class using the given array of <see cref="Rectangle"/>.
        /// The <see cref="Location"/> and <see cref="Size"/> is calculated to completely contain all rectangles provided as parameters.
        /// </summary>
        /// <param name="rectangles">The array of <see cref="Rectangle"/> used to calculate the <see cref="Location"/> and <see cref="Size"/></param>
        public Rectangle(params Rectangle[] rectangles)
            : this(rectangles.AsEnumerable())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> class using the given list of <see cref="Rectangle"/>.
        /// The <see cref="Location"/> and <see cref="Size"/> is calculated to completely contain all rectangles provided as parameters.
        /// </summary>
        /// <param name="rectangles">The list of <see cref="Rectangle"/> used to calculate the <see cref="Location"/> and <see cref="Size"/></param>
        public Rectangle(IEnumerable<Rectangle> rectangles)
        {
            double posX = double.MaxValue;
            double posY = double.MaxValue;
            double posX2 = double.MinValue;
            double posY2 = double.MinValue;

            foreach (Rectangle rectangle in rectangles)
            {
                posX = Math.Min(posX, rectangle.Location.X);
                posY = Math.Min(posY, rectangle.Location.Y);
                posX2 = Math.Max(posX2, rectangle.Location.X + rectangle.Size.Width);
                posY2 = Math.Max(posY2, rectangle.Location.Y + rectangle.Size.Height);
            }

            InitializeFromPoints(new Point(posX, posY), new Point(posX2, posY2));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> class using the given array of <see cref="Point"/>.
        /// The <see cref="Location"/> and <see cref="Size"/> is calculated to contain all points provided as parameters.
        /// </summary>
        /// <param name="points">The array of <see cref="Point"/> used to calculate the <see cref="Location"/> and <see cref="Size"/></param>
        public Rectangle(params Point[] points)
            : this(points.AsEnumerable())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> class using the given list of <see cref="Point"/>.
        /// The <see cref="Location"/> and <see cref="Size"/> is calculated to contain all points provided as parameters.
        /// </summary>
        /// <param name="points">The list of <see cref="Point"/> used to calculate the <see cref="Location"/> and <see cref="Size"/></param>
        public Rectangle(IEnumerable<Point> points)
            : this()
        {
            double posX = double.MaxValue;
            double posY = double.MaxValue;
            double posX2 = double.MinValue;
            double posY2 = double.MinValue;

            foreach (Point point in points)
            {
                posX = Math.Min(posX, point.X);
                posY = Math.Min(posY, point.Y);
                posX2 = Math.Max(posX2, point.X);
                posY2 = Math.Max(posY2, point.Y);
            }

            InitializeFromPoints(new Point(posX, posY), new Point(posX2, posY2));
        }

        #endregion

        #region Methods

        private void InitializeFromPoints(Point point1, Point point2)
        {
            double posX = Math.Min(point1.X, point2.X);
            double posY = Math.Min(point1.Y, point2.Y);
            double width = Math.Max(point1.X, point2.X) - posX;
            double height = Math.Max(point1.Y, point2.Y) - posY;

            Location = new Point(posX, posY);
            Size = new Size(width, height);
        }

        /// <summary>
        /// Calculates the percentage of intersection of a rectangle.
        /// </summary>
        /// <param name="intersectingRect">The intersecting rectangle.</param>
        /// <returns>The percentage of intersection.</returns>
        public double CalculateIntersectPercentage(Rectangle intersectingRect)
        {
            if (IsEmpty || intersectingRect.IsEmpty) return 0;

            Rectangle intersection = CalculateIntersection(intersectingRect);
            return intersection.IsEmpty ? 0 : (intersection.Size.Width * intersection.Size.Height) / (Size.Width * Size.Height);
        }

        /// <summary>
        /// Calculates the <see cref="Rectangle"/> representing the intersection of this <see cref="Rectangle"/> and the one provided as parameter.
        /// </summary>
        /// <param name="intersectingRectangle">The intersecting <see cref="Rectangle"/></param>
        /// <returns>A new <see cref="Rectangle"/> representing the intersection this <see cref="Rectangle"/> and the one provided as parameter.</returns>
        public Rectangle CalculateIntersection(Rectangle intersectingRectangle)
        {
            double x1 = Math.Max(Location.X, intersectingRectangle.Location.X);
            double x2 = Math.Min(Location.X + Size.Width, intersectingRectangle.Location.X + intersectingRectangle.Size.Width);

            double y1 = Math.Max(Location.Y, intersectingRectangle.Location.Y);
            double y2 = Math.Min(Location.Y + Size.Height, intersectingRectangle.Location.Y + intersectingRectangle.Size.Height);

            if ((x2 >= x1) && (y2 >= y1))
                return new Rectangle(x1, y1, x2 - x1, y2 - y1);

            return new Rectangle();
        }

        /// <summary>
        /// Determines if the specified <see cref="Point"/> is contained within this <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="point">The <see cref="Point"/> to test.</param>
        /// <returns></returns>
        public bool Contains(Point point)
        {
            return Contains(point.X, point.Y);
        }

        /// <summary>
        /// Determines if the specified location is contained within this <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="x">The X-location to test.</param>
        /// <param name="y">The Y-location to test.</param>
        /// <returns></returns>
        public bool Contains(double x, double y)
        {
            return (Location.X <= x) && (x < (Location.X + Size.Width)) && (Location.Y <= y) && (y < (Location.Y + Size.Height));
        }

        /// <summary>
        /// Determines if the specified <see cref="Rectangle"/> is contained within this <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="rect">The <see cref="Rectangle"/> to test.</param>
        /// <returns></returns>
        public bool Contains(Rectangle rect)
        {
            return (Location.X <= rect.Location.X) && ((rect.Location.X + rect.Size.Width) <= (Location.X + Size.Width))
                   && (Location.Y <= rect.Location.Y) && ((rect.Location.Y + rect.Size.Height) <= (Location.Y + Size.Height));
        }

        /// <summary>
        /// Converts the <see cref="Location"/>- and <see cref="Size"/>-position of this <see cref="Rectangle"/> to a human-readable string.
        /// </summary>
        /// <returns>A string that contains the <see cref="Location"/> and <see cref="Size"/>  of this <see cref="Rectangle"/>. For example "[Location: [X: 100, Y: 10], Size: [Width: 20, Height: [40]]".</returns>
        public override string ToString()
        {
            return $"[Location: {Location}, Size: {Size}]";
        }

        /// <summary>
        /// Tests whether the specified object is a <see cref="Rectangle" /> and is equivalent to this <see cref="Rectangle" />.
        /// </summary>
        /// <param name="obj">The object to test.</param>
        /// <returns><c>true</c> if <paramref name="obj" /> is a <see cref="Rectangle" /> equivalent to this <see cref="Rectangle" />; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            Rectangle compareRect = obj as Rectangle;
            if (ReferenceEquals(compareRect, null))
                return false;

            if (ReferenceEquals(this, compareRect))
                return true;

            if (GetType() != compareRect.GetType())
                return false;

            return (Location == compareRect.Location) && (Size == compareRect.Size);
        }

        /// <summary>
        /// Returns a hash code for this <see cref="Rectangle" />.
        /// </summary>
        /// <returns>An integer value that specifies the hash code for this <see cref="Rectangle" />.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Location.GetHashCode();
                hashCode = (hashCode * 397) ^ Size.GetHashCode();
                return hashCode;
            }
        }

        #endregion

        #region Operators

        /// <summary>
        /// Returns a value that indicates whether two specified <see cref="Rectangle" /> are equal.
        /// </summary>
        /// <param name="rectangle1">The first <see cref="Rectangle" /> to compare.</param>
        /// <param name="rectangle2">The second <see cref="Rectangle" /> to compare.</param>
        /// <returns><c>true</c> if <paramref name="rectangle1" /> and <paramref name="rectangle2" /> are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(Rectangle rectangle1, Rectangle rectangle2)
        {
            return ReferenceEquals(rectangle1, null) ? ReferenceEquals(rectangle2, null) : rectangle1.Equals(rectangle2);
        }

        /// <summary>
        /// Returns a value that indicates whether two specified <see cref="Rectangle" /> are equal.
        /// </summary>
        /// <param name="rectangle1">The first <see cref="Rectangle" /> to compare.</param>
        /// <param name="rectangle2">The second <see cref="Rectangle" /> to compare.</param>
        /// <returns><c>true</c> if <paramref name="rectangle1" /> and <paramref name="rectangle2" /> are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(Rectangle rectangle1, Rectangle rectangle2)
        {
            return !(rectangle1 == rectangle2);
        }

        #endregion
    }
}
