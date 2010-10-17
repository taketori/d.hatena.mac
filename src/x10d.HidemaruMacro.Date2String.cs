// dateParse.cs
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace x10d.HidemaruMacro {
///<example>
///このクラスの使い方。
///</example>
///<code>
///using x10d.HidemaruMacro;
///class SampleOfStringToDateTime {
///	static void Main () {
///		string[] ary1 = {"", "5 ", "05 ", "15 ", "215 ", "205 ", "0215 ", "1215 ", "90205 ", "090205 ", "091215 "};
///		string[] ary2 = {"", "21:", "21:34"};
///		foreach(string h in ary2) {
///			foreach(string d in ary1) {
///				Console.Write(d + "\t" + h + " =\t");
///				Console.WriteLine(new StringToDateTime(d + h).dateTime.ToString("s"));
///			}
///		}
///	}
///}
///</code>
	public class StringToDateTime {
		readonly public DateTime dateTime;

		public StringToDateTime() {
			dateTime = DateTime.Now;
		}

		public StringToDateTime(string dtstr) {
		//TODO: 曜日。月末。日にちを25thというように指定できる。
		/*
			Su(nday)	曜日。先頭の2文字。小文字でも可能。年月の指定と同時にできない。
			[12]?(1st|2nd|3rd|\dth)	日にち。年月の指定と同時にできない。
			5/$		月末。パースする前に28日に置換される。月日が確定したらその月の月末に移動する。
			+/-計算||月末記号を置換→普通にパース→月末に移動→<>@を反映
		*/
			DateTime       dt  = new DateTime ();
			CultureInfo    ci  = new CultureInfo("", true);
			DateTimeStyles dts = (DateTimeStyles.AllowTrailingWhite
			                   |  DateTimeStyles.NoCurrentDateDefault);
			try {
				// ((yy)yy/)MM/dd、12:34(:56)、およびその組み合わせ、が成功する。年が指定されていない場合は今年。日にちが指定されていない場合は西暦1年元旦。
				dt = DateTime.Parse(dtstr, ci, dts);

			} catch (FormatException) {
				dt = ParseString(dtstr, ci, dts);
			} finally {
				// 現時点に最も近い日時に変更する。
				DateTime today = DateTime.Today;
				if(dt.Year < 1000) {
					if(dt.Month == 1) {
						if(dt.Day == 1) {
							dt = today.Date + dt.TimeOfDay;
						} else {
							dt = dt.AddYears((dt.Year == 1 ? today.Year - 1 : 2000)).AddMonths(today.Month - 1);
						}
					} else {
						dt = dt.AddYears((dt.Year == 1 ? today.Year - 1 : 2000));
					}
				} else {
					if(dt.Month == 1 && today.Month != 1) {
						dt = dt.AddMonths(today.Month - 1);
					}
				}
				/*
				>日時		直後の。現時点より新しい日時
				<日時		直前の。現時点より古い日時
				@日時		直近の。現時点より新しい日時と古い日時とを比較して、時間的に近い方の日時
				+日/時	現時点から指定された時間を加算する。+1の代わりに+でも可能
				-日/時	現時点から指定された時間を減算する。-1の代わりに-でも可能
				例:
				+1 15:	翌日の15時。
				+1 +15:	翌日の同時刻から15時間後。
				+1 @15:	翌日の同時刻から最も近い15時。
				+sun
				*/
			}
			this.dateTime = dt;
		}

		private DateTime ParseString(string dtstr, CultureInfo ci, DateTimeStyles dts) {

			if(dtstr == "")	return DateTime.Now;		// この行がない場合、当日0時0分を返す。

			DateTime dt = new DateTime ();
			try {
				try {
					// スラッシュやハイフンがない場合、日付偶数桁で時間が指定されていないときのみ成功する。月が指定されていない場合は今年1月。年が指定されていない場合は今年。
					string[] candidates = {"d", "dd", "Mdd", "MMdd", "yMMdd", "yyMMdd", "M/d", "MM/d", "M/dd", "MM/dd", "M-d", "MM-d", "M-dd", "MM-dd", "h:00", "hh:00"};
					dt = DateTime.ParseExact(dtstr, candidates, ci, dts);
				} catch (FormatException) {
					Match m = Regex.Match(dtstr, @"(?(\d+:)|(((?<y>\d{1,4}[-/]?)(?<M>\d{2}[-/]?)(?<d>\d{2}))|((?<M>\d{1,2}[-/]?)(?<d>\d{2}))|(?<d>\d{1,2}))?)\s*((?<h>(?<!\d+)\d{1,2}(?=:)):(?<m>\d{1,2})?)?");
					if(m.Success){
						dt = new DateTime(
						           (m.Groups["y"].Success ?
						            //Convert.ToInt32(m.Groups["y"].Value.PadLeft(3, '0').PadLeft(4, '2')) :
						            Convert.ToInt32(m.Groups["y"].Value) :
						            1),
						           (m.Groups["M"].Success ? Convert.ToInt32(m.Groups["M"].Value) : 1),
						           (m.Groups["d"].Success ? Convert.ToInt32(m.Groups["d"].Value) : 1),
						           (m.Groups["h"].Success ? Convert.ToInt32(m.Groups["h"].Value) : 0),
						           (m.Groups["m"].Success ? Convert.ToInt32(m.Groups["m"].Value) : 0),
						           0);
						/* どっちが速いんやろ。一応Parseも残しとく。
						dt = DateTime.Parse((m.Groups["y"].Success ? m.Groups["y"].Value : "0001")
						                + "/" + (m.Groups["M"].Success ? m.Groups["M"].Value : "1")
						                + "/" + (m.Groups["d"].Success ? m.Groups["d"].Value : "1")
						                + " " + (m.Groups["h"].Success ? m.Groups["h"].Value : "0")
						                + ":" + (m.Groups["m"].Success ? m.Groups["m"].Value : "0"),
						                new CultureInfo("ja-JP", true),
						                DateTimeStyles.AllowTrailingWhite | DateTimeStyles.NoCurrentDateDefault);
						*/
					}
				}
			} catch {
				dt = DateTime.Now;
			}
			return dt;
		}
	}
}
