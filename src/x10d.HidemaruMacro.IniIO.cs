// d.hatena.mac.dll.IniIO.cs

using System;
using System.Runtime.InteropServices;

namespace x10d.HidemaruMacro {
	class IniIO {
		[DllImport("KERNEL32.DLL", EntryPoint="WritePrivateProfileString")]
		public static extern uint WriteIniStr(
			string lpAppName,
			string lpKeyName,
			string lpString,
			string lpFileName);
	}
}
