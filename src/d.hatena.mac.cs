// d.hatena.mac.cs
/** �R���p�C��:
csc.exe /R:d.hatena.mac.Settings.dll /t:winexe /w:4 %b .\x10d.HidemaruMacro\*
**/

using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Windows.Forms;
using x10d.HidemaruMacro;
using x10d.HidemaruMacro.GUI;
using x10d.HidemaruMacro.d_hatena_mac;

class d_hatena_mac {
	private static string selectUser() {
		ChooseOneForm choiceForm = new ChooseOneForm (Settings.hash);
		choiceForm.ShowDialog();
		choiceForm.Dispose();

		// ���ʔ���FOK�{�^�����N���b�N���ꂽ�B
		if (choiceForm.DialogResult == DialogResult.OK) {
			return choiceForm.lbUser.SelectedItem.ToString();
		}else{
			return "";
		}
	}

	private static string utcdatetime() {
		DateTime d = DateTime.UtcNow;
		return String.Format("{0}-{1}-{2}T{3}:{4}:{5}Z",
						d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second);
	}

	private static string b64_sha1(string source) {
		byte[] hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(source));
		return Convert.ToBase64String(hash);
	}

	private static int compare_filedate(string exebase) {
		// �R���p�C�����ꂽDLL�ƃ\�[�X�t�@�C���Ƃ̐V�����r���āA���̌��ʂ��I���R�[�h�ɂ��ďI������B0=���s�t�@�C���̕����Â��B0�ȊO=�R���p�C���s�v(���s�t�@�C���̕����V�������\�[�X�����݂��Ȃ��B)
		//NOTE: CompareTo�̕Ԓl: 0���傫��=�C���X�^���X���������傫�����A������null�Q��
		string dll, src;
		dll = exebase + ".util.dll";
		src = exebase + ".Settings.cs";
		if(File.Exists(dll)
		&& 0 < File.GetLastWriteTime(dll).CompareTo(File.GetLastWriteTime(src))){
			Console.WriteLine("DLL�t�@�C�����R���p�C���������_���ŐV�̃\�[�X�͑��݂��܂���B");
			return 1;
		}else{
			return 0;
		}
	}

	private static int generate_xwsse(string user, string inifile, string section) {

		// X-WSSE�̃f�[�^�𐶐�����B
		string nonce, nonceEncoded, created, passwordDigest;
		nonce = b64_sha1(String.Format("{0}{1}x10d-d.hatena.mac", utcdatetime(), new Random().Next(100)));
		nonceEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(nonce));
		created = utcdatetime();
		passwordDigest = b64_sha1(nonce + created + Settings.hash[user]);

		// ini�t�@�C���ɏ������ށB
		if(Settings.hash[user].ToString() == ""){
			IniIO.WriteIniStr(section, "x-wsse", "", inifile);
			return 1;
		}else{
			IniIO.WriteIniStr(section, "x-wsse",
			  String.Format("UsernameToken Username=\"{0}\", PasswordDigest=\"{1}\", Created=\"{2}\", Nonce=\"{3}\"", user, passwordDigest, created, nonceEncoded),
			  inifile);
			return 0;
		}
	}

	private static int show_messagebox(string title, string message) {
		// YesNoCancel���b�Z�[�W�{�b�N�X��\������B
		//ret: DialogResult�񋓑�
		return (int)MessageBox.Show(message, title,
		                       MessageBoxButtons.YesNoCancel,
		                       MessageBoxIcon.Question);
	}

	public static int Main(string[] args) {
		string exebase, section, user;
		section = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
		exebase = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), section);

		if(args.Length == 0)	return 1;
		switch(args[0]) {
			case "compare":
				return compare_filedate(exebase);
			case "date":
				switch(args.Length){
					case 1:
						IniIO.WriteIniStr(section, "date", new StringToDateTime().dateTime.ToString("s"), exebase + ".ini");
						return 0;
					case 2:
						IniIO.WriteIniStr(section, "date", new StringToDateTime(args[1]).dateTime.ToString("s"), exebase + ".ini");
						return 0;
					default:
						return 1;
				}
			case "msgbox":
				if(args.Length < 3)	return 1;
				return show_messagebox(args[1], args[2]);
			case "user":
				user = "";
				switch(args.Length){
					case 1:
						MessageBox.Show("No user name is setted. Select one.");
						user = selectUser();
						break;
					case 2:
						user = args[1];
						if(!Settings.hash.ContainsKey(user))	user = selectUser();
						break;
					case 3:
						user = args[1];
						Settings.hash[user] = args[2];
						break;
					default:
						return 1;
				}
				IniIO.WriteIniStr(section, "user", user, exebase + ".ini");
				if(user == "")	return 1;
				else	          return generate_xwsse(user, exebase + ".ini", section);
			default:
				return 1;
		}
	}
}