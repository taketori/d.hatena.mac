// dateParse.cs
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace x10d.HidemaruMacro {
///<example>
///���̃N���X�̎g�����B
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
		//TODO: �j���B�����B���ɂ���25th�Ƃ����悤�Ɏw��ł���B
		/*
			Su(nday)	�j���B�擪��2�����B�������ł��\�B�N���̎w��Ɠ����ɂł��Ȃ��B
			[12]?(1st|2nd|3rd|\dth)	���ɂ��B�N���̎w��Ɠ����ɂł��Ȃ��B
			5/$		�����B�p�[�X����O��28���ɒu�������B�������m�肵���炻�̌��̌����Ɉړ�����B
			+/-�v�Z||�����L����u�������ʂɃp�[�X�������Ɉړ���<>@�𔽉f
		*/
			DateTime       dt  = new DateTime ();
			CultureInfo    ci  = new CultureInfo("", true);
			DateTimeStyles dts = (DateTimeStyles.AllowTrailingWhite
			                   |  DateTimeStyles.NoCurrentDateDefault);
			try {
				// ((yy)yy/)MM/dd�A12:34(:56)�A����т��̑g�ݍ��킹�A����������B�N���w�肳��Ă��Ȃ��ꍇ�͍��N�B���ɂ����w�肳��Ă��Ȃ��ꍇ�͐���1�N���U�B
				dt = DateTime.Parse(dtstr, ci, dts);

			} catch (FormatException) {
				dt = ParseString(dtstr, ci, dts);
			} finally {
				// �����_�ɍł��߂������ɕύX����B
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
				>����		����́B�����_���V��������
				<����		���O�́B�����_���Â�����
				@����		���߂́B�����_���V���������ƌÂ������Ƃ��r���āA���ԓI�ɋ߂����̓���
				+��/��	�����_����w�肳�ꂽ���Ԃ����Z����B+1�̑����+�ł��\
				-��/��	�����_����w�肳�ꂽ���Ԃ����Z����B-1�̑����-�ł��\
				��:
				+1 15:	������15���B
				+1 +15:	�����̓���������15���Ԍ�B
				+1 @15:	�����̓���������ł��߂�15���B
				+sun
				*/
			}
			this.dateTime = dt;
		}

		private DateTime ParseString(string dtstr, CultureInfo ci, DateTimeStyles dts) {

			if(dtstr == "")	return DateTime.Now;		// ���̍s���Ȃ��ꍇ�A����0��0����Ԃ��B

			DateTime dt = new DateTime ();
			try {
				try {
					// �X���b�V����n�C�t�����Ȃ��ꍇ�A���t�������Ŏ��Ԃ��w�肳��Ă��Ȃ��Ƃ��̂ݐ�������B�����w�肳��Ă��Ȃ��ꍇ�͍��N1���B�N���w�肳��Ă��Ȃ��ꍇ�͍��N�B
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
						/* �ǂ�������������B�ꉞParse���c���Ƃ��B
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
