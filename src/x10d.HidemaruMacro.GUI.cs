// d.hatena.mac.dll.GUI.cs

using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

///	<example>
///		�ݒ���e��I��������_�C�A���O��\������}�N���B
///		���L�̂悤�ȃR�[�h���t�@�C���ɕۑ����A�ȉ��̂悤�ɃR���p�C������B
///		(%b = ���̃t�@�C���̃t�@�C����)
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
///			// ���ʔ���FOK�{�^�����N���b�N���ꂽ�B
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
				// OK�{�^���ƃL�����Z���{�^���𐶐�����
				btnOK = new Button ();
				btnCancel = new Button ();
				// �{�^���I�u�W�F�N�g�̑�����ݒ肷��
				btnOK.Text = "�I��";
				btnOK.DialogResult = DialogResult.OK;
				btnOK.Location = new Point (lbUser.Left, lbUser.Bottom + 1);

				btnCancel.Text = "���~";
				btnCancel.Location = new Point (btnOK.Left, btnOK.Bottom + 1);
				Controls.Add(btnOK);// OK�{�^�����t�H�[���ɒǉ�
				Controls.Add(btnCancel);// �L�����Z���{�^�����t�H�[���ɒǉ�

				// �{�[�_�X�^�C�����`����
				FormBorderStyle = FormBorderStyle.FixedDialog;
				MaximizeBox = false;// �ő剻�{�b�N�X�͕\�����Ȃ�
				MinimizeBox = false;// �ŏ����{�b�N�X�͕\�����Ȃ�
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
				//ToolTip�̐ݒ���s��
				//ToolTip���\�������܂ł̎���
				ttUser.InitialDelay = 2;
				//ToolTip���\������Ă��鎞�ɁA�ʂ�ToolTip��\������܂ł̎���
				ttUser.ReshowDelay = 1;
				//ToolTip��\�����鎞��
				ttUser.AutoPopDelay = 1;
				//�t�H�[�����A�N�e�B�u�łȂ����ł�ToolTip��\������
				ttUser.ShowAlways = true;
				//lbUser��ToolTip���\�������悤�ɂ���
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
				Items.Add("*ID������͂���*");
				EndUpdate();
				SelectedIndex = 0;
				Height = (Items.Count + 1) * ItemHeight;
			}
		}
	}
}
//TODO: �c�[���`�b�v��\������Bhttp://uchukamen.com/Programming1/ToolTip/index.htm
