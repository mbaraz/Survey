using System;
using System.Linq;

namespace SurveyModel.Logic
{
    public static class LogicalUtil
    {
        public static readonly string[] Syndetics = new[] { string.Empty, "И", "ИЛИ" };
        private static readonly string[] Signs = new[] {"=", "≠", ">", "<", "≥", "≤"};

		public static bool checkInequality(string sign, int [] codesArray, int value)
        {
			var signIndex = Array.IndexOf(Signs, sign);
			var result = false;
			switch (signIndex) {
				case 0 :
					result = checkET(codesArray, value);
					break;
				case 1 :
					result = checkNET(codesArray, value);
					break;
				case 2 :
					result = checkGT(codesArray, value);
					break;
				case 3 :
					result = checkLT(codesArray, value);
					break;
				case 4 :
					result = checkNLT(codesArray, value);
					break;
				case 5 :
					result = checkNGT(codesArray, value);
                    break;
			}
			return result;
		}
		
		public static bool checkSyndetic(string syndetic, bool result, bool value)
        {
			var syndeticIndex = Array.IndexOf(Syndetics, syndetic);
			switch (syndeticIndex) {
				case 0 :
					result = value;
					break;
				case 1 :
					result = result && value;
					break;
				case 2 :
					result = result || value;
                    break;
			}
			return result;
		}
		
		public static bool checkGT(int [] arr, int value)
        {
			return !arr.Any() || arr[arr.Length - 1] > value;
		}
		
		public static bool checkNLT(int [] arr, int value)
        {
			return !arr.Any() || arr[arr.Length - 1] >= value;
		}
		
		public static bool checkLT(int [] arr, int value)
        {
			return !arr.Any() || arr[0] < value;
		}
		
		public static bool checkNGT(int [] arr, int value)
        {
			return !arr.Any() || arr[0] <= value;
		}
		
		public static bool checkET(int [] arr, int value)
        {
			var result = false;
			for (var i = 0; i < arr.Length && !result; i++) {
				if (value < arr[i])
					break;
				result = arr[i] == value;
			}
			return !arr.Any() || result;
		}
		
		public static bool checkNET(int [] arr, int value)
        {
			return !arr.Any() || !checkET(arr, value);
		}
/*		
		private int findIndex(int [] arr, string value)
        {
			var result = 0;
			while (arr[result] != value)
				result++;

			return result;
		}*/
    }
}