using System;
using Android.Graphics;

namespace Exercise_One.Droid.Common
{
    public enum Shape
    {
        Square,
        Circle,
    }    

    public class Utility
    {
        #region <-PrivateStaticMembers->
        private static Random _randomizer = new Random();
        #endregion

        #region <-PublicMethods->
        public static Color GetRandomColor()
        {
            try
            {
                var randomizer = new Random();
                return Color.Argb(255, randomizer.Next(256), randomizer.Next(256),
                                   randomizer.Next(256));
            }
            catch (Exception)
            {
            }
            return Color.White;

        }

		public static Shape GetRandomShape()
		{
			Array values = Enum.GetValues(typeof(Shape));
			Shape randomShape = (Shape)values.GetValue(_randomizer.Next(values.Length));
			return randomShape;
		}
        #endregion
    }
}