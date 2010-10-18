// d.hatena.mac.dll.GUI.cs

using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

///	<example>
///		設定内容を選択させるダイアログを表示するマクロ。
///		下記のようなコードをファイルに保存し、以下のようにコンパイルする。
///		(%b = このファイルのファイル名)
///		csc.exe /R:d.hatena.mac.Settings.dll /t:winexe /w:4 %b
///	</example>
///	<code>
///	using x10d.HidemaruMacro.d_hatena_mac;
///	using x10d.HidemaruMacro.GUI;
///	class Sample {
///		static void Main () {
///			Hashtable ht = new Hashtable ();
///			ht["keyA"] = "valueA";
///			ht["keyB"] = "valueB";
///			ht["keyC"] = "valueC";
///			ChooseOneForm choiceForm = new ChooseOneForm (ht);
///			choiceForm.ShowDialog();
///			choiceForm.Dispose();
///
///			// 結果判定：OKボタンがクリックされた。
///			if (choiceForm.DialogResult == DialogResult.OK) {
///				MessageBox.Show(choiceForm.lbUser.SelectedItem.ToString() + " = " + ht[choiceForm.lbUser.SelectedItem.ToString()]);
///			}
///		}
///	}
///	</code>

namespace x10d.HidemaruMacro {
	namespace GUI {
		public class ChooseOneForm : Form {
			public ChoicesListBox lbUser;
			public Button btnOK;
			public Button btnCancel;

			public ChooseOneForm(Hashtable userPass) {
				lbUser = new ChoicesListBox (userPass);
				lbUser.DoubleClick += new EventHandler(this.lbUser_DoubleClicked);
				lbUser.Sorted = true;
				Controls.Add(lbUser);

				// http://msdn.microsoft.com/ja-jp/library/system.windows.forms.form.acceptb(v=VS.80).aspx
				// OKボタンとキャンセルボタンを生成する
				btnOK = new Button ();
				btnCancel = new Button ();
				// ボタンオブジェクトの属性を設定する
				btnOK.Text = "選択";
				btnOK.DialogResult = DialogResult.OK;
				btnOK.Location = new Point (lbUser.Left, lbUser.Bottom + 1);

				btnCancel.Text = "中止";
				btnCancel.Location = new Point (btnOK.Left, btnOK.Bottom + 1);
				Controls.Add(btnOK);// OKボタンをフォームに追加
				Controls.Add(btnCancel);// キャンセルボタンをフォームに追加

				// ボーダスタイルを定義する
				FormBorderStyle = FormBorderStyle.FixedDialog;
				MaximizeBox = false;// 最大化ボックスは表示しない
				MinimizeBox = false;// 最小化ボックスは表示しない
				Text = "Select a ID.";
				AcceptButton = btnOK;
				CancelButton = btnCancel;
				ClientSize = new System.Drawing.Size(lbUser.Right, btnCancel.Bottom);
				StartPosition = FormStartPosition.CenterScreen;
			}

			private void lbUser_DoubleClicked(object sender, EventArgs e) {
				this.DialogResult = DialogResult.OK;
			}

			private void ChooseOneForm_Load(object sender,EventArgs e){

				// http://dobon.net/vb/dotnet/control/showtooltip.html
				ToolTip ttUser = new ToolTip();
				//ToolTipの設定を行う
				//ToolTipが表示されるまでの時間
				ttUser.InitialDelay = 2;
				//ToolTipが表示されている時に、別のToolTipを表示するまでの時間
				ttUser.ReshowDelay = 1;
				//ToolTipを表示する時間
				ttUser.AutoPopDelay = 1;
				//フォームがアクティブでない時でもToolTipを表示する
				ttUser.ShowAlways = true;
				//lbUserにToolTipが表示されるようにする
				ttUser.SetToolTip(btnOK, "ToolTip desu.");
			}
		}

		public class ChoicesListBox : ListBox {
		// http://homepage3.nifty.com/aya_js/JScript.NET/event1.htm
			public ChoicesListBox(Hashtable userPass){
				BeginUpdate();
				foreach(string key in userPass.Keys){
					Items.Add(key);
				}
				Items.Add("*IDを手入力する*");
				EndUpdate();
				SelectedIndex = 0;
				Height = (Items.Count + 1) * ItemHeight;
			}
		}
	}
}
//TODO: ツールチップを表示する。http://uchukamen.com/Programming1/ToolTip/index.htm
